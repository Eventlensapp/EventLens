using EventLensAI.Application.Features.AI;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace EventLensAI.Infrastructure.AI;
internal sealed class AIJobWorker(IAIJobQueue queue,IServiceScopeFactory scopes,ILogger<AIJobWorker> logger):BackgroundService
{
 protected override async Task ExecuteAsync(CancellationToken stoppingToken)
 {
  while(!stoppingToken.IsCancellationRequested){var id=await queue.DequeueAsync(stoppingToken);await Process(id,stoppingToken);}
 }
 private async Task Process(Guid id,CancellationToken ct)
 {
  using var scope=scopes.CreateScope();var jobs=scope.ServiceProvider.GetRequiredService<IAIJobRepository>();var unit=scope.ServiceProvider.GetRequiredService<IUnitOfWork>();var resolver=scope.ServiceProvider.GetRequiredService<IAIProviderResolver>();var notify=scope.ServiceProvider.GetRequiredService<IAIJobNotifier>();
  var job=await jobs.GetAsync(id,ct);if(job is null||job.Status is not (AIJobStatus.Queued or AIJobStatus.RetryScheduled))return;
  try{job.Start();await unit.SaveChangesAsync(ct);await notify.StartedAsync(AIProcessingService.Map(job),ct);
   var progress=new Progress<int>(value=>{job.ReportProgress(value);_ = notify.ProgressAsync(job.EventId,job.Id,value,ct);});
   var provider=resolver.Resolve(job.Provider,job.JobType);var result=await provider.ProcessAsync(new(job.Id,job.JobType,job.InputImage,job.Prompt,job.NegativePrompt,new Dictionary<string,string>()),progress,ct);
   job.Complete(result.OutputImage);
   if(job.PhotoId.HasValue){var photos=scope.ServiceProvider.GetRequiredService<IPhotoRepository>();var photo=await photos.GetAsync(job.PhotoId.Value,ct);if(photo is not null)photo.Complete(result.OutputImage,photo.ThumbnailUrl,photo.Width,photo.Height,photo.FileSize,photo.Format);}
   await unit.SaveChangesAsync(ct);await notify.CompletedAsync(AIProcessingService.Map(job),ct);}
  catch(Exception ex){var retry=job.RetryCount<3;job.Fail(ex.Message,retry);await unit.SaveChangesAsync(ct);await notify.FailedAsync(AIProcessingService.Map(job),ct);logger.LogError(ex,"AI job {JobId} failed",id);if(retry){await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2,job.RetryCount)),ct);job.Requeue();await unit.SaveChangesAsync(ct);await queue.QueueAsync(id,ct);}}
 }
}
