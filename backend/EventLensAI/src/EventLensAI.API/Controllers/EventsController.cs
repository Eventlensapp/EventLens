using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Events;
using EventLensAI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventLensAI.API.Controllers;

[ApiController, Authorize]
[Route("api/events")]
[Produces("application/json")]
public sealed class EventsController(IEventService service) : ControllerBase
{
    /// <summary>Searches accessible events with filtering, sorting, and pagination.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<EventDto>>>> Search(
        [FromQuery] EventSearchRequest request, CancellationToken ct) =>
        Ok(ApiResponse<PagedResult<EventDto>>.Ok(await service.SearchAsync(request, ct)));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<EventDto>>> Get(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<EventDto>.Ok(await service.GetAsync(id, ct)));

    [HttpPost("/api/organizations/{organizationId:guid}/events")]
    public async Task<ActionResult<ApiResponse<EventDto>>> Create(
        Guid organizationId, UpsertEventRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(organizationId, request, ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, ApiResponse<EventDto>.Ok(result, "Event created."));
    }

    /// <summary>Creates an event inside an organization after membership and subscription checks.</summary>
    [HttpPost]
    [Authorize(Policy="ManageEvents")]
    public async Task<ActionResult<ApiResponse<EventDto>>> Create(CreateEventRequest request,CancellationToken ct)
    {
        var result=await service.CreateAsync(request,ct);
        return CreatedAtAction(nameof(Get),new{id=result.Id},ApiResponse<EventDto>.Ok(result,"Event created."));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<EventDto>>> Update(
        Guid id, UpsertEventRequest request, CancellationToken ct) =>
        Ok(ApiResponse<EventDto>.Ok(await service.UpdateAsync(id, request, ct), "Event updated."));

    [HttpPut("{id:guid}/core")]
    [Authorize(Policy="ManageEvents")]
    public async Task<ActionResult<ApiResponse<EventDto>>>UpdateCore(Guid id,UpdateEventCoreRequest request,CancellationToken ct)=>
        Ok(ApiResponse<EventDto>.Ok(await service.UpdateCoreAsync(id,request,ct),"Event updated."));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Event deleted."));
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<ActionResult<ApiResponse<EventDto>>> Publish(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<EventDto>.Ok(await service.PublishAsync(id, ct), "Event published."));

    [HttpPost("{id:guid}/archive")]
    public async Task<ActionResult<ApiResponse<EventDto>>> Archive(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<EventDto>.Ok(await service.ArchiveAsync(id, ct), "Event archived."));

    [HttpPost("{id:guid}/restore")]
    [Authorize(Policy="ManageEvents")]
    public async Task<ActionResult<ApiResponse<EventDto>>>Restore(Guid id,CancellationToken ct)=>
        Ok(ApiResponse<EventDto>.Ok(await service.RestoreAsync(id,ct),"Event restored."));

    [HttpPut("{id:guid}/status")]
    [Authorize(Policy="ManageEvents")]
    public async Task<ActionResult<ApiResponse<EventDto>>>ChangeStatus(Guid id,ChangeEventStatusRequest request,CancellationToken ct)=>
        Ok(ApiResponse<EventDto>.Ok(await service.ChangeStatusAsync(id,request.Status,ct),"Event status changed."));

    [HttpGet("{id:guid}/dashboard")]
    public async Task<ActionResult<ApiResponse<EventDashboardDto>>>Dashboard(Guid id,CancellationToken ct)=>
        Ok(ApiResponse<EventDashboardDto>.Ok(await service.DashboardAsync(id,ct)));

    [HttpPost("{id:guid}/duplicate")]
    public async Task<ActionResult<ApiResponse<EventDto>>> Duplicate(
        Guid id, DuplicateEventRequest request, CancellationToken ct) =>
        Ok(ApiResponse<EventDto>.Ok(await service.DuplicateAsync(id, request, ct), "Event duplicated."));

    [HttpPost("{id:guid}/clone")]
    [Authorize(Policy="ManageEvents")]
    public async Task<ActionResult<ApiResponse<EventDto>>>Clone(Guid id,CloneEventRequest request,CancellationToken ct)=>
        Ok(ApiResponse<EventDto>.Ok(await service.DuplicateAsync(id,new DuplicateEventRequest(request.Name,request.StartDate,request.EndDate),ct),"Event cloned."));

    [HttpGet("{id:guid}/settings")]
    public async Task<ActionResult<ApiResponse<EventSettingsDto>>> Settings(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<EventSettingsDto>.Ok(await service.GetSettingsAsync(id, ct)));

    [HttpPut("{id:guid}/settings")]
    public async Task<ActionResult<ApiResponse<EventSettingsDto>>> UpdateSettings(
        Guid id, UpdateEventSettingsRequest request, CancellationToken ct) =>
        Ok(ApiResponse<EventSettingsDto>.Ok(await service.UpdateSettingsAsync(id, request, ct), "Settings updated."));

    [HttpPost("{id:guid}/qr")]
    public async Task<ActionResult<ApiResponse<QrCodeDto>>> GenerateQr(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<QrCodeDto>.Ok(await service.GenerateQrAsync(id, ct), "QR code generated."));

    [HttpGet("{id:guid}/qr/image")]
    [Produces("image/svg+xml")]
    public async Task<IActionResult> QrImage(Guid id, CancellationToken ct) =>
        Content(await service.GetQrSvgAsync(id, ct), "image/svg+xml");
}
