using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Events;
using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Application.Interfaces.Services;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;
using EventLensAI.Application.Features.Billing;
using System.Text.Json;

namespace EventLensAI.Application.Services;

public sealed class EventService(
    IEventRepository events, IOrganizationRepository organizations, IAuditRepository audits,
    IQrCodeService qrCodes, IPublicUrlService urls, ICurrentUserService current, IUnitOfWork unitOfWork,
    IFeaturePermissionService features) : IEventService
{
    public async Task<PagedResult<EventDto>> SearchAsync(EventSearchRequest request, CancellationToken ct)
    {
        var result = await events.SearchAsync(UserId, request, ct);
        return new(result.Items.Select(Map).ToArray(), result.Page, result.PageSize, result.TotalCount);
    }
    public async Task<EventDto> GetAsync(Guid id, CancellationToken ct)
    {
        var entity = await Entity(id, ct); await RequireRead(entity.OrganizationId, ct); return Map(entity);
    }
    public async Task<EventDto> CreateAsync(Guid organizationId, UpsertEventRequest request, CancellationToken ct)
    {
        await RequireManage(organizationId, ct);
        await features.EnsureAsync(organizationId, BillingFeature.Events, 1, ct);
        var slug = await UniqueSlug(request.Name, ct);
        var entity = new Event(organizationId, request.Name, slug, request.EventType, request.StartDate, request.EndDate, UserId);
        Apply(entity, request); entity.InitializeComponents();
        await events.AddAsync(entity, ct); await Audit(entity.Id, AuditAction.Create, ct);
        await unitOfWork.SaveChangesAsync(ct); await features.TrackAsync(organizationId, UsageMetric.EventsCreated, 1, ct); return Map(entity);
    }
    public async Task<EventDto> CreateAsync(CreateEventRequest request, CancellationToken ct)
    {
        await RequireManage(request.OrganizationId, ct);
        EventTemplate? template=null;
        if(request.TemplateId.HasValue)
        {
            template=await events.GetTemplateAsync(request.TemplateId.Value,ct)??throw new NotFoundException("Event template not found.");
            if(template.OrganizationId!=request.OrganizationId||!template.IsActive)throw new UnauthorizedException("Event template access denied.");
        }
        var eventTypeId=request.EventTypeId!=Guid.Empty?request.EventTypeId:template?.EventTypeId??Guid.Empty;
        var brandProfileId=request.BrandProfileId??template?.DefaultBrandProfileId;
        var endDate=request.EndDate>request.StartDate?request.EndDate:request.StartDate+(template?.DefaultDuration??TimeSpan.FromHours(4));
        await ValidateReferences(request.OrganizationId, eventTypeId, request.BranchId,
            request.AssignedManagerId, brandProfileId, ct);
        await features.EnsureAsync(request.OrganizationId, BillingFeature.Events, 1, ct);
        var slug=await UniqueSlug(request.Name,ct);
        var configuration=template is null?null:JsonSerializer.Deserialize<EventTemplateConfiguration>(template.ConfigurationJson);
        var entity=new Event(request.OrganizationId,request.Name,slug,LegacyType(eventTypeId),
            request.StartDate,endDate,UserId);
        entity.Update(request.Name,request.Description,LegacyType(eventTypeId),request.VenueName,request.Address,
            null,null,request.StartDate,endDate,null,null,"#111111","#FFFFFF",null,null,null,
            configuration?.GalleryEnabled??true,false,true,true,configuration?.AIEnabled??false,true);
        entity.ConfigureCore(eventTypeId,request.BranchId,request.AssignedManagerId,brandProfileId,
            request.Timezone,request.VenueName,request.Address,request.City,request.Country,request.ContactPerson,request.ContactEmail,request.ContactPhone);
        if(request.Status!=EventStatus.Draft) entity.ChangeStatus(request.Status);
        entity.InitializeComponents();
        await events.AddAsync(entity,ct);
        await events.AddMemberAsync(new EventMember(entity.Id,UserId,EventMemberRole.EventOwner,UserId),ct);
        if(template is not null)await events.AddTemplateUsageAsync(new TemplateUsage(template.Id,entity.Id,UserId),ct);
        await Audit(entity.Id,AuditAction.Create,ct); await unitOfWork.SaveChangesAsync(ct);
        await features.TrackAsync(request.OrganizationId,UsageMetric.EventsCreated,1,ct);
        return Map(entity);
    }
    public async Task<EventDto> UpdateCoreAsync(Guid id,UpdateEventCoreRequest request,CancellationToken ct)
    {
        var entity=await Entity(id,ct); await RequireManage(entity.OrganizationId,ct);
        await ValidateReferences(entity.OrganizationId,request.EventTypeId,request.BranchId,request.AssignedManagerId,request.BrandProfileId,ct);
        entity.Update(request.Name,request.Description,LegacyType(request.EventTypeId),request.VenueName,request.Address,
            entity.Latitude,entity.Longitude,request.StartDate,request.EndDate,entity.CoverImage,entity.Logo,
            entity.PrimaryColor,entity.SecondaryColor,entity.GuestLimit,entity.PhotoLimit,entity.StorageLimit,
            entity.PublicGalleryEnabled,entity.RequireGuestRegistration,entity.AllowDownloads,entity.AllowSocialSharing,entity.EnableAI,entity.EnableQRCode);
        entity.ConfigureCore(request.EventTypeId,request.BranchId,request.AssignedManagerId,request.BrandProfileId,
            request.Timezone,request.VenueName,request.Address,request.City,request.Country,request.ContactPerson,request.ContactEmail,request.ContactPhone);
        if(entity.Status!=request.Status)entity.ChangeStatus(request.Status);
        entity.MarkUpdated(UserId); await Audit(id,AuditAction.Update,ct); await unitOfWork.SaveChangesAsync(ct); return Map(entity);
    }
    public async Task<EventDto> UpdateAsync(Guid id, UpsertEventRequest request, CancellationToken ct)
    {
        var entity = await Entity(id, ct); await RequireManage(entity.OrganizationId, ct);
        Apply(entity, request); entity.MarkUpdated(UserId); await Audit(id, AuditAction.Update, ct);
        await unitOfWork.SaveChangesAsync(ct); return Map(entity);
    }
    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await Entity(id, ct); await RequireManage(entity.OrganizationId, ct);
        entity.SoftDelete(UserId); await Audit(id, AuditAction.Delete, ct); await unitOfWork.SaveChangesAsync(ct);
    }
    public async Task<EventDto> PublishAsync(Guid id, CancellationToken ct) =>
        await Transition(id, AuditAction.Publish, entity => entity.Publish(), ct);
    public async Task<EventDto> ArchiveAsync(Guid id, CancellationToken ct) =>
        await Transition(id, AuditAction.Archive, entity => { entity.Archive(); entity.SoftDelete(UserId); }, ct);
    public async Task<EventDto> RestoreAsync(Guid id,CancellationToken ct)
    {
        var entity=await events.GetIncludingDeletedAsync(id,ct)??throw new NotFoundException("Event not found.");
        await RequireManage(entity.OrganizationId,ct); entity.RestoreFromArchive(UserId);
        await Audit(id,AuditAction.Update,ct); await unitOfWork.SaveChangesAsync(ct); return Map(entity);
    }
    public Task<EventDto> ChangeStatusAsync(Guid id,EventStatus status,CancellationToken ct)=>
        Transition(id,AuditAction.Update,e=>e.ChangeStatus(status),ct);
    public async Task<EventDto> DuplicateAsync(Guid id, DuplicateEventRequest request, CancellationToken ct)
    {
        var source = await Entity(id, ct); await RequireManage(source.OrganizationId, ct);
        var dto = new UpsertEventRequest(request.Name, source.Description, source.EventType, source.Venue,
            source.Address, source.Latitude, source.Longitude, request.StartDate, request.EndDate,
            source.CoverImage, source.Logo, source.PrimaryColor, source.SecondaryColor, source.GuestLimit,
            source.PhotoLimit, source.StorageLimit, source.PublicGalleryEnabled, source.RequireGuestRegistration,
            source.AllowDownloads, source.AllowSocialSharing, source.EnableAI, source.EnableQRCode);
        var clone=await CreateAsync(source.OrganizationId,dto,ct);
        if(source.EventTypeId.HasValue)
        {
            var target=await Entity(clone.Id,ct);
            target.ConfigureCore(source.EventTypeId.Value,source.BranchId,source.AssignedManagerId,source.BrandProfileId,
                source.Timezone,source.Venue,source.Address,source.City,source.Country,source.ContactPerson,source.ContactEmail,source.ContactPhone);
            await unitOfWork.SaveChangesAsync(ct); return Map(target);
        }
        return clone;
    }
    public async Task<EventSettingsDto> GetSettingsAsync(Guid id, CancellationToken ct)
    {
        var entity = await Entity(id, ct); await RequireRead(entity.OrganizationId, ct);
        return MapSettings(entity.Settings);
    }
    public async Task<EventSettingsDto> UpdateSettingsAsync(Guid id, UpdateEventSettingsRequest request, CancellationToken ct)
    {
        var entity = await Entity(id, ct); await RequireManage(entity.OrganizationId, ct);
        entity.Settings.Update(request.Countdown, request.CaptureMode, request.TemplateId, request.Language,
            request.Watermark, request.DefaultFilter, request.PrintEnabled, request.GIFEnabled,
            request.BoomerangEnabled, request.VideoEnabled, request.AIEnabled,
            request.BackgroundRemovalEnabled, request.FaceDetectionEnabled);
        await Audit(id, AuditAction.Update, ct); await unitOfWork.SaveChangesAsync(ct);
        return MapSettings(entity.Settings);
    }
    public async Task<QrCodeDto> GenerateQrAsync(Guid id, CancellationToken ct)
    {
        var entity = await Entity(id, ct); await RequireManage(entity.OrganizationId, ct);
        var publicUrl = urls.GetEventUrl(entity.Slug); var imageUrl = urls.GetQrImageUrl(entity.Id);
        entity.SetPublicAccess(publicUrl, imageUrl, qrCodes.GenerateSvg(publicUrl));
        await unitOfWork.SaveChangesAsync(ct); return new(publicUrl, imageUrl);
    }
    public async Task<string> GetQrSvgAsync(Guid id, CancellationToken ct)
    {
        var entity = await Entity(id, ct); await RequireRead(entity.OrganizationId, ct);
        if (string.IsNullOrWhiteSpace(entity.QRCodeSvg)) await GenerateQrAsync(id, ct);
        return entity.QRCodeSvg!;
    }
    private async Task<EventDto> Transition(Guid id, AuditAction action, Action<Event> transition, CancellationToken ct)
    {
        var entity = await Entity(id, ct); await RequireManage(entity.OrganizationId, ct); transition(entity);
        entity.MarkUpdated(UserId); await Audit(id, action, ct); await unitOfWork.SaveChangesAsync(ct); return Map(entity);
    }
    private async Task<Event> Entity(Guid id, CancellationToken ct) =>
        await events.GetAsync(id, ct) ?? throw new NotFoundException("Event not found.");
    private async Task RequireRead(Guid organizationId, CancellationToken ct)
    {
        if (current.Roles.Contains(SystemRoles.SuperAdmin)) return;
        _ = await organizations.GetMemberAsync(organizationId, UserId, ct)
            ?? throw new UnauthorizedException("Event access denied.");
    }
    private async Task RequireManage(Guid organizationId, CancellationToken ct)
    {
        if (current.Roles.Contains(SystemRoles.SuperAdmin)) return;
        var member = await organizations.GetMemberAsync(organizationId, UserId, ct)
            ?? throw new UnauthorizedException("Event access denied.");
        if (member.Role.Name is not (SystemRoles.Owner or SystemRoles.Manager))
            throw new UnauthorizedException("Event management permission denied.");
    }
    private async Task<string> UniqueSlug(string name, CancellationToken ct)
    {
        var baseSlug = OrganizationService.Slug(name); var slug = baseSlug; var suffix = 2;
        while (await events.SlugExistsAsync(slug, null, ct)) slug = $"{baseSlug}-{suffix++}";
        return slug;
    }
    private static void Apply(Event e, UpsertEventRequest x) => e.Update(x.Name, x.Description, x.EventType,
        x.Venue, x.Address, x.Latitude, x.Longitude, x.StartDate, x.EndDate, x.CoverImage, x.Logo,
        x.PrimaryColor, x.SecondaryColor, x.GuestLimit, x.PhotoLimit, x.StorageLimit,
        x.PublicGalleryEnabled, x.RequireGuestRegistration, x.AllowDownloads, x.AllowSocialSharing, x.EnableAI, x.EnableQRCode);
    private async Task Audit(Guid resourceId, AuditAction action, CancellationToken ct) =>
        await audits.AddAsync(new AuditLog(UserId, nameof(Event), resourceId, action, current.IPAddress), ct);
    private Guid UserId => current.UserId ?? throw new UnauthorizedException("Authentication is required.");
    public async Task<EventDashboardDto> DashboardAsync(Guid id,CancellationToken ct)
    {
        var entity=await Entity(id,ct);await RequireRead(entity.OrganizationId,ct);
        var team=await events.ListMembersAsync(id,ct);
        var mapped=team.Select(MapMember).ToArray();
        var organization=await organizations.GetAsync(entity.OrganizationId,ct)??throw new NotFoundException("Organization not found.");
        return new(Map(entity),organization.Name,entity.Branch?.Name,entity.EventTypeDefinition?.Name,
            entity.AssignedManagerId.HasValue?mapped.FirstOrDefault(x=>x.UserId==entity.AssignedManagerId):null,
            mapped,entity.Photos.Count,0,entity.Galleries.Count>0?"Configured":"Not configured",
            "Not configured",0);
    }
    private async Task ValidateReferences(Guid organizationId,Guid eventTypeId,Guid? branchId,Guid? managerId,Guid? brandProfileId,CancellationToken ct)
    {
        _=await events.GetTypeAsync(eventTypeId,organizationId,ct)??throw new NotFoundException("Event type not found.");
        if(branchId.HasValue&&await events.GetBranchAsync(branchId.Value,organizationId,ct)is null)throw new NotFoundException("Branch not found.");
        if(managerId.HasValue&&await organizations.GetMemberAsync(organizationId,managerId.Value,ct)is null)throw new UnauthorizedException("Assigned manager must belong to the organization.");
        if(brandProfileId.HasValue&&await events.GetBrandProfileAsync(brandProfileId.Value,organizationId,ct)is null)throw new NotFoundException("Brand profile not found.");
    }
    private static EventType LegacyType(Guid id)=>(id.ToString()[^1]) switch {'1'=>EventType.Wedding,'2'=>EventType.Birthday,'3'=>EventType.Corporate,'4'=>EventType.Graduation,'7'=>EventType.Festival,'8'=>EventType.BrandActivation,'a'=>EventType.Conference,_=>EventType.Private};
    private static EventMemberDto MapMember(EventMember x)=>new(x.UserId,x.User.FirstName,x.User.LastName,x.User.Email,x.Role);
    private static EventDto Map(Event x) => new(x.Id, x.OrganizationId, x.Name, x.Slug, x.Description,
        x.EventType, x.Venue, x.Address, x.Latitude, x.Longitude, x.StartDate, x.EndDate, x.CoverImage,
        x.Logo, x.PrimaryColor, x.SecondaryColor, x.Status, x.GuestLimit, x.PhotoLimit, x.StorageLimit,
        x.PublicGalleryEnabled, x.RequireGuestRegistration, x.AllowDownloads, x.AllowSocialSharing,
        x.EnableAI, x.EnableQRCode, x.PublicUrl, x.QRCodeUrl,x.EventTypeId,x.BranchId,x.AssignedManagerId,
        x.BrandProfileId,x.Timezone,x.City,x.Country,x.ContactPerson,x.ContactEmail,x.ContactPhone);
    private static EventSettingsDto MapSettings(EventSettings x) => new(x.EventId, x.Countdown, x.CaptureMode,
        x.TemplateId, x.Language, x.Watermark, x.DefaultFilter, x.PrintEnabled, x.GIFEnabled,
        x.BoomerangEnabled, x.VideoEnabled, x.AIEnabled, x.BackgroundRemovalEnabled, x.FaceDetectionEnabled);
}
