using EventLensAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventLensAI.Infrastructure.Persistence.Configurations;

internal sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> b)
    {
        b.ToTable("departments"); b.ConfigureBase();
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Description).HasMaxLength(2000);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        b.HasIndex(x => x.OrganizationId);
        b.HasIndex(x => new { x.OrganizationId, x.Name }).IsUnique().HasFilter("[IsDeleted] = 0");
        b.HasOne(x => x.Organization).WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.NoAction);
        b.HasOne(x => x.HeadUser).WithMany().HasForeignKey(x => x.HeadUserId).OnDelete(DeleteBehavior.NoAction);
    }
}
internal sealed class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> b)
    {
        b.ToTable("branches"); b.ConfigureBase();
        b.Property(x => x.Name).HasMaxLength(200).IsRequired(); b.Property(x => x.Code).HasMaxLength(50).IsRequired();
        b.Property(x => x.Address).HasMaxLength(500); b.Property(x => x.City).HasMaxLength(100);
        b.Property(x => x.Country).HasMaxLength(100); b.Property(x => x.Timezone).HasMaxLength(100).IsRequired();
        b.Property(x => x.Phone).HasMaxLength(50); b.Property(x => x.Email).HasMaxLength(320);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        b.HasIndex(x => x.OrganizationId); b.HasIndex(x => new { x.OrganizationId, x.Code }).IsUnique().HasFilter("[IsDeleted] = 0");
        b.HasOne(x => x.Organization).WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.NoAction);
        b.HasOne(x => x.ManagerUser).WithMany().HasForeignKey(x => x.ManagerUserId).OnDelete(DeleteBehavior.NoAction);
    }
}
internal sealed class DepartmentMemberConfiguration : IEntityTypeConfiguration<DepartmentMember>
{
    public void Configure(EntityTypeBuilder<DepartmentMember> b)
    {
        b.ToTable("department_members"); b.ConfigureBase();
        b.HasIndex(x => x.DepartmentId); b.HasIndex(x => x.UserId);
        b.HasIndex(x => new { x.DepartmentId, x.UserId }).IsUnique().HasFilter("[IsDeleted] = 0");
        b.HasOne(x => x.Department).WithMany(x => x.Members).HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.NoAction);
        b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
    }
}
internal sealed class BranchMemberConfiguration : IEntityTypeConfiguration<BranchMember>
{
    public void Configure(EntityTypeBuilder<BranchMember> b)
    {
        b.ToTable("branch_members"); b.ConfigureBase();
        b.HasIndex(x => x.BranchId); b.HasIndex(x => x.UserId);
        b.HasIndex(x => new { x.BranchId, x.UserId }).IsUnique().HasFilter("[IsDeleted] = 0");
        b.HasOne(x => x.Branch).WithMany(x => x.Members).HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.NoAction);
        b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
    }
}
