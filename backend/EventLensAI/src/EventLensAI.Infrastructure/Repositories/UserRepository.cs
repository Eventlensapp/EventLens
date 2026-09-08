using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Domain.Entities;
using EventLensAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventLensAI.Infrastructure.Repositories;

internal sealed class UserRepository(EventLensDbContext context) : IUserRepository
{
    public Task<User?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken) =>
        context.Users.Include(x => x.Memberships).ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
    public Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken) =>
        context.Users.Include(x => x.Memberships).ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash, CancellationToken cancellationToken) =>
        context.RefreshTokens.IgnoreQueryFilters().Include(x => x.User)
            .ThenInclude(x => x.Memberships).ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
    public async Task<IReadOnlyList<RefreshToken>> GetActiveTokenFamilyAsync(
        Guid userId, Guid familyId, CancellationToken cancellationToken) =>
        await context.RefreshTokens
            .Where(x => x.UserId == userId && x.FamilyId == familyId &&
                        x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    public Task AddUserAsync(User user, CancellationToken cancellationToken) =>
        context.Users.AddAsync(user, cancellationToken).AsTask();
    public Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken) =>
        context.RefreshTokens.AddAsync(refreshToken, cancellationToken).AsTask();
    public Task<UserToken?> GetUserTokenAsync(string hash,string purpose,CancellationToken ct)=>context.UserTokens.Include(x=>x.User).FirstOrDefaultAsync(x=>x.TokenHash==hash&&x.Purpose==purpose,ct);
    public Task AddUserTokenAsync(UserToken token,CancellationToken ct)=>context.UserTokens.AddAsync(token,ct).AsTask();
    public Task<UserPreference?> GetPreferenceAsync(Guid userId,CancellationToken ct)=>context.UserPreferences.FirstOrDefaultAsync(x=>x.UserId==userId,ct);
    public Task AddPreferenceAsync(UserPreference preference,CancellationToken ct)=>context.UserPreferences.AddAsync(preference,ct).AsTask();
    public async Task<IReadOnlyList<RefreshToken>> ListSessionsAsync(Guid userId,CancellationToken ct)=>await context.RefreshTokens.Where(x=>x.UserId==userId&&x.RevokedAt==null&&x.ExpiresAt>DateTime.UtcNow).OrderByDescending(x=>x.LastActivityAt).ToListAsync(ct);
    public Task<RefreshToken?> GetSessionAsync(Guid userId,Guid id,CancellationToken ct)=>context.RefreshTokens.FirstOrDefaultAsync(x=>x.UserId==userId&&x.Id==id,ct);
    public async Task<IReadOnlyList<ActivityLog>> ListActivityAsync(Guid userId,int take,CancellationToken ct)=>await context.ActivityLogs.Where(x=>x.UserId==userId).OrderByDescending(x=>x.CreatedAt).Take(take).ToListAsync(ct);
    public Task AddActivityAsync(ActivityLog activity,CancellationToken ct)=>context.ActivityLogs.AddAsync(activity,ct).AsTask();
    public async Task<IReadOnlyList<ApiKey>> ListApiKeysAsync(Guid userId,CancellationToken ct)=>await context.ApiKeys.Where(x=>x.UserId==userId).OrderByDescending(x=>x.CreatedAt).ToListAsync(ct);
    public Task<ApiKey?> GetApiKeyAsync(Guid userId,Guid id,CancellationToken ct)=>context.ApiKeys.FirstOrDefaultAsync(x=>x.UserId==userId&&x.Id==id,ct);
    public Task AddApiKeyAsync(ApiKey key,CancellationToken ct)=>context.ApiKeys.AddAsync(key,ct).AsTask();
}
