using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Events;
using EventLensAI.Application.DTOs.Organizations;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Application.Services;

public interface IOrganizationService
{
    Task<IReadOnlyList<OrganizationDto>> ListAsync(CancellationToken ct);
    Task<OrganizationDto> GetAsync(Guid id, CancellationToken ct);
    Task<OrganizationDto> CreateAsync(CreateOrganizationRequest request, CancellationToken ct);
    Task<OrganizationDto> UpdateAsync(Guid id, UpdateOrganizationRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<OrganizationMemberDto>> ListMembersAsync(Guid id, CancellationToken ct);
    Task<InvitationResponse> InviteAsync(Guid id, InviteMemberRequest request, CancellationToken ct);
    Task AcceptInvitationAsync(string token, CancellationToken ct);
    Task RemoveMemberAsync(Guid id, Guid memberId, CancellationToken ct);
    Task ChangeRoleAsync(Guid id, Guid memberId, string role, CancellationToken ct);
    Task<OrganizationMemberDto> AddMemberAsync(Guid id, AddOrganizationMemberRequest request, CancellationToken ct);
    Task<OrganizationMemberDto> CreateMemberAsync(Guid id, CreateOrganizationMemberRequest request, CancellationToken ct);
    Task RemoveMemberByUserAsync(Guid id, Guid userId, CancellationToken ct);
    Task ChangeRoleByUserAsync(Guid id, Guid userId, string role, CancellationToken ct);
    Task<IReadOnlyList<InvitationDto>> ListInvitationsAsync(Guid id, CancellationToken ct);
    Task<InvitationDto> GetInvitationAsync(string token, CancellationToken ct);
    Task CancelInvitationAsync(Guid invitationId, CancellationToken ct);
}

public interface IEventService
{
    Task<PagedResult<EventDto>> SearchAsync(EventSearchRequest request, CancellationToken ct);
    Task<EventDto> GetAsync(Guid id, CancellationToken ct);
    Task<EventDto> CreateAsync(Guid organizationId, UpsertEventRequest request, CancellationToken ct);
    Task<EventDto> UpdateAsync(Guid id, UpsertEventRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<EventDto> PublishAsync(Guid id, CancellationToken ct);
    Task<EventDto> ArchiveAsync(Guid id, CancellationToken ct);
    Task<EventDto> DuplicateAsync(Guid id, DuplicateEventRequest request, CancellationToken ct);
    Task<EventSettingsDto> GetSettingsAsync(Guid id, CancellationToken ct);
    Task<EventSettingsDto> UpdateSettingsAsync(Guid id, UpdateEventSettingsRequest request, CancellationToken ct);
    Task<QrCodeDto> GenerateQrAsync(Guid id, CancellationToken ct);
    Task<string> GetQrSvgAsync(Guid id, CancellationToken ct);
    Task<EventDto> CreateAsync(CreateEventRequest request, CancellationToken ct);
    Task<EventDto> UpdateCoreAsync(Guid id, UpdateEventCoreRequest request, CancellationToken ct);
    Task<EventDto> RestoreAsync(Guid id, CancellationToken ct);
    Task<EventDto> ChangeStatusAsync(Guid id, EventStatus status, CancellationToken ct);
    Task<EventDashboardDto> DashboardAsync(Guid id, CancellationToken ct);
}

public interface IEventTypeService
{
    Task<IReadOnlyList<EventTypeDto>> ListAsync(Guid? organizationId, CancellationToken ct);
    Task<EventTypeDto>CreateAsync(CreateEventTypeRequest request,CancellationToken ct);
    Task<EventTypeDto>UpdateAsync(Guid id,UpdateEventTypeRequest request,CancellationToken ct);
    Task ArchiveAsync(Guid id,CancellationToken ct);
}

public interface IEventTemplateService
{
    Task<IReadOnlyList<EventTemplateDto>>ListAsync(Guid organizationId,CancellationToken ct);
    Task<EventTemplateDto>GetAsync(Guid id,CancellationToken ct);
    Task<EventTemplateDto>CreateAsync(CreateEventTemplateRequest request,CancellationToken ct);
    Task<EventTemplateDto>UpdateAsync(Guid id,UpdateEventTemplateRequest request,CancellationToken ct);
    Task ArchiveAsync(Guid id,CancellationToken ct);
    Task<EventTemplateDto>CloneAsync(Guid id,CloneEventTemplateRequest request,CancellationToken ct);
}

public interface IEventMemberService
{
    Task<IReadOnlyList<EventMemberDto>> ListAsync(Guid eventId, CancellationToken ct);
    Task<EventMemberDto> AssignAsync(Guid eventId, AssignEventMemberRequest request, CancellationToken ct);
    Task<EventMemberDto> ChangeRoleAsync(Guid eventId, Guid userId, ChangeEventMemberRoleRequest request, CancellationToken ct);
    Task RemoveAsync(Guid eventId, Guid userId, CancellationToken ct);
}
