using EventLensAI.Domain.Enums;

namespace EventLensAI.Application.DTOs.Organizations;

public sealed record DepartmentDto(Guid Id, Guid OrganizationId, string Name, string? Description,
    Guid? HeadUserId, string? HeadName, int MemberCount, OrganizationalUnitStatus Status, DateTime CreatedAt);
public sealed record CreateDepartmentRequest(string Name, string? Description, Guid? HeadUserId);
public sealed record UpdateDepartmentRequest(string Name, string? Description, Guid? HeadUserId, OrganizationalUnitStatus Status);
public sealed record BranchDto(Guid Id, Guid OrganizationId, string Name, string Code, string? Address,
    string? City, string? Country, string Timezone, string? Phone, string? Email, Guid? ManagerUserId,
    string? ManagerName, int MemberCount, OrganizationalUnitStatus Status, DateTime CreatedAt);
public sealed record CreateBranchRequest(string Name, string Code, string? Address, string? City, string? Country,
    string Timezone, string? Phone, string? Email, Guid? ManagerUserId);
public sealed record UpdateBranchRequest(string Name, string Code, string? Address, string? City, string? Country,
    string Timezone, string? Phone, string? Email, Guid? ManagerUserId, OrganizationalUnitStatus Status);
public sealed record AssignUnitMemberRequest(Guid UserId);
public sealed record UnitMemberDto(Guid UserId, string Name, string Email, string Role, DateTime AssignedAt);
