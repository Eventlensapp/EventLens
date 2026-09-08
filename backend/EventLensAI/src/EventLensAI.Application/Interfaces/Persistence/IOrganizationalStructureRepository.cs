using EventLensAI.Domain.Entities;

namespace EventLensAI.Application.Interfaces.Persistence;

public interface IOrganizationalStructureRepository
{
    Task<IReadOnlyList<Department>> ListDepartmentsAsync(Guid organizationId, CancellationToken ct);
    Task<Department?> GetDepartmentAsync(Guid id, CancellationToken ct);
    Task<bool> DepartmentNameExistsAsync(Guid organizationId, string name, Guid? exceptId, CancellationToken ct);
    Task AddDepartmentAsync(Department entity, CancellationToken ct);
    Task<IReadOnlyList<DepartmentMember>> ListDepartmentMembersAsync(Guid departmentId, CancellationToken ct);
    Task<DepartmentMember?> GetDepartmentMemberAsync(Guid departmentId, Guid userId, CancellationToken ct);
    Task AddDepartmentMemberAsync(DepartmentMember entity, CancellationToken ct);
    Task<IReadOnlyList<Branch>> ListBranchesAsync(Guid organizationId, CancellationToken ct);
    Task<Branch?> GetBranchAsync(Guid id, CancellationToken ct);
    Task<bool> BranchCodeExistsAsync(Guid organizationId, string code, Guid? exceptId, CancellationToken ct);
    Task AddBranchAsync(Branch entity, CancellationToken ct);
    Task<IReadOnlyList<BranchMember>> ListBranchMembersAsync(Guid branchId, CancellationToken ct);
    Task<BranchMember?> GetBranchMemberAsync(Guid branchId, Guid userId, CancellationToken ct);
    Task AddBranchMemberAsync(BranchMember entity, CancellationToken ct);
}
