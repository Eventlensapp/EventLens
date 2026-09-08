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
    Task<UserToken?> GetUserTokenAsync(string tokenHash,string purpose,CancellationToken cancellationToken);
    Task AddUserTokenAsync(UserToken token,CancellationToken cancellationToken);
    Task<UserPreference?> GetPreferenceAsync(Guid userId,CancellationToken cancellationToken);
    Task AddPreferenceAsync(UserPreference preference,CancellationToken cancellationToken);
    Task<IReadOnlyList<RefreshToken>> ListSessionsAsync(Guid userId,CancellationToken cancellationToken);
    Task<RefreshToken?> GetSessionAsync(Guid userId,Guid id,CancellationToken cancellationToken);
    Task<IReadOnlyList<ActivityLog>> ListActivityAsync(Guid userId,int take,CancellationToken cancellationToken);
    Task AddActivityAsync(ActivityLog activity,CancellationToken cancellationToken);
    Task<IReadOnlyList<ApiKey>> ListApiKeysAsync(Guid userId,CancellationToken cancellationToken);
    Task<ApiKey?> GetApiKeyAsync(Guid userId,Guid id,CancellationToken cancellationToken);
    Task AddApiKeyAsync(ApiKey key,CancellationToken cancellationToken);
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
    string CreateOpaqueToken();
}

public sealed record AccessToken(string Value, DateTime ExpiresAt);
