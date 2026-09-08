using EventLensAI.API.Hubs;
using EventLensAI.Application.Features.Analytics;
using EventLensAI.Domain.Enums;
using Microsoft.AspNetCore.SignalR;
namespace EventLensAI.API.Services;
internal sealed class SignalRAnalyticsNotifier(IHubContext<AnalyticsHub> hub):IAnalyticsNotifier
{
 public async Task MetricRecordedAsync(Guid organizationId,Guid? eventId,AnalyticsMetric metric,int value,CancellationToken ct){var payload=new{organizationId,eventId,metric=metric.ToString(),value,occurredAt=DateTime.UtcNow};await hub.Clients.Group($"analytics:organization:{organizationId}").SendAsync("MetricRecorded",payload,ct);if(eventId.HasValue)await hub.Clients.Group($"analytics:event:{eventId}").SendAsync("MetricRecorded",payload,ct);}
}
