using EventLensAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EventLensAI.Infrastructure.Persistence.Configurations;
internal sealed class PermissionConfiguration:IEntityTypeConfiguration<Permission>
{
 public void Configure(EntityTypeBuilder<Permission>b){b.ToTable("permissions");b.ConfigureBase();b.Property(x=>x.Key).HasMaxLength(100).IsRequired();b.Property(x=>x.Description).HasMaxLength(300);b.HasIndex(x=>x.Key).IsUnique();b.HasData(SystemPermissions.All.Select(x=>new{Id=x.Id,Key=x.Key,Description=x.Description,CreatedAt=new DateTime(2026,7,30,0,0,0,DateTimeKind.Utc),UpdatedAt=(DateTime?)null,CreatedBy=(Guid?)null,UpdatedBy=(Guid?)null,IsDeleted=false}));}
}
internal sealed class RolePermissionConfiguration:IEntityTypeConfiguration<RolePermission>
{
 public void Configure(EntityTypeBuilder<RolePermission>b){b.ToTable("role_permissions");b.ConfigureBase();b.HasIndex(x=>new{x.RoleId,x.PermissionId}).IsUnique();b.HasOne(x=>x.Role).WithMany().HasForeignKey(x=>x.RoleId).OnDelete(DeleteBehavior.Cascade);b.HasOne(x=>x.Permission).WithMany().HasForeignKey(x=>x.PermissionId).OnDelete(DeleteBehavior.Cascade);var rows=new List<object>();var sequence=1;Add(rows,SystemRoles.OwnerId,Enumerable.Range(0,8),ref sequence);Add(rows,SystemRoles.ManagerId,[1,2,3,4,5,6,7],ref sequence);Add(rows,SystemRoles.PhotographerId,[3,7],ref sequence);Add(rows,SystemRoles.BoothOperatorId,[3,7],ref sequence);Add(rows,SystemRoles.DesignerId,[4,7],ref sequence);Add(rows,SystemRoles.MarketingManagerId,[5,6,7],ref sequence);Add(rows,SystemRoles.ViewerId,[6,7],ref sequence);b.HasData(rows);}
 private static void Add(List<object>rows,Guid role,IEnumerable<int>indexes,ref int sequence){foreach(var i in indexes){var permission=SystemPermissions.All[i].Id;rows.Add(new{Id=Guid.Parse($"40000000-0000-0000-0000-{sequence++:000000000000}"),RoleId=role,PermissionId=permission,CreatedAt=new DateTime(2026,7,30,0,0,0,DateTimeKind.Utc),UpdatedAt=(DateTime?)null,CreatedBy=(Guid?)null,UpdatedBy=(Guid?)null,IsDeleted=false});}}
}
internal sealed class MemberPermissionConfiguration:IEntityTypeConfiguration<OrganizationMemberPermission>
{
 public void Configure(EntityTypeBuilder<OrganizationMemberPermission>b){b.ToTable("organization_member_permissions");b.ConfigureBase();b.HasIndex(x=>new{x.OrganizationMemberId,x.PermissionId}).IsUnique();b.HasOne(x=>x.Member).WithMany().HasForeignKey(x=>x.OrganizationMemberId).OnDelete(DeleteBehavior.Cascade);b.HasOne(x=>x.Permission).WithMany().HasForeignKey(x=>x.PermissionId).OnDelete(DeleteBehavior.Cascade);}
}
internal sealed class OwnershipTransferConfiguration:IEntityTypeConfiguration<OrganizationOwnershipTransfer>
{
 public void Configure(EntityTypeBuilder<OrganizationOwnershipTransfer>b){b.ToTable("organization_ownership_transfers");b.ConfigureBase();b.Property(x=>x.Status).HasConversion<string>().HasMaxLength(20);b.HasIndex(x=>new{x.OrganizationId,x.Status});b.HasOne(x=>x.Organization).WithMany().HasForeignKey(x=>x.OrganizationId).OnDelete(DeleteBehavior.Cascade);b.Ignore(x=>x.IsValid);}
}
