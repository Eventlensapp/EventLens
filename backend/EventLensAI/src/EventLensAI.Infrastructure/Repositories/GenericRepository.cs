using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Common;
using EventLensAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventLensAI.Infrastructure.Repositories;

internal sealed class GenericRepository<TEntity>(EventLensDbContext context) : IRepository<TEntity>
    where TEntity : BaseEntity
{
    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);

    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Set<TEntity>().FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        context.Set<TEntity>().AddAsync(entity, cancellationToken).AsTask();

    public void Update(TEntity entity) => context.Set<TEntity>().Update(entity);
    public void Delete(TEntity entity, Guid? actorId = null) => entity.SoftDelete(actorId);
}
