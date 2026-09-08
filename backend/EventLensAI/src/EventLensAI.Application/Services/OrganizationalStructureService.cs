using EventLensAI.Application.DTOs.Organizations;
using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Application.Services;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> ListAsync(Guid organizationId, CancellationToken ct);
    Task<DepartmentDto> CreateAsync(Guid organizationId, CreateDepartmentRequest request, CancellationToken ct);
    Task<DepartmentDto> UpdateAsync(Guid organizationId, Guid id, UpdateDepartmentRequest request, CancellationToken ct);
    Task ArchiveAsync(Guid organizationId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<UnitMemberDto>> ListMembersAsync(Guid departmentId, CancellationToken ct);
    Task AddMemberAsync(Guid departmentId, AssignUnitMemberRequest request, CancellationToken ct);
    Task RemoveMemberAsync(Guid departmentId, Guid userId, CancellationToken ct);
}
public interface IBranchService
{
    Task<IReadOnlyList<BranchDto>> ListAsync(Guid organizationId, CancellationToken ct);
    Task<BranchDto> CreateAsync(Guid organizationId, CreateBranchRequest request, CancellationToken ct);
    Task<BranchDto> UpdateAsync(Guid organizationId, Guid id, UpdateBranchRequest request, CancellationToken ct);
    Task ArchiveAsync(Guid organizationId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<UnitMemberDto>> ListMembersAsync(Guid branchId, CancellationToken ct);
    Task AddMemberAsync(Guid branchId, AssignUnitMemberRequest request, CancellationToken ct);
    Task RemoveMemberAsync(Guid branchId, Guid userId, CancellationToken ct);
}

public sealed class DepartmentService(IOrganizationalStructureRepository structures, IOrganizationRepository organizations,
    IAuditRepository audits, ICurrentUserService current, IUnitOfWork uow) : IDepartmentService
{
    public async Task<IReadOnlyList<DepartmentDto>> ListAsync(Guid org, CancellationToken ct)
    { await RequireMember(org, ct); return (await structures.ListDepartmentsAsync(org, ct)).Select(Map).ToArray(); }
    public async Task<DepartmentDto> CreateAsync(Guid org, CreateDepartmentRequest r, CancellationToken ct)
    {
        await RequireManager(org, ct); await ValidateUser(org, r.HeadUserId, ct);
        if (await structures.DepartmentNameExistsAsync(org, r.Name.Trim(), null, ct)) throw new ConflictException("Department name already exists.");
        var entity = new Department(org, r.Name, r.Description, r.HeadUserId);
        await structures.AddDepartmentAsync(entity, ct); await Audit(entity.Id, AuditAction.Create, ct); await uow.SaveChangesAsync(ct);
        return Map(entity);
    }
    public async Task<DepartmentDto> UpdateAsync(Guid org, Guid id, UpdateDepartmentRequest r, CancellationToken ct)
    {
        await RequireManager(org, ct); var entity = await Get(org, id, ct); await ValidateUser(org, r.HeadUserId, ct);
        if (await structures.DepartmentNameExistsAsync(org, r.Name.Trim(), id, ct)) throw new ConflictException("Department name already exists.");
        entity.Update(r.Name, r.Description, r.HeadUserId, r.Status); entity.MarkUpdated(UserId);
        await Audit(id, AuditAction.Update, ct); await uow.SaveChangesAsync(ct); return Map(entity);
    }
    public async Task ArchiveAsync(Guid org, Guid id, CancellationToken ct)
    { await RequireManager(org, ct); var entity = await Get(org, id, ct); entity.Archive(UserId); await Audit(id, AuditAction.Archive, ct); await uow.SaveChangesAsync(ct); }
    public async Task<IReadOnlyList<UnitMemberDto>> ListMembersAsync(Guid id, CancellationToken ct)
    {
        var entity = await structures.GetDepartmentAsync(id, ct) ?? throw new NotFoundException("Department not found.");
        await RequireMember(entity.OrganizationId, ct); return await MapMembers(entity.OrganizationId, await structures.ListDepartmentMembersAsync(id, ct), ct);
    }
    public async Task AddMemberAsync(Guid id, AssignUnitMemberRequest r, CancellationToken ct)
    {
        var entity = await structures.GetDepartmentAsync(id, ct) ?? throw new NotFoundException("Department not found.");
        await RequireManager(entity.OrganizationId, ct); if (entity.Status != OrganizationalUnitStatus.Active) throw new ConflictException("Members cannot be assigned to an inactive or archived department.");
        await ValidateUser(entity.OrganizationId, r.UserId, ct);
        if (await structures.GetDepartmentMemberAsync(id, r.UserId, ct) is not null) throw new ConflictException("Member is already assigned.");
        await structures.AddDepartmentMemberAsync(new DepartmentMember(id, r.UserId), ct); await Audit(id, AuditAction.Update, ct); await uow.SaveChangesAsync(ct);
    }
    public async Task RemoveMemberAsync(Guid id, Guid userId, CancellationToken ct)
    {
        var entity = await structures.GetDepartmentAsync(id, ct) ?? throw new NotFoundException("Department not found.");
        await RequireManager(entity.OrganizationId, ct); var assignment = await structures.GetDepartmentMemberAsync(id, userId, ct) ?? throw new NotFoundException("Department member not found.");
        assignment.SoftDelete(UserId); await Audit(id, AuditAction.RemoveMember, ct); await uow.SaveChangesAsync(ct);
    }
    private async Task<Department> Get(Guid org, Guid id, CancellationToken ct)
    { var x = await structures.GetDepartmentAsync(id, ct); return x is not null && x.OrganizationId == org ? x : throw new NotFoundException("Department not found."); }
    private async Task ValidateUser(Guid org, Guid? user, CancellationToken ct) { if (user.HasValue && await organizations.GetMemberAsync(org, user.Value, ct) is null) throw new ConflictException("User must belong to this organization."); }
    private async Task RequireMember(Guid org, CancellationToken ct) { if (current.Roles.Contains(SystemRoles.SuperAdmin)) return; if (await organizations.GetMemberAsync(org, UserId, ct) is null) throw new UnauthorizedException("Organization access denied."); }
    private async Task RequireManager(Guid org, CancellationToken ct) { if (current.Roles.Contains(SystemRoles.SuperAdmin)) return; var m = await organizations.GetMemberAsync(org, UserId, ct) ?? throw new UnauthorizedException("Organization access denied."); if (m.Role.Name is not (SystemRoles.Owner or SystemRoles.Manager)) throw new UnauthorizedException("Owner or Manager permission required."); }
    private async Task<IReadOnlyList<UnitMemberDto>> MapMembers(Guid org, IReadOnlyList<DepartmentMember> rows, CancellationToken ct) { var result=new List<UnitMemberDto>(); foreach(var x in rows){var m=await organizations.GetMemberAsync(org,x.UserId,ct);if(m is not null)result.Add(new(x.UserId,$"{x.User.FirstName} {x.User.LastName}",x.User.Email,m.Role.Name,x.CreatedAt));}return result; }
    private Guid UserId => current.UserId ?? throw new UnauthorizedException("Authentication required.");
    private Task Audit(Guid id, AuditAction action, CancellationToken ct) => audits.AddAsync(new AuditLog(UserId, nameof(Department), id, action, current.IPAddress), ct);
    private static DepartmentDto Map(Department x) => new(x.Id,x.OrganizationId,x.Name,x.Description,x.HeadUserId,x.HeadUser is null?null:$"{x.HeadUser.FirstName} {x.HeadUser.LastName}",x.Members.Count,x.Status,x.CreatedAt);
}

public sealed class BranchService(IOrganizationalStructureRepository structures, IOrganizationRepository organizations,
    IAuditRepository audits, ICurrentUserService current, IUnitOfWork uow) : IBranchService
{
    public async Task<IReadOnlyList<BranchDto>> ListAsync(Guid org,CancellationToken ct){await Member(org,ct);return (await structures.ListBranchesAsync(org,ct)).Select(Map).ToArray();}
    public async Task<BranchDto> CreateAsync(Guid org,CreateBranchRequest r,CancellationToken ct){await Manager(org,ct);await ValidUser(org,r.ManagerUserId,ct);if(await structures.BranchCodeExistsAsync(org,r.Code.Trim().ToUpperInvariant(),null,ct))throw new ConflictException("Branch code already exists.");var x=new Branch(org,r.Name,r.Code);x.Update(r.Name,r.Code,r.Address,r.City,r.Country,r.Timezone,r.Phone,r.Email,r.ManagerUserId,OrganizationalUnitStatus.Active);await structures.AddBranchAsync(x,ct);await Audit(x.Id,AuditAction.Create,ct);await uow.SaveChangesAsync(ct);return Map(x);}
    public async Task<BranchDto> UpdateAsync(Guid org,Guid id,UpdateBranchRequest r,CancellationToken ct){await Manager(org,ct);var x=await Scoped(org,id,ct);await ValidUser(org,r.ManagerUserId,ct);if(await structures.BranchCodeExistsAsync(org,r.Code.Trim().ToUpperInvariant(),id,ct))throw new ConflictException("Branch code already exists.");x.Update(r.Name,r.Code,r.Address,r.City,r.Country,r.Timezone,r.Phone,r.Email,r.ManagerUserId,r.Status);x.MarkUpdated(UserId);await Audit(id,AuditAction.Update,ct);await uow.SaveChangesAsync(ct);return Map(x);}
    public async Task ArchiveAsync(Guid org,Guid id,CancellationToken ct){await Manager(org,ct);var x=await Scoped(org,id,ct);x.Archive(UserId);await Audit(id,AuditAction.Archive,ct);await uow.SaveChangesAsync(ct);}
    public async Task<IReadOnlyList<UnitMemberDto>> ListMembersAsync(Guid id,CancellationToken ct){var x=await structures.GetBranchAsync(id,ct)??throw new NotFoundException("Branch not found.");await Member(x.OrganizationId,ct);var rows=await structures.ListBranchMembersAsync(id,ct);var result=new List<UnitMemberDto>();foreach(var row in rows){var m=await organizations.GetMemberAsync(x.OrganizationId,row.UserId,ct);if(m is not null)result.Add(new(row.UserId,$"{row.User.FirstName} {row.User.LastName}",row.User.Email,m.Role.Name,row.CreatedAt));}return result;}
    public async Task AddMemberAsync(Guid id,AssignUnitMemberRequest r,CancellationToken ct){var x=await structures.GetBranchAsync(id,ct)??throw new NotFoundException("Branch not found.");await Manager(x.OrganizationId,ct);if(x.Status!=OrganizationalUnitStatus.Active)throw new ConflictException("Members cannot be assigned to an inactive or archived branch.");await ValidUser(x.OrganizationId,r.UserId,ct);if(await structures.GetBranchMemberAsync(id,r.UserId,ct)is not null)throw new ConflictException("Member is already assigned.");await structures.AddBranchMemberAsync(new BranchMember(id,r.UserId),ct);await Audit(id,AuditAction.Update,ct);await uow.SaveChangesAsync(ct);}
    public async Task RemoveMemberAsync(Guid id,Guid userId,CancellationToken ct){var x=await structures.GetBranchAsync(id,ct)??throw new NotFoundException("Branch not found.");await Manager(x.OrganizationId,ct);var row=await structures.GetBranchMemberAsync(id,userId,ct)??throw new NotFoundException("Branch member not found.");row.SoftDelete(UserId);await Audit(id,AuditAction.RemoveMember,ct);await uow.SaveChangesAsync(ct);}
    private async Task<Branch> Scoped(Guid org,Guid id,CancellationToken ct){var x=await structures.GetBranchAsync(id,ct);return x is not null&&x.OrganizationId==org?x:throw new NotFoundException("Branch not found.");}
    private async Task ValidUser(Guid org,Guid? user,CancellationToken ct){if(user.HasValue&&await organizations.GetMemberAsync(org,user.Value,ct)is null)throw new ConflictException("User must belong to this organization.");}
    private async Task Member(Guid org,CancellationToken ct){if(current.Roles.Contains(SystemRoles.SuperAdmin))return;if(await organizations.GetMemberAsync(org,UserId,ct)is null)throw new UnauthorizedException("Organization access denied.");}
    private async Task Manager(Guid org,CancellationToken ct){if(current.Roles.Contains(SystemRoles.SuperAdmin))return;var m=await organizations.GetMemberAsync(org,UserId,ct)??throw new UnauthorizedException("Organization access denied.");if(m.Role.Name is not(SystemRoles.Owner or SystemRoles.Manager))throw new UnauthorizedException("Owner or Manager permission required.");}
    private Guid UserId=>current.UserId??throw new UnauthorizedException("Authentication required.");
    private Task Audit(Guid id,AuditAction action,CancellationToken ct)=>audits.AddAsync(new AuditLog(UserId,nameof(Branch),id,action,current.IPAddress),ct);
    private static BranchDto Map(Branch x)=>new(x.Id,x.OrganizationId,x.Name,x.Code,x.Address,x.City,x.Country,x.Timezone,x.Phone,x.Email,x.ManagerUserId,x.ManagerUser is null?null:$"{x.ManagerUser.FirstName} {x.ManagerUser.LastName}",x.Members.Count,x.Status,x.CreatedAt);
}
