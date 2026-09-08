using EventLensAI.Application.Services;using Microsoft.Extensions.DependencyInjection;using Microsoft.Extensions.Hosting;using Microsoft.Extensions.Logging;
namespace EventLensAI.Infrastructure.Services;
internal sealed class SessionExpiryWorker(IServiceScopeFactory scopes,ILogger<SessionExpiryWorker> logger):BackgroundService
{
 protected override async Task ExecuteAsync(CancellationToken stoppingToken)
 {
  using var timer=new PeriodicTimer(TimeSpan.FromSeconds(30));
  while(await timer.WaitForNextTickAsync(stoppingToken)){try{await using var scope=scopes.CreateAsyncScope();var count=await scope.ServiceProvider.GetRequiredService<IBoothSessionService>().ExpireInactiveAsync(300,stoppingToken);if(count>0)logger.LogInformation("Expired {SessionCount} inactive booth sessions.",count);}catch(OperationCanceledException)when(stoppingToken.IsCancellationRequested){break;}catch(Exception ex){logger.LogError(ex,"Booth session expiry scan failed.");}}
 }
}
