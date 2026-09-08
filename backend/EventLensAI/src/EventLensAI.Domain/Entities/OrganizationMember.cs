using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;

public sealed class OrganizationMember : BaseEntity
{
    private OrganizationMember() { }
    public OrganizationMember(Guid organizationId, Guid userId, Guid roleId, Guid? invitedBy = null)
    {
        OrganizationId = organizationId;
        UserId = userId;
        RoleId = roleId;
        JoinedAt = DateTime.UtcNow;
        InvitedBy = invitedBy;
    }
    public Guid OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = null!;
    public DateTime JoinedAt { get; private set; }
    public OrganizationMemberStatus Status { get; private set; } = OrganizationMemberStatus.Active;
    public Guid? InvitedBy { get; private set; }
    public void ChangeRole(Guid roleId) => RoleId = roleId;
}
