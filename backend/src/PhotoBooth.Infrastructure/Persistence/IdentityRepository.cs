using Microsoft.EntityFrameworkCore;
using PhotoBooth.Application.Abstractions.Authentication;
using PhotoBooth.Domain.Entities;

namespace PhotoBooth.Infrastructure.Persistence;

internal sealed class IdentityRepository(AppDbContext context) : IIdentityRepository
{
    public Task<User?> FindUserByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) =>
        context.Users.FirstOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);

    public Task<RefreshToken?> FindRefreshTokenAsync(string tokenHash, CancellationToken cancellationToken) =>
        context.RefreshTokens
            .Include(token => token.User)
            .FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

    public async Task<IReadOnlyList<RefreshToken>> ListActiveTokenFamilyAsync(
        Guid userId, Guid familyId, CancellationToken cancellationToken) =>
        await context.RefreshTokens
            .Where(token => token.UserId == userId && token.FamilyId == familyId &&
                            token.RevokedAtUtc == null && token.ExpiresAtUtc > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

    public Task AddUserAsync(User user, CancellationToken cancellationToken) =>
        context.Users.AddAsync(user, cancellationToken).AsTask();

    public Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken) =>
        context.RefreshTokens.AddAsync(refreshToken, cancellationToken).AsTask();
}
