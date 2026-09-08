using EventLensAI.Application.Features.Booth;using EventLensAI.Domain.Entities;using EventLensAI.Infrastructure.Persistence;using Microsoft.EntityFrameworkCore;
namespace EventLensAI.Infrastructure.Repositories;
public sealed class GuestExperienceRepository(EventLensDbContext db):IGuestExperienceRepository
{
 public Task<BoothExperienceConfiguration?>ConfigurationAsync(Guid eventId,CancellationToken ct)=>db.BoothExperienceConfigurations.SingleOrDefaultAsync(x=>x.EventId==eventId,ct);
 public Task<GuestBoothSession?>SessionAsync(Guid id,CancellationToken ct)=>db.GuestBoothSessions.SingleOrDefaultAsync(x=>x.Id==id,ct);
 public Task AddConfigurationAsync(BoothExperienceConfiguration x,CancellationToken ct)=>db.BoothExperienceConfigurations.AddAsync(x,ct).AsTask();
 public Task AddSessionAsync(GuestBoothSession x,CancellationToken ct)=>db.GuestBoothSessions.AddAsync(x,ct).AsTask();
 public Task AddLogAsync(BoothExperienceLog x,CancellationToken ct)=>db.BoothExperienceLogs.AddAsync(x,ct).AsTask();
}
