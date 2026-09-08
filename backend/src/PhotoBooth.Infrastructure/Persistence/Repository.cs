using Microsoft.EntityFrameworkCore;
using PhotoBooth.Application.Abstractions.Persistence;
using PhotoBooth.Domain.Common;

namespace PhotoBooth.Infrastructure.Persistence;

internal sealed class Repository<TEntity>(AppDbContext context) : IRepository<TEntity>
    where TEntity : Entity
{
    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default) =>
        await context.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        context.Set<TEntity>().AddAsync(entity, cancellationToken).AsTask();

    public void Update(TEntity entity) => context.Set<TEntity>().Update(entity);
    public void Remove(TEntity entity) => context.Set<TEntity>().Remove(entity);
}
