namespace EventLensAI.Application.Interfaces.Services;
public interface IEmailService
{
    Task SendOrganizationInvitationAsync(string email,string organizationName,string inviterName,string acceptUrl,CancellationToken ct);
    Task SendEmailVerificationAsync(string email, string verificationUrl, CancellationToken ct);
    Task SendPasswordResetAsync(string email, string resetUrl, CancellationToken ct);
}
