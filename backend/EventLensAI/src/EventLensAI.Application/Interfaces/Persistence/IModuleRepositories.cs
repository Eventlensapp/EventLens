using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Events;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Application.Interfaces.Persistence;

public interface IOrganizationRepository
{
    Task<IReadOnlyList<Organization>> ListForUserAsync(Guid userId, CancellationToken ct);
    Task<Organization?> GetAsync(Guid id, CancellationToken ct);
    Task<bool> SlugExistsAsync(string slug, Guid? exceptId, CancellationToken ct);
    Task<OrganizationMember?> GetMemberAsync(Guid organizationId, Guid userId, CancellationToken ct);
    Task<OrganizationMember?> GetMemberByIdAsync(Guid organizationId, Guid memberId, CancellationToken ct);
    Task<IReadOnlyList<OrganizationMember>> ListMembersAsync(Guid organizationId, CancellationToken ct);
    Task<OrganizationInvitation?> GetInvitationByHashAsync(string tokenHash, CancellationToken ct);
    Task<OrganizationInvitation?> GetInvitationAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<OrganizationInvitation>> ListInvitationsAsync(Guid organizationId, CancellationToken ct);
    Task<Role?> GetRoleAsync(string roleName, CancellationToken ct);
    Task AddAsync(Organization organization, CancellationToken ct);
    Task AddMemberAsync(OrganizationMember member, CancellationToken ct);
    Task AddInvitationAsync(OrganizationInvitation invitation, CancellationToken ct);
}

public interface IEventRepository
{
    Task<PagedResult<Event>> SearchAsync(Guid userId, EventSearchRequest request, CancellationToken ct);
    Task<Event?> GetAsync(Guid id, CancellationToken ct);
    Task<Event?> GetBySlugAsync(string slug, CancellationToken ct);
    Task<bool> SlugExistsAsync(string slug, Guid? exceptId, CancellationToken ct);
    Task AddAsync(Event eventEntity, CancellationToken ct);
    Task<Event?> GetIncludingDeletedAsync(Guid id, CancellationToken ct);
    Task<EventTypeDefinition?> GetTypeAsync(Guid id, Guid organizationId, CancellationToken ct);
    Task<IReadOnlyList<EventTypeDefinition>> ListTypesAsync(Guid? organizationId, CancellationToken ct);
    Task<Branch?> GetBranchAsync(Guid id, Guid organizationId, CancellationToken ct);
    Task<BrandTheme?> GetBrandProfileAsync(Guid id, Guid organizationId, CancellationToken ct);
    Task<EventMember?> GetMemberAsync(Guid eventId, Guid userId, CancellationToken ct);
    Task<IReadOnlyList<EventMember>> ListMembersAsync(Guid eventId, CancellationToken ct);
    Task AddMemberAsync(EventMember member, CancellationToken ct);
    void RemoveMember(EventMember member);
    Task<EventTypeDefinition?> GetTypeIncludingDeletedAsync(Guid id,CancellationToken ct);
    Task<bool>TypeNameExistsAsync(Guid? organizationId,string name,Guid?exceptId,CancellationToken ct);
    Task AddTypeAsync(EventTypeDefinition type,CancellationToken ct);
    Task<EventTemplate?>GetTemplateAsync(Guid id,CancellationToken ct);
    Task<IReadOnlyList<EventTemplate>>ListTemplatesAsync(Guid organizationId,CancellationToken ct);
    Task<bool>TemplateNameExistsAsync(Guid organizationId,string name,Guid?exceptId,CancellationToken ct);
    Task AddTemplateAsync(EventTemplate template,CancellationToken ct);
    Task AddTemplateUsageAsync(TemplateUsage usage,CancellationToken ct);
    Task<EventBrandConfiguration?>GetBrandConfigurationAsync(Guid eventId,CancellationToken ct);
    Task<EventExperienceSettings?>GetExperienceAsync(Guid eventId,CancellationToken ct);
    Task<IReadOnlyList<EventAsset>>ListEventAssetsAsync(Guid eventId,CancellationToken ct);
    Task<EventAsset?>GetEventAssetAsync(Guid id,CancellationToken ct);
    Task<IReadOnlyList<EventSponsor>>ListSponsorsAsync(Guid eventId,CancellationToken ct);
    Task<EventSponsor?>GetSponsorAsync(Guid id,CancellationToken ct);
    Task AddBrandConfigurationAsync(EventBrandConfiguration entity,CancellationToken ct);
    Task AddExperienceAsync(EventExperienceSettings entity,CancellationToken ct);
    Task AddEventAssetAsync(EventAsset entity,CancellationToken ct);
    Task AddSponsorAsync(EventSponsor entity,CancellationToken ct);
    Task<IReadOnlyList<VenueType>>ListVenueTypesAsync(Guid organizationId,CancellationToken ct);Task<VenueType?>GetVenueTypeAsync(Guid id,Guid organizationId,CancellationToken ct);
    Task<IReadOnlyList<ScheduleType>>ListScheduleTypesAsync(Guid organizationId,CancellationToken ct);Task<ScheduleType?>GetScheduleTypeAsync(Guid id,Guid organizationId,CancellationToken ct);
    Task<IReadOnlyList<Venue>>ListVenuesAsync(Guid eventId,bool archived,CancellationToken ct);Task<Venue?>GetVenueAsync(Guid id,bool includeDeleted,CancellationToken ct);Task AddVenueAsync(Venue venue,CancellationToken ct);
    Task<IReadOnlyList<EventScheduleItem>>ListScheduleAsync(Guid eventId,CancellationToken ct);Task<EventScheduleItem?>GetScheduleItemAsync(Guid id,CancellationToken ct);Task AddScheduleItemAsync(EventScheduleItem item,CancellationToken ct);
    Task<EventChecklist?>GetChecklistAsync(Guid eventId,CancellationToken ct);Task<EventChecklistItem?>GetChecklistItemAsync(Guid id,CancellationToken ct);Task<IReadOnlyList<EventChecklistItem>>ListChecklistItemsAsync(Guid eventId,CancellationToken ct);Task AddChecklistAsync(EventChecklist x,CancellationToken ct);Task AddChecklistItemAsync(EventChecklistItem x,CancellationToken ct);
    Task<IReadOnlyList<EventStaffAssignment>>ListStaffAsync(Guid eventId,CancellationToken ct);Task<EventStaffAssignment?>GetStaffAsync(Guid id,CancellationToken ct);Task AddStaffAsync(EventStaffAssignment x,CancellationToken ct);
    Task<IReadOnlyList<EventZone>>ListZonesAsync(Guid eventId,CancellationToken ct);Task<EventZone?>GetZoneAsync(Guid id,CancellationToken ct);Task AddZoneAsync(EventZone x,CancellationToken ct);
    Task<IReadOnlyList<BoothPlacement>>ListPlacementsAsync(Guid eventId,CancellationToken ct);Task<BoothPlacement?>GetPlacementAsync(Guid id,CancellationToken ct);Task AddPlacementAsync(BoothPlacement x,CancellationToken ct);
    Task<IReadOnlyList<EventNotification>>ListNotificationsAsync(Guid userId,CancellationToken ct);Task<EventNotification?>GetNotificationAsync(Guid id,CancellationToken ct);Task AddNotificationAsync(EventNotification x,CancellationToken ct);
    Task<IReadOnlyList<EventQRCode>>ListQRCodesAsync(Guid eventId,CancellationToken ct);Task<EventQRCode?>GetQRCodeAsync(Guid id,CancellationToken ct);Task<EventQRCode?>GetQRCodeByTokenAsync(string token,CancellationToken ct);Task<bool>QRTokenExistsAsync(string token,CancellationToken ct);Task AddQRCodeAsync(EventQRCode x,CancellationToken ct);
    Task<EventAccessConfiguration?>GetAccessConfigurationAsync(Guid eventId,CancellationToken ct);Task AddAccessConfigurationAsync(EventAccessConfiguration x,CancellationToken ct);
    Task<GuestSession?>GetGuestSessionByTokenAsync(string token,CancellationToken ct);Task AddGuestSessionAsync(GuestSession x,CancellationToken ct);Task AddAccessLogAsync(EventAccessLog x,CancellationToken ct);Task<IReadOnlyList<EventAccessLog>>ListAccessLogsAsync(Guid eventId,CancellationToken ct);
}

public interface IAuditRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken ct);
}

public interface IPhotoRepository
{
    Task<Photo?> GetAsync(Guid id, CancellationToken ct);
    Task AddAsync(Photo photo, CancellationToken ct);
}
public interface ITemplateRepository
{
    Task<IReadOnlyList<Template>> ListAsync(Guid userId, Guid? organizationId, CancellationToken ct);
    Task<Template?> GetAsync(Guid id, CancellationToken ct);
    Task AddAsync(Template template, CancellationToken ct);
}
public interface IAIJobRepository
{
    Task<AIJob?> GetAsync(Guid id,CancellationToken ct);
    Task<IReadOnlyList<AIJob>> ListAsync(Guid userId,Guid? eventId,AIJobStatus? status,CancellationToken ct);
    Task<int> CountRecentAsync(Guid organizationId,DateTime since,CancellationToken ct);
    Task AddAsync(AIJob job,CancellationToken ct);
}
public interface IAIPromptRepository
{
    Task<AIPromptDefinition?> GetAsync(string key,AIJobType type,CancellationToken ct);
}
public interface IAIBackgroundRepository
{
    Task<IReadOnlyList<AIBackground>> ListAsync(Guid userId,Guid? organizationId,CancellationToken ct);
    Task AddAsync(AIBackground background,CancellationToken ct);
}
