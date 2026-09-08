using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Events;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;
using EventLensAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventLensAI.Infrastructure.Repositories;

internal sealed class OrganizationRepository(EventLensDbContext db) : IOrganizationRepository
{
    public async Task<IReadOnlyList<Organization>> ListForUserAsync(Guid userId, CancellationToken ct) =>
        await db.Organizations.AsNoTracking().Where(x => x.Members.Any(m => m.UserId == userId)).OrderBy(x => x.Name).ToListAsync(ct);
    public Task<Organization?> GetAsync(Guid id, CancellationToken ct) => db.Organizations.FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task<bool> SlugExistsAsync(string slug, Guid? exceptId, CancellationToken ct) =>
        db.Organizations.IgnoreQueryFilters()
            .AnyAsync(x => x.Slug == slug && (!exceptId.HasValue || x.Id != exceptId), ct);
    public Task<OrganizationMember?> GetMemberAsync(Guid organizationId, Guid userId, CancellationToken ct) =>
        db.OrganizationMembers.Include(x => x.Role).Include(x=>x.User).FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.UserId == userId, ct);
    public Task<OrganizationMember?> GetMemberByIdAsync(Guid organizationId, Guid memberId, CancellationToken ct) =>
        db.OrganizationMembers.Include(x => x.Role).FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Id == memberId, ct);
    public async Task<IReadOnlyList<OrganizationMember>> ListMembersAsync(Guid organizationId, CancellationToken ct) =>
        await db.OrganizationMembers.AsNoTracking().Include(x => x.User).Include(x => x.Role)
            .Where(x => x.OrganizationId == organizationId).OrderBy(x => x.User.FirstName).ToListAsync(ct);
    public Task<OrganizationInvitation?> GetInvitationByHashAsync(string tokenHash, CancellationToken ct) =>
        db.OrganizationInvitations.Include(x=>x.Organization).Include(x=>x.Role).FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
    public Task<OrganizationInvitation?> GetInvitationAsync(Guid id,CancellationToken ct)=>
        db.OrganizationInvitations.Include(x=>x.Organization).Include(x=>x.Role).FirstOrDefaultAsync(x=>x.Id==id,ct);
    public async Task<IReadOnlyList<OrganizationInvitation>> ListInvitationsAsync(Guid organizationId,CancellationToken ct)=>
        await db.OrganizationInvitations.AsNoTracking().Include(x=>x.Organization).Include(x=>x.Role)
            .Where(x=>x.OrganizationId==organizationId).OrderByDescending(x=>x.CreatedAt).ToListAsync(ct);
    public Task<Role?> GetRoleAsync(string roleName, CancellationToken ct) =>
        db.Roles.FirstOrDefaultAsync(x => x.Name.ToLower() == roleName.ToLower(), ct);
    public Task AddAsync(Organization entity, CancellationToken ct) => db.Organizations.AddAsync(entity, ct).AsTask();
    public Task AddMemberAsync(OrganizationMember entity, CancellationToken ct) => db.OrganizationMembers.AddAsync(entity, ct).AsTask();
    public Task AddInvitationAsync(OrganizationInvitation entity, CancellationToken ct) => db.OrganizationInvitations.AddAsync(entity, ct).AsTask();
}

internal sealed class EventRepository(EventLensDbContext db) : IEventRepository
{
    public async Task<PagedResult<Event>> SearchAsync(Guid userId, EventSearchRequest request, CancellationToken ct)
    {
        var query = db.Events.AsNoTracking().Where(x => x.Organization.Members.Any(m => m.UserId == userId));
        if (request.OrganizationId.HasValue) query = query.Where(x => x.OrganizationId == request.OrganizationId);
        if (!string.IsNullOrWhiteSpace(request.Search)) query = query.Where(x => x.Name.ToLower().Contains(request.Search.ToLower()));
        if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status);
        if (request.EventType.HasValue) query = query.Where(x => x.EventType == request.EventType);
        if (request.FromDate.HasValue) query = query.Where(x => x.StartDate >= request.FromDate.Value.ToUniversalTime());
        if (request.ToDate.HasValue) query = query.Where(x => x.StartDate <= request.ToDate.Value.ToUniversalTime());
        query = (request.SortBy.ToLowerInvariant(), request.Descending) switch
        {
            ("name", false) => query.OrderBy(x => x.Name), ("name", true) => query.OrderByDescending(x => x.Name),
            ("enddate", false) => query.OrderBy(x => x.EndDate), ("enddate", true) => query.OrderByDescending(x => x.EndDate),
            ("status", false) => query.OrderBy(x => x.Status), ("status", true) => query.OrderByDescending(x => x.Status),
            ("createdat", false) => query.OrderBy(x => x.CreatedAt), ("createdat", true) => query.OrderByDescending(x => x.CreatedAt),
            (_, true) => query.OrderByDescending(x => x.StartDate), _ => query.OrderBy(x => x.StartDate)
        };
        var total = await query.CountAsync(ct);
        var items = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync(ct);
        return new(items, request.Page, request.PageSize, total);
    }
    public Task<Event?> GetAsync(Guid id, CancellationToken ct) =>
        db.Events.Include(x => x.Settings).Include(x => x.Branding).Include(x=>x.EventTypeDefinition)
            .Include(x=>x.Branch).Include(x=>x.AssignedManager).FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task<Event?> GetIncludingDeletedAsync(Guid id,CancellationToken ct) =>
        db.Events.IgnoreQueryFilters().Include(x=>x.Settings).Include(x=>x.Branding).Include(x=>x.EventTypeDefinition)
            .Include(x=>x.Branch).Include(x=>x.AssignedManager).FirstOrDefaultAsync(x=>x.Id==id,ct);
    public Task<Event?> GetBySlugAsync(string slug, CancellationToken ct) =>
        db.Events.AsNoTracking().Include(x => x.Settings)
            .FirstOrDefaultAsync(x => x.Slug == slug.ToLower(), ct);
    public Task<bool> SlugExistsAsync(string slug, Guid? exceptId, CancellationToken ct) =>
        db.Events.AnyAsync(x => x.Slug == slug && (!exceptId.HasValue || x.Id != exceptId), ct);
    public Task AddAsync(Event entity, CancellationToken ct) => db.Events.AddAsync(entity, ct).AsTask();
    public Task<EventTypeDefinition?> GetTypeAsync(Guid id,Guid organizationId,CancellationToken ct)=>
        db.EventTypes.FirstOrDefaultAsync(x=>x.Id==id&&(x.OrganizationId==null||x.OrganizationId==organizationId)&&x.Status==EventTypeStatus.Active,ct);
    public async Task<IReadOnlyList<EventTypeDefinition>> ListTypesAsync(Guid? organizationId,CancellationToken ct)=>
        await db.EventTypes.AsNoTracking().Where(x=>x.Status==EventTypeStatus.Active&&(x.OrganizationId==null||x.OrganizationId==organizationId))
            .OrderBy(x=>x.Name).ToListAsync(ct);
    public Task<Branch?> GetBranchAsync(Guid id,Guid organizationId,CancellationToken ct)=>
        db.Branches.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id&&x.OrganizationId==organizationId,ct);
    public Task<BrandTheme?> GetBrandProfileAsync(Guid id,Guid organizationId,CancellationToken ct)=>
        db.BrandThemes.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id&&x.OrganizationId==organizationId,ct);
    public Task<EventMember?> GetMemberAsync(Guid eventId,Guid userId,CancellationToken ct)=>
        db.EventMembers.Include(x=>x.User).FirstOrDefaultAsync(x=>x.EventId==eventId&&x.UserId==userId,ct);
    public async Task<IReadOnlyList<EventMember>> ListMembersAsync(Guid eventId,CancellationToken ct)=>
        await db.EventMembers.AsNoTracking().Include(x=>x.User).Where(x=>x.EventId==eventId)
            .OrderBy(x=>x.User.FirstName).ThenBy(x=>x.User.LastName).ToListAsync(ct);
    public Task AddMemberAsync(EventMember member,CancellationToken ct)=>db.EventMembers.AddAsync(member,ct).AsTask();
    public void RemoveMember(EventMember member)=>db.EventMembers.Remove(member);
    public Task<EventTypeDefinition?>GetTypeIncludingDeletedAsync(Guid id,CancellationToken ct)=>db.EventTypes.IgnoreQueryFilters().FirstOrDefaultAsync(x=>x.Id==id,ct);
    public Task<bool>TypeNameExistsAsync(Guid? organizationId,string name,Guid?exceptId,CancellationToken ct)=>db.EventTypes.AnyAsync(x=>x.OrganizationId==organizationId&&x.Name==name&&(!exceptId.HasValue||x.Id!=exceptId),ct);
    public Task AddTypeAsync(EventTypeDefinition type,CancellationToken ct)=>db.EventTypes.AddAsync(type,ct).AsTask();
    public Task<EventTemplate?>GetTemplateAsync(Guid id,CancellationToken ct)=>db.EventTemplates.Include(x=>x.EventType).Include(x=>x.DefaultBrandProfile).FirstOrDefaultAsync(x=>x.Id==id,ct);
    public async Task<IReadOnlyList<EventTemplate>>ListTemplatesAsync(Guid organizationId,CancellationToken ct)=>await db.EventTemplates.AsNoTracking().Include(x=>x.EventType).Include(x=>x.DefaultBrandProfile).Where(x=>x.OrganizationId==organizationId).OrderBy(x=>x.Name).ToListAsync(ct);
    public Task<bool>TemplateNameExistsAsync(Guid organizationId,string name,Guid?exceptId,CancellationToken ct)=>db.EventTemplates.AnyAsync(x=>x.OrganizationId==organizationId&&x.Name==name&&(!exceptId.HasValue||x.Id!=exceptId),ct);
    public Task AddTemplateAsync(EventTemplate template,CancellationToken ct)=>db.EventTemplates.AddAsync(template,ct).AsTask();
    public Task AddTemplateUsageAsync(TemplateUsage usage,CancellationToken ct)=>db.EventTemplateUsages.AddAsync(usage,ct).AsTask();
    public Task<EventBrandConfiguration?>GetBrandConfigurationAsync(Guid id,CancellationToken ct)=>db.EventBrandConfigurations.FirstOrDefaultAsync(x=>x.EventId==id,ct);
    public Task<EventExperienceSettings?>GetExperienceAsync(Guid id,CancellationToken ct)=>db.EventExperienceSettings.FirstOrDefaultAsync(x=>x.EventId==id,ct);
    public async Task<IReadOnlyList<EventAsset>>ListEventAssetsAsync(Guid id,CancellationToken ct)=>await db.EventAssets.AsNoTracking().Where(x=>x.EventId==id).OrderBy(x=>x.DisplayOrder).ToListAsync(ct);
    public Task<EventAsset?>GetEventAssetAsync(Guid id,CancellationToken ct)=>db.EventAssets.FirstOrDefaultAsync(x=>x.Id==id,ct);
    public async Task<IReadOnlyList<EventSponsor>>ListSponsorsAsync(Guid id,CancellationToken ct)=>await db.EventSponsors.AsNoTracking().Where(x=>x.EventId==id).OrderBy(x=>x.DisplayOrder).ToListAsync(ct);
    public Task<EventSponsor?>GetSponsorAsync(Guid id,CancellationToken ct)=>db.EventSponsors.FirstOrDefaultAsync(x=>x.Id==id,ct);
    public Task AddBrandConfigurationAsync(EventBrandConfiguration x,CancellationToken ct)=>db.EventBrandConfigurations.AddAsync(x,ct).AsTask();
    public Task AddExperienceAsync(EventExperienceSettings x,CancellationToken ct)=>db.EventExperienceSettings.AddAsync(x,ct).AsTask();
    public Task AddEventAssetAsync(EventAsset x,CancellationToken ct)=>db.EventAssets.AddAsync(x,ct).AsTask();
    public Task AddSponsorAsync(EventSponsor x,CancellationToken ct)=>db.EventSponsors.AddAsync(x,ct).AsTask();
    public async Task<IReadOnlyList<VenueType>>ListVenueTypesAsync(Guid o,CancellationToken ct)=>await db.VenueTypes.AsNoTracking().Where(x=>x.IsActive&&(x.OrganizationId==null||x.OrganizationId==o)).OrderBy(x=>x.Name).ToListAsync(ct);
    public Task<VenueType?>GetVenueTypeAsync(Guid id,Guid o,CancellationToken ct)=>db.VenueTypes.FirstOrDefaultAsync(x=>x.Id==id&&x.IsActive&&(x.OrganizationId==null||x.OrganizationId==o),ct);
    public async Task<IReadOnlyList<ScheduleType>>ListScheduleTypesAsync(Guid o,CancellationToken ct)=>await db.ScheduleTypes.AsNoTracking().Where(x=>x.IsActive&&(x.OrganizationId==null||x.OrganizationId==o)).OrderBy(x=>x.Name).ToListAsync(ct);
    public Task<ScheduleType?>GetScheduleTypeAsync(Guid id,Guid o,CancellationToken ct)=>db.ScheduleTypes.FirstOrDefaultAsync(x=>x.Id==id&&x.IsActive&&(x.OrganizationId==null||x.OrganizationId==o),ct);
    public async Task<IReadOnlyList<Venue>>ListVenuesAsync(Guid e,bool archived,CancellationToken ct){var q=archived?db.Venues.IgnoreQueryFilters().Where(x=>x.IsDeleted):db.Venues.Where(x=>!x.IsDeleted);return await q.AsNoTracking().Include(x=>x.VenueType).Where(x=>x.EventId==e).OrderByDescending(x=>x.IsPrimary).ThenBy(x=>x.Name).ToListAsync(ct);}
    public Task<Venue?>GetVenueAsync(Guid id,bool deleted,CancellationToken ct)=>(deleted?db.Venues.IgnoreQueryFilters():db.Venues).Include(x=>x.VenueType).FirstOrDefaultAsync(x=>x.Id==id,ct);
    public Task AddVenueAsync(Venue x,CancellationToken ct)=>db.Venues.AddAsync(x,ct).AsTask();
    public async Task<IReadOnlyList<EventScheduleItem>>ListScheduleAsync(Guid e,CancellationToken ct)=>await db.EventScheduleItems.AsNoTracking().Include(x=>x.Venue).Include(x=>x.ScheduleType).Where(x=>x.EventId==e).OrderBy(x=>x.StartDateTime).ThenBy(x=>x.DisplayOrder).ToListAsync(ct);
    public Task<EventScheduleItem?>GetScheduleItemAsync(Guid id,CancellationToken ct)=>db.EventScheduleItems.Include(x=>x.Venue).Include(x=>x.ScheduleType).FirstOrDefaultAsync(x=>x.Id==id,ct);
    public Task AddScheduleItemAsync(EventScheduleItem x,CancellationToken ct)=>db.EventScheduleItems.AddAsync(x,ct).AsTask();
    public Task<EventChecklist?>GetChecklistAsync(Guid e,CancellationToken ct)=>db.EventChecklists.Include(x=>x.Items).FirstOrDefaultAsync(x=>x.EventId==e,ct);
    public Task<EventChecklistItem?>GetChecklistItemAsync(Guid id,CancellationToken ct)=>db.EventChecklistItems.Include(x=>x.AssignedUser).Include(x=>x.Event).FirstOrDefaultAsync(x=>x.Id==id,ct);
    public async Task<IReadOnlyList<EventChecklistItem>>ListChecklistItemsAsync(Guid e,CancellationToken ct)=>await db.EventChecklistItems.AsNoTracking().Include(x=>x.AssignedUser).Where(x=>x.EventId==e).OrderBy(x=>x.DisplayOrder).ToListAsync(ct);
    public Task AddChecklistAsync(EventChecklist x,CancellationToken ct)=>db.EventChecklists.AddAsync(x,ct).AsTask();public Task AddChecklistItemAsync(EventChecklistItem x,CancellationToken ct)=>db.EventChecklistItems.AddAsync(x,ct).AsTask();
    public async Task<IReadOnlyList<EventStaffAssignment>>ListStaffAsync(Guid e,CancellationToken ct)=>await db.EventStaffAssignments.AsNoTracking().Include(x=>x.User).Where(x=>x.EventId==e).OrderBy(x=>x.StartDateTime).ToListAsync(ct);
    public Task<EventStaffAssignment?>GetStaffAsync(Guid id,CancellationToken ct)=>db.EventStaffAssignments.Include(x=>x.User).Include(x=>x.Event).FirstOrDefaultAsync(x=>x.Id==id,ct);public Task AddStaffAsync(EventStaffAssignment x,CancellationToken ct)=>db.EventStaffAssignments.AddAsync(x,ct).AsTask();
    public async Task<IReadOnlyList<EventZone>>ListZonesAsync(Guid e,CancellationToken ct)=>await db.EventZones.AsNoTracking().Include(x=>x.Venue).Where(x=>x.Venue.EventId==e).OrderBy(x=>x.Venue.Name).ThenBy(x=>x.Name).ToListAsync(ct);
    public Task<EventZone?>GetZoneAsync(Guid id,CancellationToken ct)=>db.EventZones.Include(x=>x.Venue).ThenInclude(x=>x.Event).FirstOrDefaultAsync(x=>x.Id==id,ct);public Task AddZoneAsync(EventZone x,CancellationToken ct)=>db.EventZones.AddAsync(x,ct).AsTask();
    public async Task<IReadOnlyList<BoothPlacement>>ListPlacementsAsync(Guid e,CancellationToken ct)=>await db.BoothPlacements.AsNoTracking().Include(x=>x.Venue).Include(x=>x.Zone).Include(x=>x.AssignedOperator).Where(x=>x.EventId==e).OrderBy(x=>x.Name).ToListAsync(ct);
    public Task<BoothPlacement?>GetPlacementAsync(Guid id,CancellationToken ct)=>db.BoothPlacements.Include(x=>x.Event).Include(x=>x.Venue).Include(x=>x.Zone).Include(x=>x.AssignedOperator).FirstOrDefaultAsync(x=>x.Id==id,ct);public Task AddPlacementAsync(BoothPlacement x,CancellationToken ct)=>db.BoothPlacements.AddAsync(x,ct).AsTask();
    public async Task<IReadOnlyList<EventNotification>>ListNotificationsAsync(Guid u,CancellationToken ct)=>await db.EventNotifications.AsNoTracking().Where(x=>x.UserId==u).OrderByDescending(x=>x.CreatedAt).Take(200).ToListAsync(ct);public Task<EventNotification?>GetNotificationAsync(Guid id,CancellationToken ct)=>db.EventNotifications.FirstOrDefaultAsync(x=>x.Id==id,ct);public Task AddNotificationAsync(EventNotification x,CancellationToken ct)=>db.EventNotifications.AddAsync(x,ct).AsTask();
    public async Task<IReadOnlyList<EventQRCode>>ListQRCodesAsync(Guid e,CancellationToken ct)=>await db.EventQRCodes.AsNoTracking().Where(x=>x.EventId==e).OrderByDescending(x=>x.CreatedAt).ToListAsync(ct);
    public Task<EventQRCode?>GetQRCodeAsync(Guid id,CancellationToken ct)=>db.EventQRCodes.Include(x=>x.Event).FirstOrDefaultAsync(x=>x.Id==id,ct);public Task<EventQRCode?>GetQRCodeByTokenAsync(string token,CancellationToken ct)=>db.EventQRCodes.AsNoTracking().Include(x=>x.Event).FirstOrDefaultAsync(x=>x.Token==token,ct);public Task<bool>QRTokenExistsAsync(string token,CancellationToken ct)=>db.EventQRCodes.IgnoreQueryFilters().AnyAsync(x=>x.Token==token,ct);public Task AddQRCodeAsync(EventQRCode x,CancellationToken ct)=>db.EventQRCodes.AddAsync(x,ct).AsTask();
    public Task<EventAccessConfiguration?>GetAccessConfigurationAsync(Guid e,CancellationToken ct)=>db.EventAccessConfigurations.FirstOrDefaultAsync(x=>x.EventId==e,ct);public Task AddAccessConfigurationAsync(EventAccessConfiguration x,CancellationToken ct)=>db.EventAccessConfigurations.AddAsync(x,ct).AsTask();
    public Task<GuestSession?>GetGuestSessionByTokenAsync(string token,CancellationToken ct)=>db.GuestSessions.FirstOrDefaultAsync(x=>x.SessionToken==token,ct);public Task AddGuestSessionAsync(GuestSession x,CancellationToken ct)=>db.GuestSessions.AddAsync(x,ct).AsTask();public Task AddAccessLogAsync(EventAccessLog x,CancellationToken ct)=>db.EventAccessLogs.AddAsync(x,ct).AsTask();public async Task<IReadOnlyList<EventAccessLog>>ListAccessLogsAsync(Guid e,CancellationToken ct)=>await db.EventAccessLogs.AsNoTracking().Where(x=>x.EventId==e).ToListAsync(ct);
}
internal sealed class AuditRepository(EventLensDbContext db) : IAuditRepository
{
    public Task AddAsync(AuditLog entity, CancellationToken ct) => db.AuditLogs.AddAsync(entity, ct).AsTask();
}
internal sealed class PhotoRepository(EventLensDbContext db) : IPhotoRepository
{
    public Task<Photo?> GetAsync(Guid id, CancellationToken ct) =>
        db.Photos.Include(x => x.Event).FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task AddAsync(Photo entity, CancellationToken ct) => db.Photos.AddAsync(entity, ct).AsTask();
}
internal sealed class TemplateRepository(EventLensDbContext db) : ITemplateRepository
{
    public async Task<IReadOnlyList<Template>> ListAsync(Guid userId, Guid? organizationId, CancellationToken ct) =>
        await db.Templates.AsNoTracking()
            .Where(x => (x.OrganizationId == null || x.Organization!.Members.Any(m => m.UserId == userId)) &&
                (!organizationId.HasValue || x.OrganizationId == organizationId))
            .OrderBy(x => x.Category).ThenBy(x => x.Name).ToListAsync(ct);
    public Task<Template?> GetAsync(Guid id, CancellationToken ct) => db.Templates.FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task AddAsync(Template entity, CancellationToken ct) => db.Templates.AddAsync(entity, ct).AsTask();
}
internal sealed class AIJobRepository(EventLensDbContext db):IAIJobRepository
{
 public Task<AIJob?> GetAsync(Guid id,CancellationToken ct)=>db.AIJobs.FirstOrDefaultAsync(x=>x.Id==id,ct);
 public async Task<IReadOnlyList<AIJob>> ListAsync(Guid userId,Guid? eventId,AIJobStatus? status,CancellationToken ct)=>await db.AIJobs.AsNoTracking().Where(x=>x.Event.Organization.Members.Any(m=>m.UserId==userId)&&(!eventId.HasValue||x.EventId==eventId)&&(!status.HasValue||x.Status==status)).OrderByDescending(x=>x.CreatedAt).Take(200).ToListAsync(ct);
 public Task<int> CountRecentAsync(Guid organizationId,DateTime since,CancellationToken ct)=>db.AIJobs.CountAsync(x=>x.Event.OrganizationId==organizationId&&x.CreatedAt>=since,ct);
 public Task AddAsync(AIJob job,CancellationToken ct)=>db.AIJobs.AddAsync(job,ct).AsTask();
}
internal sealed class AIPromptRepository(EventLensDbContext db):IAIPromptRepository
{
 public Task<AIPromptDefinition?> GetAsync(string key,AIJobType type,CancellationToken ct)=>db.AIPromptDefinitions.AsNoTracking().FirstOrDefaultAsync(x=>x.Key==key&&x.JobType==type&&x.IsActive,ct);
}
internal sealed class AIBackgroundRepository(EventLensDbContext db):IAIBackgroundRepository
{
 public async Task<IReadOnlyList<AIBackground>> ListAsync(Guid userId,Guid? organizationId,CancellationToken ct)=>await db.AIBackgrounds.AsNoTracking().Where(x=>x.OrganizationId==null||x.Organization!.Members.Any(m=>m.UserId==userId)&&(!organizationId.HasValue||x.OrganizationId==organizationId)).OrderBy(x=>x.Category).ThenBy(x=>x.Name).ToListAsync(ct);
 public Task AddAsync(AIBackground background,CancellationToken ct)=>db.AIBackgrounds.AddAsync(background,ct).AsTask();
}
