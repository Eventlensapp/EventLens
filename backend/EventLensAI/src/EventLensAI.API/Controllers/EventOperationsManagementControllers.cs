using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Events;
using EventLensAI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventLensAI.API.Controllers;

[ApiController, Authorize, Route("api/events/{eventId:guid}/checklists")]
public sealed class EventChecklistController(IEventChecklistService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<ApiResponse<IReadOnlyList<ChecklistDto>>>> List(Guid eventId, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<ChecklistDto>>.Ok(await service.ListAsync(eventId, ct)));
    [HttpPost] public async Task<ActionResult<ApiResponse<ChecklistDto>>> Create(Guid eventId, CreateChecklistItemRequest request, CancellationToken ct) =>
        Ok(ApiResponse<ChecklistDto>.Ok(await service.CreateAsync(eventId, request, ct), "Checklist task created."));
}

[ApiController, Authorize, Route("api/checklists")]
public sealed class ChecklistItemsController(IEventChecklistService service) : ControllerBase
{
    [HttpPut("{id:guid}")] public async Task<ActionResult<ApiResponse<ChecklistDto>>> Update(Guid id, UpdateChecklistItemRequest request, CancellationToken ct) =>
        Ok(ApiResponse<ChecklistDto>.Ok(await service.UpdateAsync(id, request, ct), "Checklist task updated."));
    [HttpPost("{id:guid}/complete")] public async Task<ActionResult<ApiResponse<ChecklistDto>>> Complete(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<ChecklistDto>.Ok(await service.CompleteAsync(id, ct), "Checklist task completed."));
    [HttpDelete("{id:guid}")] public async Task<ActionResult<ApiResponse<object>>> Archive(Guid id, CancellationToken ct)
    { await service.ArchiveAsync(id, ct); return Ok(ApiResponse<object>.Ok(new { }, "Checklist task archived.")); }
}

[ApiController, Authorize, Route("api/events/{eventId:guid}/readiness")]
public sealed class EventReadinessController(IEventReadinessService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<ApiResponse<ReadinessDashboardDto>>> Get(Guid eventId, CancellationToken ct) =>
        Ok(ApiResponse<ReadinessDashboardDto>.Ok(await service.GetAsync(eventId, ct)));
}

[ApiController, Authorize, Route("api/events/{eventId:guid}/staff")]
public sealed class EventStaffController(IEventStaffService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<ApiResponse<IReadOnlyList<StaffAssignmentDto>>>> List(Guid eventId, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<StaffAssignmentDto>>.Ok(await service.ListAsync(eventId, ct)));
    [HttpPost] public async Task<ActionResult<ApiResponse<StaffAssignmentDto>>> Create(Guid eventId, CreateStaffAssignmentRequest request, CancellationToken ct) =>
        Ok(ApiResponse<StaffAssignmentDto>.Ok(await service.CreateAsync(eventId, request, ct), "Staff assigned."));
    [HttpPut("{id:guid}")] public async Task<ActionResult<ApiResponse<StaffAssignmentDto>>> Update(Guid eventId, Guid id, UpdateStaffAssignmentRequest request, CancellationToken ct) =>
        Ok(ApiResponse<StaffAssignmentDto>.Ok(await service.UpdateAsync(eventId, id, request, ct), "Staff schedule updated."));
    [HttpDelete("{id:guid}")] public async Task<ActionResult<ApiResponse<object>>> Archive(Guid eventId, Guid id, CancellationToken ct)
    { await service.ArchiveAsync(eventId, id, ct); return Ok(ApiResponse<object>.Ok(new { }, "Staff assignment removed.")); }
}

[ApiController, Authorize, Route("api")]
public sealed class EventPlacementController(IBoothPlacementService service) : ControllerBase
{
    [HttpGet("events/{eventId:guid}/zones")] public async Task<ActionResult<ApiResponse<IReadOnlyList<EventZoneDto>>>> Zones(Guid eventId, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<EventZoneDto>>.Ok(await service.ZonesAsync(eventId, ct)));
    [HttpPost("events/{eventId:guid}/zones")] public async Task<ActionResult<ApiResponse<EventZoneDto>>> CreateZone(Guid eventId, CreateEventZoneRequest request, CancellationToken ct) =>
        Ok(ApiResponse<EventZoneDto>.Ok(await service.CreateZoneAsync(eventId, request, ct), "Event zone created."));
    [HttpPut("zones/{id:guid}")] public async Task<ActionResult<ApiResponse<EventZoneDto>>> UpdateZone(Guid id, UpdateEventZoneRequest request, CancellationToken ct) =>
        Ok(ApiResponse<EventZoneDto>.Ok(await service.UpdateZoneAsync(id, request, ct), "Event zone updated."));
    [HttpDelete("zones/{id:guid}")] public async Task<ActionResult<ApiResponse<object>>> ArchiveZone(Guid id, CancellationToken ct)
    { await service.ArchiveZoneAsync(id, ct); return Ok(ApiResponse<object>.Ok(new { }, "Event zone archived.")); }
    [HttpGet("events/{eventId:guid}/placements")] public async Task<ActionResult<ApiResponse<IReadOnlyList<BoothPlacementDto>>>> List(Guid eventId, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<BoothPlacementDto>>.Ok(await service.ListAsync(eventId, ct)));
    [HttpPost("events/{eventId:guid}/placements")] public async Task<ActionResult<ApiResponse<BoothPlacementDto>>> Create(Guid eventId, CreateBoothPlacementRequest request, CancellationToken ct) =>
        Ok(ApiResponse<BoothPlacementDto>.Ok(await service.CreateAsync(eventId, request, ct), "Booth placement created."));
    [HttpPut("placements/{id:guid}")] public async Task<ActionResult<ApiResponse<BoothPlacementDto>>> Update(Guid id, UpdateBoothPlacementRequest request, CancellationToken ct) =>
        Ok(ApiResponse<BoothPlacementDto>.Ok(await service.UpdateAsync(id, request, ct), "Booth placement updated."));
    [HttpDelete("placements/{id:guid}")] public async Task<ActionResult<ApiResponse<object>>> Archive(Guid id, CancellationToken ct)
    { await service.ArchiveAsync(id, ct); return Ok(ApiResponse<object>.Ok(new { }, "Booth placement archived.")); }
}

[ApiController, Authorize, Route("api/notifications")]
public sealed class NotificationsController(INotificationService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<ApiResponse<IReadOnlyList<NotificationDto>>>> Mine(CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<NotificationDto>>.Ok(await service.MineAsync(ct)));
    [HttpPut("{id:guid}/read")] public async Task<ActionResult<ApiResponse<object>>> Read(Guid id, CancellationToken ct)
    { await service.ReadAsync(id, ct); return Ok(ApiResponse<object>.Ok(new { }, "Notification marked as read.")); }
}
