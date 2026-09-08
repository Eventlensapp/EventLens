using EventLensAI.Application.Interfaces.Persistence;using EventLensAI.Domain.Entities;using EventLensAI.Domain.Enums;using EventLensAI.Infrastructure.Persistence;using Microsoft.EntityFrameworkCore;
namespace EventLensAI.Infrastructure.Repositories;
internal sealed class OrganizationSecurityRepository(EventLensDbContext db):IOrganizationSecurityRepository
{
 public async Task<IReadOnlyList<Permission>>ListPermissionsAsync(CancellationToken ct)=>await db.Permissions.AsNoTracking().OrderBy(x=>x.Key).ToListAsync(ct);
 public async Task<IReadOnlyList<RolePermission>>ListRolePermissionsAsync(Guid id,CancellationToken ct)=>await db.RolePermissions.AsNoTracking().Where(x=>x.RoleId==id).ToListAsync(ct);
 public async Task<IReadOnlyList<OrganizationMemberPermission>>ListMemberPermissionsAsync(Guid id,CancellationToken ct)=>await db.OrganizationMemberPermissions.AsNoTracking().Where(x=>x.OrganizationMemberId==id).ToListAsync(ct);
 public Task<Permission?>GetPermissionAsync(string key,CancellationToken ct)=>db.Permissions.FirstOrDefaultAsync(x=>x.Key==key,ct);
 public Task<OrganizationMemberPermission?>GetMemberPermissionAsync(Guid m,Guid p,CancellationToken ct)=>db.OrganizationMemberPermissions.FirstOrDefaultAsync(x=>x.OrganizationMemberId==m&&x.PermissionId==p,ct);
 public Task AddMemberPermissionAsync(OrganizationMemberPermission x,CancellationToken ct)=>db.OrganizationMemberPermissions.AddAsync(x,ct).AsTask();
 public Task<OrganizationOwnershipTransfer?>GetPendingTransferAsync(Guid o,CancellationToken ct)=>db.OrganizationOwnershipTransfers.FirstOrDefaultAsync(x=>x.OrganizationId==o&&x.Status==OwnershipTransferStatus.Pending&&x.ExpiresAt>DateTime.UtcNow,ct);
 public Task<OrganizationOwnershipTransfer?>GetTransferAsync(Guid id,CancellationToken ct)=>db.OrganizationOwnershipTransfers.FirstOrDefaultAsync(x=>x.Id==id,ct);
 public Task AddTransferAsync(OrganizationOwnershipTransfer x,CancellationToken ct)=>db.OrganizationOwnershipTransfers.AddAsync(x,ct).AsTask();
}
