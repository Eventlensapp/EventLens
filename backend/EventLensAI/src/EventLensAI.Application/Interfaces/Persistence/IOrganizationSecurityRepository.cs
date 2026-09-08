using EventLensAI.Domain.Entities;
namespace EventLensAI.Application.Interfaces.Persistence;
public interface IOrganizationSecurityRepository
{
 Task<IReadOnlyList<Permission>> ListPermissionsAsync(CancellationToken ct);
 Task<IReadOnlyList<RolePermission>> ListRolePermissionsAsync(Guid roleId,CancellationToken ct);
 Task<IReadOnlyList<OrganizationMemberPermission>> ListMemberPermissionsAsync(Guid memberId,CancellationToken ct);
 Task<Permission?> GetPermissionAsync(string key,CancellationToken ct);
 Task<OrganizationMemberPermission?> GetMemberPermissionAsync(Guid memberId,Guid permissionId,CancellationToken ct);
 Task AddMemberPermissionAsync(OrganizationMemberPermission value,CancellationToken ct);
 Task<OrganizationOwnershipTransfer?> GetPendingTransferAsync(Guid organizationId,CancellationToken ct);
 Task<OrganizationOwnershipTransfer?> GetTransferAsync(Guid id,CancellationToken ct);
 Task AddTransferAsync(OrganizationOwnershipTransfer transfer,CancellationToken ct);
}
