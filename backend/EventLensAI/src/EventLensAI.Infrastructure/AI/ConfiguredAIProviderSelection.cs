using EventLensAI.Application.Features.AI;
using EventLensAI.Domain.Enums;
using Microsoft.Extensions.Configuration;
namespace EventLensAI.Infrastructure.AI;
internal sealed class ConfiguredAIProviderSelection(IConfiguration configuration):IAIProviderSelection
{
 public string Select(AIJobType type)=>configuration[$"AI:Routing:{type}"]??configuration["AI:DefaultProvider"]??throw new InvalidOperationException("No AI provider is configured.");
}
