using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;

public sealed class Department : BaseEntity
{
    private Department() { }
    public Department(Guid organizationId, string name, string? description, Guid? headUserId)
    {
        OrganizationId = organizationId;
        Update(name, description, headUserId, OrganizationalUnitStatus.Active);
    }
    public Guid OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid? HeadUserId { get; private set; }
    public User? HeadUser { get; private set; }
    public OrganizationalUnitStatus Status { get; private set; } = OrganizationalUnitStatus.Active;
    public ICollection<DepartmentMember> Members { get; private set; } = [];
    public void Update(string name, string? description, Guid? headUserId, OrganizationalUnitStatus status)
    {
        if (status == OrganizationalUnitStatus.Archived) throw new InvalidOperationException("Use Archive to archive a department.");
        Name = name.Trim(); Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        HeadUserId = headUserId; Status = status;
    }
    public void Archive(Guid actorId) { Status = OrganizationalUnitStatus.Archived; SoftDelete(actorId); }
}

public sealed class Branch : BaseEntity
{
    private Branch() { }
    public Branch(Guid organizationId, string name, string code) { OrganizationId = organizationId; Name = name.Trim(); Code = code.Trim().ToUpperInvariant(); }
    public Guid OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? Country { get; private set; }
    public string Timezone { get; private set; } = "UTC";
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public Guid? ManagerUserId { get; private set; }
    public User? ManagerUser { get; private set; }
    public OrganizationalUnitStatus Status { get; private set; } = OrganizationalUnitStatus.Active;
    public ICollection<BranchMember> Members { get; private set; } = [];
    public void Update(string name, string code, string? address, string? city, string? country, string timezone,
        string? phone, string? email, Guid? managerUserId, OrganizationalUnitStatus status)
    {
        if (status == OrganizationalUnitStatus.Archived) throw new InvalidOperationException("Use Archive to archive a branch.");
        Name = name.Trim(); Code = code.Trim().ToUpperInvariant(); Address = Clean(address); City = Clean(city);
        Country = Clean(country); Timezone = timezone.Trim(); Phone = Clean(phone); Email = Clean(email);
        ManagerUserId = managerUserId; Status = status;
    }
    public void Archive(Guid actorId) { Status = OrganizationalUnitStatus.Archived; SoftDelete(actorId); }
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class DepartmentMember : BaseEntity
{
    private DepartmentMember() { }
    public DepartmentMember(Guid departmentId, Guid userId) { DepartmentId = departmentId; UserId = userId; }
    public Guid DepartmentId { get; private set; }
    public Department Department { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
}

public sealed class BranchMember : BaseEntity
{
    private BranchMember() { }
    public BranchMember(Guid branchId, Guid userId) { BranchId = branchId; UserId = userId; }
    public Guid BranchId { get; private set; }
    public Branch Branch { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
}
