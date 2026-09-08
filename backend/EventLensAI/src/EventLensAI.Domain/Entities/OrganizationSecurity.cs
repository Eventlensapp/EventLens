using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;
namespace EventLensAI.Domain.Entities;

public sealed class Permission:BaseEntity
{
    private Permission(){} public Permission(Guid id,string key,string description){Id=id;Key=key;Description=description;}
    public string Key{get;private set;}=string.Empty;public string Description{get;private set;}=string.Empty;
}
public sealed class RolePermission:BaseEntity
{
    private RolePermission(){} public RolePermission(Guid id,Guid roleId,Guid permissionId){Id=id;RoleId=roleId;PermissionId=permissionId;}
    public Guid RoleId{get;private set;}public Role Role{get;private set;}=null!;public Guid PermissionId{get;private set;}public Permission Permission{get;private set;}=null!;
}
public sealed class OrganizationMemberPermission:BaseEntity
{
    private OrganizationMemberPermission(){} public OrganizationMemberPermission(Guid organizationMemberId,Guid permissionId,bool allowed){OrganizationMemberId=organizationMemberId;PermissionId=permissionId;Allowed=allowed;}
    public Guid OrganizationMemberId{get;private set;}public OrganizationMember Member{get;private set;}=null!;
    public Guid PermissionId{get;private set;}public Permission Permission{get;private set;}=null!;public bool Allowed{get;private set;}
    public void Set(bool allowed)=>Allowed=allowed;
}
public sealed class OrganizationOwnershipTransfer:BaseEntity
{
    private OrganizationOwnershipTransfer(){} public OrganizationOwnershipTransfer(Guid organizationId,Guid fromUserId,Guid toUserId,DateTime expiresAt){OrganizationId=organizationId;FromUserId=fromUserId;ToUserId=toUserId;ExpiresAt=expiresAt;}
    public Guid OrganizationId{get;private set;}public Organization Organization{get;private set;}=null!;
    public Guid FromUserId{get;private set;}public Guid ToUserId{get;private set;}public DateTime ExpiresAt{get;private set;}
    public DateTime? AcceptedAt{get;private set;}public OwnershipTransferStatus Status{get;private set;}=OwnershipTransferStatus.Pending;
    public bool IsValid=>Status==OwnershipTransferStatus.Pending&&ExpiresAt>DateTime.UtcNow;
    public void Accept(){if(!IsValid)throw new InvalidOperationException("Transfer is no longer valid.");Status=OwnershipTransferStatus.Accepted;AcceptedAt=DateTime.UtcNow;}
    public void Cancel(){if(Status!=OwnershipTransferStatus.Pending)throw new InvalidOperationException("Only pending transfers can be cancelled.");Status=OwnershipTransferStatus.Cancelled;}
}
public static class SystemPermissions
{
    public static readonly (Guid Id,string Key,string Description)[] All=[
        (Guid.Parse("30000000-0000-0000-0000-000000000001"),"organization.manage","Manage organization settings"),
        (Guid.Parse("30000000-0000-0000-0000-000000000002"),"organization.members.manage","Manage members and invitations"),
        (Guid.Parse("30000000-0000-0000-0000-000000000003"),"events.manage","Create and manage events"),
        (Guid.Parse("30000000-0000-0000-0000-000000000004"),"photos.manage","Capture and manage photos"),
        (Guid.Parse("30000000-0000-0000-0000-000000000005"),"templates.manage","Manage templates"),
        (Guid.Parse("30000000-0000-0000-0000-000000000006"),"crm.manage","Manage CRM and marketing"),
        (Guid.Parse("30000000-0000-0000-0000-000000000007"),"analytics.view","View analytics"),
        (Guid.Parse("30000000-0000-0000-0000-000000000008"),"organization.view","View organization resources")];
}
