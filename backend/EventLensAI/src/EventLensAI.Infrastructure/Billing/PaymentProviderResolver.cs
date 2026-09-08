using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Features.Billing;
namespace EventLensAI.Infrastructure.Billing;
public sealed class PaymentProviderResolver(IEnumerable<IPaymentProvider> providers):IPaymentProviderResolver
{
 readonly Dictionary<string,IPaymentProvider> items=providers.ToDictionary(x=>x.Name,StringComparer.OrdinalIgnoreCase);
 public IReadOnlyList<string>AvailableProviders=>items.Keys.Order().ToList();
 public IPaymentProvider Resolve(string name)=>items.TryGetValue(name,out var provider)?provider:throw new ConflictException($"Payment provider '{name}' is not configured.");
}
