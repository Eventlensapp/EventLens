using System.Net.Http.Json;
using EventLensAI.Application.Features.AI;
using EventLensAI.Domain.Enums;
namespace EventLensAI.Infrastructure.AI;
public sealed record AIProviderOptions(string Name,string Endpoint,string? ApiKey,IReadOnlySet<AIJobType> SupportedJobTypes,int TimeoutSeconds=120);
internal sealed class HttpAIProvider(AIProviderOptions options,HttpClient client):IAIProvider
{
 public string Name=>options.Name;public IReadOnlySet<AIJobType> SupportedJobTypes=>options.SupportedJobTypes;
 public async Task<AIProviderResult> ProcessAsync(AIProviderRequest request,IProgress<int> progress,CancellationToken ct)
 {
  progress.Report(10);using var message=new HttpRequestMessage(HttpMethod.Post,options.Endpoint){Content=JsonContent.Create(request)};
  if(!string.IsNullOrWhiteSpace(options.ApiKey))message.Headers.Authorization=new("Bearer",options.ApiKey);
  using var timeout=CancellationTokenSource.CreateLinkedTokenSource(ct);timeout.CancelAfter(TimeSpan.FromSeconds(options.TimeoutSeconds));
  using var response=await client.SendAsync(message,HttpCompletionOption.ResponseHeadersRead,timeout.Token);response.EnsureSuccessStatusCode();progress.Report(90);
  return await response.Content.ReadFromJsonAsync<AIProviderResult>(cancellationToken:timeout.Token)??throw new InvalidOperationException("Provider returned an empty response.");
 }
}
internal sealed class AIProviderResolver(IEnumerable<IAIProvider> providers):IAIProviderResolver
{
 public IAIProvider Resolve(string? name,AIJobType type){var provider=providers.FirstOrDefault(x=>x.Name.Equals(name,StringComparison.OrdinalIgnoreCase))??throw new InvalidOperationException($"AI provider '{name}' is unavailable.");if(!provider.SupportedJobTypes.Contains(type))throw new InvalidOperationException($"Provider '{name}' does not support {type}.");return provider;}
}
