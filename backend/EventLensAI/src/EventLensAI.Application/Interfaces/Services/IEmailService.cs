namespace EventLensAI.Application.Interfaces.Services;
public interface IEmailService
{
    Task SendOrganizationInvitationAsync(string email,string organizationName,string inviterName,string acceptUrl,CancellationToken ct);
}
