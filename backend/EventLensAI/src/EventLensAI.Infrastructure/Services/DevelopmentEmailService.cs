using EventLensAI.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
namespace EventLensAI.Infrastructure.Services;
internal sealed class DevelopmentEmailService(ILogger<DevelopmentEmailService> logger):IEmailService
{
    public Task SendOrganizationInvitationAsync(string email,string organizationName,string inviterName,string acceptUrl,CancellationToken ct)
    {
        logger.LogInformation("Development invitation email queued for {Email} to organization {OrganizationName}",email,organizationName);
        return Task.CompletedTask;
    }
}
