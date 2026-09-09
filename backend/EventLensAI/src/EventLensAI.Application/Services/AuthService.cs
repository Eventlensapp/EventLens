using EventLensAI.Application.DTOs.Auth;
using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Entities;

namespace EventLensAI.Application.Services;

public sealed class AuthService(
    IUserRepository users,
    IPasswordService passwords,
    ITokenService tokens,
    ICurrentUserService current,
    IUnitOfWork unitOfWork) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();
        if (await users.GetByNormalizedEmailAsync(normalizedEmail, cancellationToken) is not null)
            throw new ConflictException("An account with this email already exists.");

        var passwordHash = passwords.Hash(request.Password);
        var user = new User(request.FirstName, request.LastName, request.Email, passwordHash, request.Phone);
        await users.AddUserAsync(user, cancellationToken);
        await users.AddActivityAsync(new(user.Id,"account.registered","Account registered.",current.IPAddress,null),cancellationToken);
        return await IssueAsync(user, [SystemRoles.Guest], Guid.NewGuid(), cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await users.GetByNormalizedEmailAsync(
            request.Email.Trim().ToUpperInvariant(), cancellationToken);
        if(user?.IsLockedOut==true)throw new UnauthorizedException("Account is temporarily locked. Try again later.");
        if (user is null || !user.IsActive || !passwords.Verify(user, request.Password, user.PasswordHash))
        {
            if(user is not null){user.RecordFailedLogin();await users.AddActivityAsync(new(user.Id,"login.failed","Failed login attempt.",current.IPAddress,null),cancellationToken);await unitOfWork.SaveChangesAsync(cancellationToken);}
            throw new UnauthorizedException("Invalid email or password.");
        }

        user.RecordLogin();
        await users.AddActivityAsync(new(user.Id,"login.succeeded","Signed in successfully.",current.IPAddress,null),cancellationToken);
        var roles = RolesFor(user);
        return await IssueAsync(user, roles, Guid.NewGuid(), cancellationToken);
    }

    public async Task<AuthResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var stored = await users.GetRefreshTokenAsync(tokens.HashRefreshToken(refreshToken), cancellationToken);
        if (stored is null) throw new UnauthorizedException("Refresh token is invalid.");
        if (!stored.IsActive)
        {
            var family = await users.GetActiveTokenFamilyAsync(stored.UserId, stored.FamilyId, cancellationToken);
            foreach (var member in family) member.Revoke();
            await unitOfWork.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedException("Refresh token is expired or has been reused.");
        }

        var newRawToken = tokens.CreateRefreshToken();
        var replacement = new RefreshToken(
            stored.UserId, tokens.HashRefreshToken(newRawToken), stored.FamilyId, DateTime.UtcNow.AddDays(30),stored.DeviceInfo,current.IPAddress);
        stored.Revoke(replacement.Id);
        await users.AddRefreshTokenAsync(replacement, cancellationToken);
        var roles = RolesFor(stored.User);
        var access = tokens.CreateAccessToken(stored.User, roles);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Response(stored.User, roles, access, newRawToken);
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var stored = await users.GetRefreshTokenAsync(tokens.HashRefreshToken(refreshToken), cancellationToken);
        if (stored?.IsActive == true)
        {
            stored.Revoke();
            await users.AddActivityAsync(new(stored.UserId,"logout","Session signed out.",current.IPAddress,stored.DeviceInfo),cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<AuthResponse> IssueAsync(
        User user, IReadOnlyCollection<string> roles, Guid familyId, CancellationToken cancellationToken)
    {
        var access = tokens.CreateAccessToken(user, roles);
        var rawRefresh = tokens.CreateRefreshToken();
        await users.AddRefreshTokenAsync(new RefreshToken(
            user.Id, tokens.HashRefreshToken(rawRefresh), familyId, DateTime.UtcNow.AddDays(30),null,current.IPAddress), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Response(user, roles, access, rawRefresh);
    }

    private static IReadOnlyCollection<string> RolesFor(User user)
    {
        var roles = user.Memberships.Select(x => x.Role.Name).Distinct().ToArray();
        return roles.Length == 0 ? [SystemRoles.Guest] : roles;
    }

    private static AuthResponse Response(
        User user, IReadOnlyCollection<string> roles, AccessToken access, string refreshToken) =>
        new(access.Value, refreshToken, access.ExpiresAt,
            new UserDto(user.Id, user.FirstName, user.LastName, user.Email, roles, user.MustChangePassword));
}
