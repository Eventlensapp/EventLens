using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Entities;
using EventLensAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventLensAI.Infrastructure.Repositories;

internal sealed class OrganizationalStructureRepository(EventLensDbContext db) : IOrganizationalStructureRepository
{
    public async Task<IReadOnlyList<Department>> ListDepartmentsAsync(Guid organizationId, CancellationToken ct) =>
        await db.Departments.AsNoTracking().Include(x => x.HeadUser).Include(x => x.Members)
            .Where(x => x.OrganizationId == organizationId).OrderBy(x => x.Name).ToListAsync(ct);
    public Task<Department?> GetDepartmentAsync(Guid id, CancellationToken ct) =>
        db.Departments.Include(x => x.HeadUser).Include(x => x.Members).FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task<bool> DepartmentNameExistsAsync(Guid organizationId, string name, Guid? exceptId, CancellationToken ct) =>
        db.Departments.AnyAsync(x => x.OrganizationId == organizationId && x.Name == name && (!exceptId.HasValue || x.Id != exceptId), ct);
    public Task AddDepartmentAsync(Department entity, CancellationToken ct) => db.Departments.AddAsync(entity, ct).AsTask();
    public async Task<IReadOnlyList<DepartmentMember>> ListDepartmentMembersAsync(Guid id, CancellationToken ct) =>
        await db.DepartmentMembers.AsNoTracking().Include(x => x.User)
            .Where(x => x.DepartmentId == id).OrderBy(x => x.User.FirstName).ToListAsync(ct);
    public Task<DepartmentMember?> GetDepartmentMemberAsync(Guid id, Guid userId, CancellationToken ct) =>
        db.DepartmentMembers.FirstOrDefaultAsync(x => x.DepartmentId == id && x.UserId == userId, ct);
    public Task AddDepartmentMemberAsync(DepartmentMember entity, CancellationToken ct) => db.DepartmentMembers.AddAsync(entity, ct).AsTask();
    public async Task<IReadOnlyList<Branch>> ListBranchesAsync(Guid organizationId, CancellationToken ct) =>
        await db.Branches.AsNoTracking().Include(x => x.ManagerUser).Include(x => x.Members)
            .Where(x => x.OrganizationId == organizationId).OrderBy(x => x.Name).ToListAsync(ct);
    public Task<Branch?> GetBranchAsync(Guid id, CancellationToken ct) =>
        db.Branches.Include(x => x.ManagerUser).Include(x => x.Members).FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task<bool> BranchCodeExistsAsync(Guid organizationId, string code, Guid? exceptId, CancellationToken ct) =>
        db.Branches.AnyAsync(x => x.OrganizationId == organizationId && x.Code == code && (!exceptId.HasValue || x.Id != exceptId), ct);
    public Task AddBranchAsync(Branch entity, CancellationToken ct) => db.Branches.AddAsync(entity, ct).AsTask();
    public async Task<IReadOnlyList<BranchMember>> ListBranchMembersAsync(Guid id, CancellationToken ct) =>
        await db.BranchMembers.AsNoTracking().Include(x => x.User)
            .Where(x => x.BranchId == id).OrderBy(x => x.User.FirstName).ToListAsync(ct);
    public Task<BranchMember?> GetBranchMemberAsync(Guid id, Guid userId, CancellationToken ct) =>
        db.BranchMembers.FirstOrDefaultAsync(x => x.BranchId == id && x.UserId == userId, ct);
    public Task AddBranchMemberAsync(BranchMember entity, CancellationToken ct) => db.BranchMembers.AddAsync(entity, ct).AsTask();
}
