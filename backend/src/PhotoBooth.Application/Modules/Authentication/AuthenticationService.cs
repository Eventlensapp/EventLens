using PhotoBooth.Application.Abstractions.Authentication;
using PhotoBooth.Application.Abstractions.Persistence;
using PhotoBooth.Domain.Entities;

namespace PhotoBooth.Application.Modules.Authentication;

public sealed class AuthenticationService(
    IIdentityRepository identityRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork) : IAuthenticationService
{
    public async Task<AuthenticationResult> RegisterAsync(
        RegisterCommand command, CancellationToken cancellationToken)
    {
        var validationError = Validate(command.Email, command.Password, command.DisplayName);
        if (validationError is not null) return AuthenticationResult.Failure(validationError);

        var normalizedEmail = command.Email.Trim().ToUpperInvariant();
        if (await identityRepository.FindUserByEmailAsync(normalizedEmail, cancellationToken) is not null)
            return AuthenticationResult.Failure("email_already_registered");

        var user = new User(command.Email, passwordHasher.Hash(command.Password), command.DisplayName);
        await identityRepository.AddUserAsync(user, cancellationToken);
        return await IssueTokensAsync(user, Guid.NewGuid(), cancellationToken);
    }

    public async Task<AuthenticationResult> LoginAsync(
        LoginCommand command, CancellationToken cancellationToken)
    {
        var normalizedEmail = command.Email.Trim().ToUpperInvariant();
        var user = await identityRepository.FindUserByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null || !user.IsActive || !passwordHasher.Verify(command.Password, user.PasswordHash))
            return AuthenticationResult.Failure("invalid_credentials");

        user.RecordLogin();
        return await IssueTokensAsync(user, Guid.NewGuid(), cancellationToken);
    }

    public async Task<AuthenticationResult> RefreshAsync(
        RefreshCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
            return AuthenticationResult.Failure("invalid_refresh_token");

        var tokenHash = tokenService.HashRefreshToken(command.RefreshToken);
        var existing = await identityRepository.FindRefreshTokenAsync(tokenHash, cancellationToken);
        if (existing is null) return AuthenticationResult.Failure("invalid_refresh_token");

        if (!existing.IsActive)
        {
            var family = await identityRepository.ListActiveTokenFamilyAsync(
                existing.UserId, existing.FamilyId, cancellationToken);
            foreach (var token in family) token.Revoke();
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return AuthenticationResult.Failure("refresh_token_reused_or_expired");
        }

        var rawToken = tokenService.GenerateRefreshToken();
        var replacement = new RefreshToken(
            existing.UserId,
            tokenService.HashRefreshToken(rawToken),
            existing.FamilyId,
            DateTime.UtcNow.AddDays(30));
        existing.Revoke(replacement.Id);
        await identityRepository.AddRefreshTokenAsync(replacement, cancellationToken);

        var accessToken = tokenService.GenerateAccessToken(existing.User);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Success(existing.User, accessToken, rawToken);
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return;
        var existing = await identityRepository.FindRefreshTokenAsync(
            tokenService.HashRefreshToken(refreshToken), cancellationToken);
        if (existing is null || !existing.IsActive) return;
        existing.Revoke();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<AuthenticationResult> IssueTokensAsync(
        User user, Guid familyId, CancellationToken cancellationToken)
    {
        var accessToken = tokenService.GenerateAccessToken(user);
        var rawRefreshToken = tokenService.GenerateRefreshToken();
        await identityRepository.AddRefreshTokenAsync(new RefreshToken(
            user.Id,
            tokenService.HashRefreshToken(rawRefreshToken),
            familyId,
            DateTime.UtcNow.AddDays(30)), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Success(user, accessToken, rawRefreshToken);
    }

    private static AuthenticationResult Success(
        User user, AccessTokenResult accessToken, string refreshToken) =>
        new(true, null, accessToken.Token, refreshToken, accessToken.ExpiresAtUtc,
            new UserResponse(user.Id, user.Email, user.DisplayName, user.Role));

    private static string? Validate(string email, string password, string displayName)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@')) return "invalid_email";
        if (string.IsNullOrWhiteSpace(displayName) || displayName.Trim().Length is < 2 or > 100)
            return "invalid_display_name";
        if (password.Length < 12 || !password.Any(char.IsUpper) ||
            !password.Any(char.IsLower) || !password.Any(char.IsDigit))
            return "weak_password";
        return null;
    }
}
