using EventLensAI.Application.Features.Photos.Templates;
using EventLensAI.Domain.Entities;
using EventLensAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventLensAI.Infrastructure.Repositories;

public sealed class PhotoTemplateRepository(EventLensDbContext db) : IPhotoTemplateRepository
{
    public Task<PhotoTemplate?> GetAsync(Guid id,CancellationToken ct)=>db.PhotoTemplates.SingleOrDefaultAsync(x=>x.Id==id,ct);
    public async Task<IReadOnlyList<PhotoTemplate>> ListAsync(Guid org,CancellationToken ct)=>await db.PhotoTemplates.AsNoTracking().Where(x=>x.OrganizationId==org||x.IsPublic).OrderByDescending(x=>x.CreatedAt).ToListAsync(ct);
    public Task<TemplateLayout?> LayoutAsync(Guid id,CancellationToken ct)=>db.TemplateLayouts.SingleOrDefaultAsync(x=>x.PhotoTemplateId==id,ct);
    public async Task<IReadOnlyList<TemplateElement>> ElementsAsync(Guid id,CancellationToken ct)=>await db.TemplateElements.Where(x=>x.PhotoTemplateId==id).OrderBy(x=>x.LayerOrder).ToListAsync(ct);
    public Task AddAsync(PhotoTemplate x,CancellationToken ct)=>db.PhotoTemplates.AddAsync(x,ct).AsTask();
    public Task AddLayoutAsync(TemplateLayout x,CancellationToken ct)=>db.TemplateLayouts.AddAsync(x,ct).AsTask();
    public async Task AddElementsAsync(IEnumerable<TemplateElement>x,CancellationToken ct)=>await db.TemplateElements.AddRangeAsync(x,ct);
    public void RemoveLayout(TemplateLayout x)=>db.TemplateLayouts.Remove(x);
    public void RemoveElements(IEnumerable<TemplateElement>x)=>db.TemplateElements.RemoveRange(x);
    public async Task<IReadOnlyList<Sticker>> StickersAsync(Guid org,CancellationToken ct)=>await db.Stickers.AsNoTracking().Where(x=>x.IsPublic||x.OrganizationId==org).OrderBy(x=>x.Name).ToListAsync(ct);
}
