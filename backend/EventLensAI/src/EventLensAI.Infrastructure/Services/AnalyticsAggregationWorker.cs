using EventLensAI.Domain.Entities;
using EventLensAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace EventLensAI.Infrastructure.Services;
public sealed class AnalyticsAggregationWorker(IServiceScopeFactory scopes,ILogger<AnalyticsAggregationWorker> logger):BackgroundService
{
 protected override async Task ExecuteAsync(CancellationToken stoppingToken){using var timer=new PeriodicTimer(TimeSpan.FromMinutes(5));do{try{await AggregateAsync(stoppingToken);}catch(OperationCanceledException)when(stoppingToken.IsCancellationRequested){break;}catch(Exception ex){logger.LogError(ex,"Analytics aggregation cycle failed");}}while(await timer.WaitForNextTickAsync(stoppingToken));}
 async Task AggregateAsync(CancellationToken ct){await using var scope=scopes.CreateAsyncScope();var db=scope.ServiceProvider.GetRequiredService<EventLensDbContext>();var start=DateTime.UtcNow.Date.AddDays(-1);var rows=await db.AnalyticsRecords.AsNoTracking().Where(x=>x.OccurredAt>=start).ToListAsync(ct);var groups=rows.GroupBy(x=>new{x.OrganizationId,Date=DateOnly.FromDateTime(x.OccurredAt),x.Metric,x.EventId,x.TemplateId,x.PhotographerId});foreach(var g in groups){var key=g.Key;var existing=await db.AnalyticsDailyAggregates.SingleOrDefaultAsync(x=>x.OrganizationId==key.OrganizationId&&x.Date==key.Date&&x.Metric==key.Metric&&x.EventId==key.EventId&&x.TemplateId==key.TemplateId&&x.PhotographerId==key.PhotographerId,ct);var total=g.Sum(x=>x.Value);var unique=g.Where(x=>x.SessionKeyHash!=null).Select(x=>x.SessionKeyHash).Distinct().Count();if(existing is null)db.Add(new AnalyticsDailyAggregate(key.OrganizationId,key.Date,key.Metric,key.EventId,key.TemplateId,key.PhotographerId,total,unique));else existing.Replace(total,unique);}await db.SaveChangesAsync(ct);}
}
