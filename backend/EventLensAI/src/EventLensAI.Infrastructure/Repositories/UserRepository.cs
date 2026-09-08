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
}
