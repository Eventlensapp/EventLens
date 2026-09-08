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
    public Task SendEmailVerificationAsync(string email,string verificationUrl,CancellationToken ct)
    { logger.LogInformation("Development verification email queued for {Email}: {VerificationUrl}",email,verificationUrl); return Task.CompletedTask; }
    public Task SendPasswordResetAsync(string email,string resetUrl,CancellationToken ct)
    { logger.LogInformation("Development password reset email queued for {Email}: {ResetUrl}",email,resetUrl); return Task.CompletedTask; }
}
