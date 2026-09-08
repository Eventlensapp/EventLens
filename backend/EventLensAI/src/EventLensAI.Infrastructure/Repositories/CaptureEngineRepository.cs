using EventLensAI.Application.Features.Booth;
using EventLensAI.Domain.Entities;
using EventLensAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace EventLensAI.Infrastructure.Repositories;
internal sealed class CaptureEngineRepository(EventLensDbContext db):ICaptureEngineRepository
{
 public Task<CaptureConfiguration?>GetConfigurationAsync(Guid organizationId,Guid eventId,CancellationToken ct)=>db.CaptureConfigurations.FirstOrDefaultAsync(x=>x.OrganizationId==organizationId&&x.EventId==eventId,ct);
 public Task AddConfigurationAsync(CaptureConfiguration value,CancellationToken ct)=>db.CaptureConfigurations.AddAsync(value,ct).AsTask();
 public Task<CapturedPhoto?>GetPhotoAsync(Guid id,CancellationToken ct)=>db.CapturedPhotos.FirstOrDefaultAsync(x=>x.Id==id,ct);
 public Task AddPhotoAsync(CapturedPhoto value,CancellationToken ct)=>db.CapturedPhotos.AddAsync(value,ct).AsTask();
 public async Task<IReadOnlyList<CapturedPhoto>>ListAsync(Guid sessionId,CancellationToken ct)=>await db.CapturedPhotos.AsNoTracking().Where(x=>x.SessionId==sessionId).OrderBy(x=>x.CaptureNumber).ThenByDescending(x=>x.CapturedAt).ToListAsync(ct);
}
