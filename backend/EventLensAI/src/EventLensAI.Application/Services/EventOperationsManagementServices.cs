using EventLensAI.Application.DTOs.Events;
using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Application.Services;

public sealed class EventChecklistService(
    IEventRepository repository,
    IOrganizationRepository organizations,
    ICurrentUserService current,
    IUnitOfWork unitOfWork) : EventOperationsBase(organizations, current), IEventChecklistService
{
    protected override IEventRepository Repo => repository;

    public async Task<IReadOnlyList<ChecklistDto>> ListAsync(Guid eventId, CancellationToken ct)
    {
        await Access(eventId, false, ct);
        var items = await repository.ListChecklistItemsAsync(eventId, ct);
        var member = await Organizations.GetMemberAsync((await repository.GetAsync(eventId, ct))!.OrganizationId, UserId, ct);
        if (member?.Role.Name == SystemRoles.BoothOperator)
            items = items.Where(x => x.AssignedUserId == UserId).ToArray();
        return items.Select(Map).ToArray();
    }

    public async Task<ChecklistDto> CreateAsync(Guid eventId, CreateChecklistItemRequest request, CancellationToken ct)
    {
        var eventEntity = await Access(eventId, true, ct);
        await ValidateAssignee(eventEntity.OrganizationId, request.AssignedUserId, ct);
        var checklist = await repository.GetChecklistAsync(eventId, ct);
        if (checklist is null)
        {
            checklist = new EventChecklist(eventId, "Event preparation");
            await repository.AddChecklistAsync(checklist, ct);
        }
        var item = new EventChecklistItem(checklist.Id, eventId, request.Title, request.Category, request.DisplayOrder);
        item.Update(request.Title, request.Description, request.Category, request.AssignedUserId, request.DueDate, request.Priority, ChecklistItemStatus.Pending, request.DisplayOrder);
        await repository.AddChecklistItemAsync(item, ct);
        if (request.AssignedUserId.HasValue)
            await AddNotification(eventEntity.OrganizationId, request.AssignedUserId.Value, eventId, "Task assigned", request.Title, EventNotificationType.TaskAssigned, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return (await repository.ListChecklistItemsAsync(eventId, ct)).Where(x => x.Id == item.Id).Select(Map).Single();
    }

    public async Task<ChecklistDto> UpdateAsync(Guid id, UpdateChecklistItemRequest request, CancellationToken ct)
    {
        var item = await repository.GetChecklistItemAsync(id, ct) ?? throw new NotFoundException("Checklist item not found.");
        var eventEntity = await Access(item.EventId, true, ct);
        await ValidateAssignee(eventEntity.OrganizationId, request.AssignedUserId, ct);
        var newlyAssigned = request.AssignedUserId.HasValue && request.AssignedUserId != item.AssignedUserId;
        item.Update(request.Title, request.Description, request.Category, request.AssignedUserId, request.DueDate, request.Priority, request.Status, request.DisplayOrder);
        item.MarkUpdated(UserId);
        if (newlyAssigned)
            await AddNotification(eventEntity.OrganizationId, request.AssignedUserId!.Value, item.EventId, "Task assigned", request.Title, EventNotificationType.TaskAssigned, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Map(item);
    }

    public async Task<ChecklistDto> CompleteAsync(Guid id, CancellationToken ct)
    {
        var item = await repository.GetChecklistItemAsync(id, ct) ?? throw new NotFoundException("Checklist item not found.");
        var eventEntity = await Access(item.EventId, true, ct);
        item.Complete(UserId);
        item.MarkUpdated(UserId);
        if (item.AssignedUserId.HasValue)
            await AddNotification(eventEntity.OrganizationId, item.AssignedUserId.Value, item.EventId, "Task completed", item.Title, EventNotificationType.TaskCompleted, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Map(item);
    }

    public async Task ArchiveAsync(Guid id, CancellationToken ct)
    {
        var item = await repository.GetChecklistItemAsync(id, ct) ?? throw new NotFoundException("Checklist item not found.");
        await Access(item.EventId, true, ct);
        item.Archive(UserId);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private async Task ValidateAssignee(Guid organizationId, Guid? userId, CancellationToken ct)
    {
        if (userId.HasValue && await Organizations.GetMemberAsync(organizationId, userId.Value, ct) is null)
            throw new UnauthorizedException("Assigned user must belong to the event organization.");
    }

    private async Task AddNotification(Guid organizationId, Guid userId, Guid eventId, string title, string message, EventNotificationType type, CancellationToken ct) =>
        await repository.AddNotificationAsync(new EventNotification(organizationId, userId, eventId, title, message, type), ct);

    private static ChecklistDto Map(EventChecklistItem x) => new(x.Id, x.ChecklistId, x.EventId, x.Title, x.Description, x.Category,
        x.AssignedUserId, x.AssignedUser is null ? null : $"{x.AssignedUser.FirstName} {x.AssignedUser.LastName}".Trim(),
        x.DueDate, x.Priority, x.Status, x.CompletedBy, x.CompletedAt, x.DisplayOrder);
}

public sealed class EventReadinessService(
    IEventRepository repository,
    IOrganizationRepository organizations,
    ICurrentUserService current) : EventOperationsBase(organizations, current), IEventReadinessService
{
    protected override IEventRepository Repo => repository;
    public async Task<ReadinessDashboardDto> GetAsync(Guid eventId, CancellationToken ct)
    {
        await Access(eventId, false, ct);
        return Calculate(await repository.ListChecklistItemsAsync(eventId, ct), await repository.ListStaffAsync(eventId, ct),
            await repository.ListPlacementsAsync(eventId, ct));
    }

    public static ReadinessDashboardDto Calculate(IEnumerable<EventChecklistItem> items, IEnumerable<EventStaffAssignment> staff, IEnumerable<BoothPlacement> placements)
    {
        var active = items.Where(x => x.Status != ChecklistItemStatus.Cancelled).ToArray();
        var completed = active.Count(x => x.Status == ChecklistItemStatus.Completed);
        var pending = active.Count(x => x.Status is ChecklistItemStatus.Pending or ChecklistItemStatus.InProgress);
        var blocked = active.Count(x => x.Status == ChecklistItemStatus.Blocked);
        var critical = active.Count(x => x.Priority == ChecklistPriority.Critical && x.Status != ChecklistItemStatus.Completed);
        var score = active.Length == 0 ? 0 : (int)Math.Round(completed * 100d / active.Length);
        return new(score, active.Length, completed, pending, blocked, critical, staff.Count(),
            placements.Count(x => x.Status is BoothPlacementStatus.Ready or BoothPlacementStatus.Active));
    }
}

public sealed class EventStaffService(
    IEventRepository repository,
    IOrganizationRepository organizations,
    ICurrentUserService current,
    IUnitOfWork unitOfWork) : EventOperationsBase(organizations, current), IEventStaffService
{
    protected override IEventRepository Repo => repository;
    public async Task<IReadOnlyList<StaffAssignmentDto>> ListAsync(Guid eventId, CancellationToken ct)
    {
        await Access(eventId, false, ct);
        return (await repository.ListStaffAsync(eventId, ct)).Select(Map).ToArray();
    }
    public async Task<StaffAssignmentDto> CreateAsync(Guid eventId, CreateStaffAssignmentRequest request, CancellationToken ct)
    {
        var e = await Access(eventId, true, ct);
        await Member(e.OrganizationId, request.UserId, ct);
        var assignment = new EventStaffAssignment(eventId, request.UserId, request.Role, request.StartDateTime, request.EndDateTime);
        assignment.Update(request.Role, request.StartDateTime, request.EndDateTime, StaffAssignmentStatus.Scheduled, request.Notes);
        await repository.AddStaffAsync(assignment, ct);
        await repository.AddNotificationAsync(new EventNotification(e.OrganizationId, request.UserId, eventId, "Event staff assignment", $"You were assigned as {request.Role}.", EventNotificationType.StaffAssigned), ct);
        await unitOfWork.SaveChangesAsync(ct);
        return (await repository.ListStaffAsync(eventId, ct)).Where(x => x.Id == assignment.Id).Select(Map).Single();
    }
    public async Task<StaffAssignmentDto> UpdateAsync(Guid eventId, Guid id, UpdateStaffAssignmentRequest request, CancellationToken ct)
    {
        await Access(eventId, true, ct);
        var x = await repository.GetStaffAsync(id, ct);
        if (x is null || x.EventId != eventId) throw new NotFoundException("Staff assignment not found.");
        x.Update(request.Role, request.StartDateTime, request.EndDateTime, request.Status, request.Notes);
        x.MarkUpdated(UserId);
        await unitOfWork.SaveChangesAsync(ct);
        return Map(x);
    }
    public async Task ArchiveAsync(Guid eventId, Guid id, CancellationToken ct)
    {
        await Access(eventId, true, ct);
        var x = await repository.GetStaffAsync(id, ct);
        if (x is null || x.EventId != eventId) throw new NotFoundException("Staff assignment not found.");
        x.Archive(UserId);
        await unitOfWork.SaveChangesAsync(ct);
    }
    private async Task Member(Guid organizationId, Guid userId, CancellationToken ct) =>
        _ = await Organizations.GetMemberAsync(organizationId, userId, ct) ?? throw new UnauthorizedException("Staff user must belong to the event organization.");
    private static StaffAssignmentDto Map(EventStaffAssignment x) => new(x.Id, x.EventId, x.UserId,
        $"{x.User.FirstName} {x.User.LastName}".Trim(), x.Role, x.StartDateTime, x.EndDateTime, x.Status, x.Notes);
}

public sealed class BoothPlacementService(
    IEventRepository repository,
    IOrganizationRepository organizations,
    ICurrentUserService current,
    IUnitOfWork unitOfWork) : EventOperationsBase(organizations, current), IBoothPlacementService
{
    protected override IEventRepository Repo => repository;
    public async Task<IReadOnlyList<EventZoneDto>> ZonesAsync(Guid eventId, CancellationToken ct)
    {
        await Access(eventId, false, ct);
        return (await repository.ListZonesAsync(eventId, ct)).Select(MapZone).ToArray();
    }
    public async Task<EventZoneDto> CreateZoneAsync(Guid eventId, CreateEventZoneRequest request, CancellationToken ct)
    {
        await Access(eventId, true, ct);
        await Venue(eventId, request.VenueId, ct);
        var zone = new EventZone(request.VenueId, request.Name, request.Description, request.Floor, request.Capacity);
        await repository.AddZoneAsync(zone, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return (await repository.ListZonesAsync(eventId, ct)).Where(x => x.Id == zone.Id).Select(MapZone).Single();
    }
    public async Task<EventZoneDto> UpdateZoneAsync(Guid id, UpdateEventZoneRequest request, CancellationToken ct)
    {
        var zone = await repository.GetZoneAsync(id, ct) ?? throw new NotFoundException("Event zone not found.");
        await Access(zone.Venue.EventId, true, ct);
        zone.Update(request.Name, request.Description, request.Floor, request.Capacity);
        zone.MarkUpdated(UserId);
        await unitOfWork.SaveChangesAsync(ct);
        return MapZone(zone);
    }
    public async Task ArchiveZoneAsync(Guid id, CancellationToken ct)
    {
        var zone = await repository.GetZoneAsync(id, ct) ?? throw new NotFoundException("Event zone not found.");
        await Access(zone.Venue.EventId, true, ct);
        zone.Archive(UserId);
        await unitOfWork.SaveChangesAsync(ct);
    }
    public async Task<IReadOnlyList<BoothPlacementDto>> ListAsync(Guid eventId, CancellationToken ct)
    {
        await Access(eventId, false, ct);
        return (await repository.ListPlacementsAsync(eventId, ct)).Select(MapPlacement).ToArray();
    }
    public async Task<BoothPlacementDto> CreateAsync(Guid eventId, CreateBoothPlacementRequest request, CancellationToken ct)
    {
        var e = await Access(eventId, true, ct);
        await ValidatePlacement(e, request.VenueId, request.ZoneId, request.AssignedOperatorId, ct);
        var x = new BoothPlacement(eventId, request.VenueId, request.ZoneId, request.Name);
        x.Update(request.VenueId, request.ZoneId, request.Name, request.Description, request.PositionNotes, request.AssignedOperatorId, request.Status);
        await repository.AddPlacementAsync(x, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return (await repository.ListPlacementsAsync(eventId, ct)).Where(y => y.Id == x.Id).Select(MapPlacement).Single();
    }
    public async Task<BoothPlacementDto> UpdateAsync(Guid id, UpdateBoothPlacementRequest request, CancellationToken ct)
    {
        var x = await repository.GetPlacementAsync(id, ct) ?? throw new NotFoundException("Booth placement not found.");
        var e = await Access(x.EventId, true, ct);
        await ValidatePlacement(e, request.VenueId, request.ZoneId, request.AssignedOperatorId, ct);
        x.Update(request.VenueId, request.ZoneId, request.Name, request.Description, request.PositionNotes, request.AssignedOperatorId, request.Status);
        x.MarkUpdated(UserId);
        await unitOfWork.SaveChangesAsync(ct);
        return MapPlacement(x);
    }
    public async Task ArchiveAsync(Guid id, CancellationToken ct)
    {
        var x = await repository.GetPlacementAsync(id, ct) ?? throw new NotFoundException("Booth placement not found.");
        await Access(x.EventId, true, ct);
        x.Archive(UserId);
        await unitOfWork.SaveChangesAsync(ct);
    }
    private async Task ValidatePlacement(Event e, Guid venueId, Guid zoneId, Guid? operatorId, CancellationToken ct)
    {
        await Venue(e.Id, venueId, ct);
        var zone = await repository.GetZoneAsync(zoneId, ct);
        if (zone is null || zone.VenueId != venueId || zone.Venue.EventId != e.Id) throw new NotFoundException("Zone does not belong to the selected event venue.");
        if (operatorId.HasValue && await Organizations.GetMemberAsync(e.OrganizationId, operatorId.Value, ct) is null)
            throw new UnauthorizedException("Assigned operator must belong to the event organization.");
    }
    private async Task Venue(Guid eventId, Guid venueId, CancellationToken ct)
    {
        var venue = await repository.GetVenueAsync(venueId, false, ct);
        if (venue is null || venue.EventId != eventId) throw new NotFoundException("Venue not found.");
    }
    private static EventZoneDto MapZone(EventZone x) => new(x.Id, x.VenueId, x.Venue.Name, x.Name, x.Description, x.Floor, x.Capacity);
    private static BoothPlacementDto MapPlacement(BoothPlacement x) => new(x.Id, x.EventId, x.VenueId, x.Venue.Name, x.ZoneId, x.Zone.Name,
        x.Name, x.Description, x.PositionNotes, x.AssignedOperatorId,
        x.AssignedOperator is null ? null : $"{x.AssignedOperator.FirstName} {x.AssignedOperator.LastName}".Trim(), x.Status);
}

public sealed class NotificationService(
    IEventRepository repository,
    ICurrentUserService current,
    IUnitOfWork unitOfWork) : INotificationService
{
    private Guid UserId => current.UserId ?? throw new UnauthorizedException("Authentication required.");
    public async Task NotifyAsync(Guid organizationId, Guid userId, Guid? eventId, string title, string message, EventNotificationType type, CancellationToken ct)
    {
        await repository.AddNotificationAsync(new EventNotification(organizationId, userId, eventId, title, message, type), ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
    public async Task<IReadOnlyList<NotificationDto>> MineAsync(CancellationToken ct) =>
        (await repository.ListNotificationsAsync(UserId, ct)).Select(x => new NotificationDto(x.Id, x.OrganizationId, x.UserId, x.EventId, x.Title, x.Message, x.Type, x.IsRead, x.CreatedAt)).ToArray();
    public async Task ReadAsync(Guid id, CancellationToken ct)
    {
        var x = await repository.GetNotificationAsync(id, ct);
        if (x is null || x.UserId != UserId) throw new NotFoundException("Notification not found.");
        x.Read();
        x.MarkUpdated(UserId);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
