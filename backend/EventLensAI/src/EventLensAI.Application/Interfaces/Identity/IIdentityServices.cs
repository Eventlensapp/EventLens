using EventLensAI.Domain.Entities;

namespace EventLensAI.Application.Interfaces.Identity;

public interface IUserRepository
{
    Task<User?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken);
    Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken);
    Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash, CancellationToken cancellationToken);
    Task<IReadOnlyList<RefreshToken>> GetActiveTokenFamilyAsync(
        Guid userId, Guid familyId, CancellationToken cancellationToken);
    Task AddUserAsync(User user, CancellationToken cancellationToken);
    Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
}

public interface IPasswordService
{
    string Hash(string password);
    bool Verify(User user, string password, string passwordHash);
    bool Verify(string password,string passwordHash);
}

public interface ITokenService
{
    AccessToken CreateAccessToken(User user, IReadOnlyCollection<string> roles);
    string CreateRefreshToken();
    string HashRefreshToken(string refreshToken);
}

public sealed record AccessToken(string Value, DateTime ExpiresAt);
