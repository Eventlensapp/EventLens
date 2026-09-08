using EventLensAI.Application.Features.Booth;using EventLensAI.Domain.Entities;using EventLensAI.Domain.Enums;using EventLensAI.Infrastructure.Persistence;using Microsoft.EntityFrameworkCore;
namespace EventLensAI.Infrastructure.Repositories;
internal sealed class BoothSessionEngineRepository(EventLensDbContext db):IBoothSessionEngineRepository
{
 public Task<BoothSession?>GetAsync(Guid id,CancellationToken ct)=>db.BoothSessions.FirstOrDefaultAsync(x=>x.Id==id,ct);
 public Task<BoothSession?>GetByTokenAsync(string token,CancellationToken ct)=>db.BoothSessions.FirstOrDefaultAsync(x=>x.SessionToken==token,ct);
 public Task AddActivityAsync(BoothSessionActivity value,CancellationToken ct)=>db.BoothSessionActivities.AddAsync(value,ct).AsTask();
 public async Task<IReadOnlyList<BoothSessionActivity>>ListActivitiesAsync(Guid id,CancellationToken ct)=>await db.BoothSessionActivities.AsNoTracking().Where(x=>x.SessionId==id).OrderBy(x=>x.Timestamp).ToListAsync(ct);
 public async Task<IReadOnlyList<BoothSession>>ListExpiredCandidatesAsync(DateTime before,CancellationToken ct)=>await db.BoothSessions.Where(x=>(x.Status==BoothSessionStatus.Active||x.Status==BoothSessionStatus.Paused||x.Status==BoothSessionStatus.Initializing)&&x.LastActivityAt<before).ToListAsync(ct);
}
