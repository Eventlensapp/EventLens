using EventLensAI.API.Hubs;
using EventLensAI.Application.Features.AI;
using Microsoft.AspNetCore.SignalR;
namespace EventLensAI.API.Services;
internal sealed class SignalRAIJobNotifier(IHubContext<AIJobsHub> hub):IAIJobNotifier
{
 public Task StartedAsync(AIJobDto job,CancellationToken ct)=>hub.Clients.Group($"event:{job.EventId}").SendAsync("JobStarted",job,ct);
 public Task ProgressAsync(Guid eventId,Guid id,int progress,CancellationToken ct)=>hub.Clients.Group($"event:{eventId}").SendAsync("JobProgress",new{jobId=id,progress},ct);
 public Task CompletedAsync(AIJobDto job,CancellationToken ct)=>hub.Clients.Group($"event:{job.EventId}").SendAsync("JobCompleted",job,ct);
 public Task FailedAsync(AIJobDto job,CancellationToken ct)=>hub.Clients.Group($"event:{job.EventId}").SendAsync("JobFailed",job,ct);
}
