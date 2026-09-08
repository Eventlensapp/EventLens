using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;

public sealed class OrganizationInvitation : BaseEntity
{
    private OrganizationInvitation() { }
    public OrganizationInvitation(Guid organizationId, string email, Guid roleId, string tokenHash, Guid invitedBy)
    {
        OrganizationId = organizationId; Email = email.Trim().ToLowerInvariant(); RoleId = roleId;
        TokenHash = tokenHash; InvitedBy = invitedBy; ExpiresAt = DateTime.UtcNow.AddDays(7);
    }
    public Guid OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public string Email { get; private set; } = string.Empty;
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = null!;
    public string TokenHash { get; private set; } = string.Empty;
    public Guid InvitedBy { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public OrganizationInvitationStatus Status { get; private set; } = OrganizationInvitationStatus.Pending;
    public bool IsValid => Status == OrganizationInvitationStatus.Pending && ExpiresAt > DateTime.UtcNow && !IsDeleted;
    public void Accept() { if (!IsValid) throw new InvalidOperationException("Invitation is no longer valid."); AcceptedAt = DateTime.UtcNow; Status = OrganizationInvitationStatus.Accepted; }
    public void Cancel() { if (Status != OrganizationInvitationStatus.Pending) throw new InvalidOperationException("Only pending invitations can be cancelled."); Status = OrganizationInvitationStatus.Cancelled; }
}
