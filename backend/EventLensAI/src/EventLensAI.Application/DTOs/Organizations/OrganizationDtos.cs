using EventLensAI.Domain.Enums;

namespace EventLensAI.Application.DTOs.Organizations;

public sealed record CreateOrganizationRequest(
    string Name, string? Description, string? Logo, string? Website, string? Email,
    string? Phone, string? Address, string? Country, string TimeZone,
    string PrimaryColor, string SecondaryColor, OrganizationType OrganizationType = OrganizationType.Other);
public sealed record UpdateOrganizationRequest(
    string Name, string? Description, string? Logo, string? Website, string? Email,
    string? Phone, string? Address, string? Country, string TimeZone,
    string PrimaryColor, string SecondaryColor, OrganizationType OrganizationType = OrganizationType.Other);
public sealed record OrganizationDto(
    Guid Id, string Name, string Slug, string? Description, string? Logo, string? Website,
    string? Email, string? Phone, string? Address, string? Country, string TimeZone,
    string PrimaryColor, string SecondaryColor, SubscriptionPlan SubscriptionPlan,
    long StorageUsed, long StorageLimit, bool IsActive, DateTime CreatedAt, DateTime? UpdatedAt,
    OrganizationType OrganizationType, OrganizationStatus Status);
public sealed record OrganizationMemberDto(
    Guid Id, Guid UserId, string Name, string Email, string Role, DateTime JoinedAt,
    OrganizationMemberStatus Status, DateTime? LastActivity);
public sealed record InviteMemberRequest(string Email, string Role);
public sealed record InvitationResponse(Guid InvitationId, string Email, string Role, DateTime ExpiresAt, string InvitationToken);
public sealed record InvitationDto(Guid Id, Guid OrganizationId, string OrganizationName, string Email,
    string Role, OrganizationInvitationStatus Status, DateTime ExpiresAt, DateTime? AcceptedAt, bool UserExists);
public sealed record AddOrganizationMemberRequest(string Email, string Role);
public sealed record CreateOrganizationMemberRequest(string FirstName,string LastName,string Email,string TemporaryPassword,string Role);
public sealed record ChangeMemberRoleRequest(string Role);
public sealed record AcceptInvitationRequest(string Token);
