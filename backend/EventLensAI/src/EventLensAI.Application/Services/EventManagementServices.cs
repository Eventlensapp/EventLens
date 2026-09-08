using EventLensAI.Application.DTOs.Events;
using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;
using System.Text.Json;

namespace EventLensAI.Application.Services;

public sealed class EventTypeService(IEventRepository events,IOrganizationRepository organizations,ICurrentUserService current,IUnitOfWork uow):IEventTypeService
{
    public async Task<IReadOnlyList<EventTypeDto>>ListAsync(Guid? organizationId,CancellationToken ct)
    {
        if(organizationId.HasValue&&!current.Roles.Contains(SystemRoles.SuperAdmin))
            _=await organizations.GetMemberAsync(organizationId.Value,UserId,ct)??throw new UnauthorizedException("Organization access denied.");
        return (await events.ListTypesAsync(organizationId,ct)).Select(Map).ToArray();
    }
    public async Task<EventTypeDto>CreateAsync(CreateEventTypeRequest request,CancellationToken ct)
    {
        await Manage(request.OrganizationId,ct);
        if(await events.TypeNameExistsAsync(request.OrganizationId,request.Name.Trim(),null,ct))throw new ConflictException("An event type with this name already exists.");
        var x=new EventTypeDefinition(request.Name,request.Description,request.Icon,request.Color,false,request.OrganizationId);
        await events.AddTypeAsync(x,ct);await uow.SaveChangesAsync(ct);return Map(x);
    }
    public async Task<EventTypeDto>UpdateAsync(Guid id,UpdateEventTypeRequest request,CancellationToken ct)
    {
        var x=await events.GetTypeIncludingDeletedAsync(id,ct)??throw new NotFoundException("Event type not found.");
        if(x.OrganizationId.HasValue)await Manage(x.OrganizationId.Value,ct);
        if(await events.TypeNameExistsAsync(x.OrganizationId,request.Name.Trim(),id,ct))throw new ConflictException("An event type with this name already exists.");
        x.Update(request.Name,request.Description,request.Icon,request.Color,request.IsActive);x.MarkUpdated(UserId);await uow.SaveChangesAsync(ct);return Map(x);
    }
    public async Task ArchiveAsync(Guid id,CancellationToken ct)
    {
        var x=await events.GetTypeIncludingDeletedAsync(id,ct)??throw new NotFoundException("Event type not found.");
        if(x.OrganizationId.HasValue)await Manage(x.OrganizationId.Value,ct);x.Archive(UserId);await uow.SaveChangesAsync(ct);
    }
    private async Task Manage(Guid id,CancellationToken ct){if(current.Roles.Contains(SystemRoles.SuperAdmin))return;var m=await organizations.GetMemberAsync(id,UserId,ct)??throw new UnauthorizedException("Organization access denied.");if(m.Role.Name is not(SystemRoles.Owner or SystemRoles.Manager))throw new UnauthorizedException("Event type management permission denied.");}
    private static EventTypeDto Map(EventTypeDefinition x)=>new(x.Id,x.Name,x.Description,x.Icon,x.Color,x.IsSystemType,x.IsActive,x.OrganizationId);
    private Guid UserId=>current.UserId??throw new UnauthorizedException("Authentication is required.");
}

public sealed class EventTemplateService(IEventRepository events,IOrganizationRepository organizations,ICurrentUserService current,IUnitOfWork uow):IEventTemplateService
{
    public async Task<IReadOnlyList<EventTemplateDto>>ListAsync(Guid o,CancellationToken ct){await Access(o,false,ct);return(await events.ListTemplatesAsync(o,ct)).Select(Map).ToArray();}
    public async Task<EventTemplateDto>GetAsync(Guid id,CancellationToken ct){var x=await Entity(id,ct);await Access(x.OrganizationId,false,ct);return Map(x);}
    public async Task<EventTemplateDto>CreateAsync(CreateEventTemplateRequest r,CancellationToken ct)
    {
        await Access(r.OrganizationId,true,ct);await Validate(r.OrganizationId,r.EventTypeId,r.DefaultBrandProfileId,ct);
        if(await events.TemplateNameExistsAsync(r.OrganizationId,r.Name.Trim(),null,ct))throw new ConflictException("A template with this name already exists.");
        var x=new EventTemplate(r.OrganizationId,r.Name,r.Description,r.EventTypeId,r.DefaultBrandProfileId,TimeSpan.FromMinutes(r.DefaultDurationMinutes),JsonSerializer.Serialize(r.Configuration));
        await events.AddTemplateAsync(x,ct);await uow.SaveChangesAsync(ct);return await GetAsync(x.Id,ct);
    }
    public async Task<EventTemplateDto>UpdateAsync(Guid id,UpdateEventTemplateRequest r,CancellationToken ct)
    {
        var x=await Entity(id,ct);if(x.IsSystemTemplate)throw new ConflictException("System templates are read-only.");await Access(x.OrganizationId,true,ct);
        await Validate(x.OrganizationId,r.EventTypeId,r.DefaultBrandProfileId,ct);
        if(await events.TemplateNameExistsAsync(x.OrganizationId,r.Name.Trim(),id,ct))throw new ConflictException("A template with this name already exists.");
        x.Update(r.Name,r.Description,r.EventTypeId,r.DefaultBrandProfileId,TimeSpan.FromMinutes(r.DefaultDurationMinutes),JsonSerializer.Serialize(r.Configuration),r.IsActive);
        x.MarkUpdated(UserId);await uow.SaveChangesAsync(ct);return await GetAsync(id,ct);
    }
    public async Task ArchiveAsync(Guid id,CancellationToken ct){var x=await Entity(id,ct);if(x.IsSystemTemplate)throw new ConflictException("System templates cannot be archived.");await Access(x.OrganizationId,true,ct);x.Archive(UserId);await uow.SaveChangesAsync(ct);}
    public async Task<EventTemplateDto>CloneAsync(Guid id,CloneEventTemplateRequest r,CancellationToken ct)
    {
        var source=await Entity(id,ct);await Access(source.OrganizationId,true,ct);
        if(await events.TemplateNameExistsAsync(source.OrganizationId,r.Name.Trim(),null,ct))throw new ConflictException("A template with this name already exists.");
        var copy=source.Duplicate(r.Name,source.OrganizationId);await events.AddTemplateAsync(copy,ct);await uow.SaveChangesAsync(ct);return await GetAsync(copy.Id,ct);
    }
    private async Task Validate(Guid o,Guid type,Guid?brand,CancellationToken ct){_=await events.GetTypeAsync(type,o,ct)??throw new NotFoundException("Event type not found.");if(brand.HasValue&&await events.GetBrandProfileAsync(brand.Value,o,ct)is null)throw new NotFoundException("Brand profile not found.");}
    private async Task<EventTemplate>Entity(Guid id,CancellationToken ct)=>await events.GetTemplateAsync(id,ct)??throw new NotFoundException("Event template not found.");
    private async Task Access(Guid o,bool manage,CancellationToken ct){if(current.Roles.Contains(SystemRoles.SuperAdmin))return;var m=await organizations.GetMemberAsync(o,UserId,ct)??throw new UnauthorizedException("Template access denied.");if(manage&&m.Role.Name is not(SystemRoles.Owner or SystemRoles.Manager))throw new UnauthorizedException("Template management permission denied.");}
    private Guid UserId=>current.UserId??throw new UnauthorizedException("Authentication is required.");
    private static EventTemplateDto Map(EventTemplate x)=>new(x.Id,x.OrganizationId,x.Name,x.Description,x.EventTypeId,x.EventType.Name,x.DefaultBrandProfileId,x.DefaultBrandProfile?.Name,(int)x.DefaultDuration.TotalMinutes,JsonSerializer.Deserialize<EventTemplateConfiguration>(x.ConfigurationJson)??new(false,false,false,false,false),x.IsSystemTemplate,x.IsActive,x.CreatedAt);
}

public sealed class EventMemberService(IEventRepository events,IOrganizationRepository organizations,
    ICurrentUserService current,IUnitOfWork uow):IEventMemberService
{
    public async Task<IReadOnlyList<EventMemberDto>>ListAsync(Guid eventId,CancellationToken ct)
    {var e=await Read(eventId,ct);await Access(e.OrganizationId,false,ct);return(await events.ListMembersAsync(eventId,ct)).Select(Map).ToArray();}
    public async Task<EventMemberDto>AssignAsync(Guid eventId,AssignEventMemberRequest request,CancellationToken ct)
    {
        var e=await Read(eventId,ct);await Access(e.OrganizationId,true,ct);
        var member=await organizations.GetMemberAsync(e.OrganizationId,request.UserId,ct)??throw new UnauthorizedException("User must belong to the event organization.");
        if(await events.GetMemberAsync(eventId,request.UserId,ct)is not null)throw new ConflictException("User is already assigned to this event.");
        var created=new EventMember(eventId,member.UserId,request.Role,UserId);await events.AddMemberAsync(created,ct);await uow.SaveChangesAsync(ct);
        return new(created.UserId,member.User.FirstName,member.User.LastName,member.User.Email,created.Role);
    }
    public async Task<EventMemberDto>ChangeRoleAsync(Guid eventId,Guid userId,ChangeEventMemberRoleRequest request,CancellationToken ct)
    {
        var e=await Read(eventId,ct);await Access(e.OrganizationId,true,ct);
        var member=await events.GetMemberAsync(eventId,userId,ct)??throw new NotFoundException("Event member not found.");
        member.ChangeRole(request.Role,UserId);await uow.SaveChangesAsync(ct);return Map(member);
    }
    public async Task RemoveAsync(Guid eventId,Guid userId,CancellationToken ct)
    {
        var e=await Read(eventId,ct);await Access(e.OrganizationId,true,ct);
        var member=await events.GetMemberAsync(eventId,userId,ct)??throw new NotFoundException("Event member not found.");
        if(member.Role==EventMemberRole.EventOwner&&(await events.ListMembersAsync(eventId,ct)).Count(x=>x.Role==EventMemberRole.EventOwner)==1)
            throw new ConflictException("An event must retain at least one Event Owner.");
        events.RemoveMember(member);await uow.SaveChangesAsync(ct);
    }
    private async Task<Event>Read(Guid id,CancellationToken ct)=>await events.GetAsync(id,ct)??throw new NotFoundException("Event not found.");
    private async Task Access(Guid organizationId,bool manage,CancellationToken ct)
    {
        if(current.Roles.Contains(SystemRoles.SuperAdmin))return;
        var m=await organizations.GetMemberAsync(organizationId,UserId,ct)??throw new UnauthorizedException("Event access denied.");
        if(manage&&m.Role.Name is not(SystemRoles.Owner or SystemRoles.Manager))throw new UnauthorizedException("Event management permission denied.");
    }
    private Guid UserId=>current.UserId??throw new UnauthorizedException("Authentication is required.");
    private static EventMemberDto Map(EventMember x)=>new(x.UserId,x.User.FirstName,x.User.LastName,x.User.Email,x.Role);
}
