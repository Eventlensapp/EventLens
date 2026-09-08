using EventLensAI.Domain.Enums;
namespace EventLensAI.Application.DTOs.Organizations;
public sealed record PermissionDto(Guid Id,string Key,string Description,bool Allowed,bool IsOverride);
public sealed record SetMemberPermissionRequest(string PermissionKey,bool Allowed);
public sealed record CreateOwnershipTransferRequest(Guid NewOwnerUserId);
public sealed record OwnershipTransferDto(Guid Id,Guid OrganizationId,Guid FromUserId,Guid ToUserId,OwnershipTransferStatus Status,DateTime ExpiresAt,DateTime? AcceptedAt);
