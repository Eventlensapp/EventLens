using PhotoBooth.Domain.Entities;

namespace PhotoBooth.Application.Abstractions.Authentication;

public interface ITokenService
{
    AccessTokenResult GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashRefreshToken(string token);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}

public interface IIdentityRepository
{
    Task<User?> FindUserByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);
    Task<RefreshToken?> FindRefreshTokenAsync(string tokenHash, CancellationToken cancellationToken);
    Task<IReadOnlyList<RefreshToken>> ListActiveTokenFamilyAsync(
        Guid userId, Guid familyId, CancellationToken cancellationToken);
    Task AddUserAsync(User user, CancellationToken cancellationToken);
    Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
}

public sealed record AccessTokenResult(string Token, DateTime ExpiresAtUtc);
