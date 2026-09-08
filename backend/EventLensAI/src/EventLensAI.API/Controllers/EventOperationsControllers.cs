using EventLensAI.Application.Common;using EventLensAI.Application.DTOs.Events;using EventLensAI.Application.Services;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;
namespace EventLensAI.API.Controllers;
[ApiController,Authorize,Route("api/events/{eventId:guid}/venues")]public sealed class VenuesController(IVenueService service):ControllerBase
{
 [HttpGet("types")]public async Task<ActionResult<ApiResponse<IReadOnlyList<OperationTypeDto>>>>Types(Guid eventId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<OperationTypeDto>>.Ok(await service.TypesAsync(eventId,ct)));
 [HttpGet]public async Task<ActionResult<ApiResponse<IReadOnlyList<VenueDto>>>>List(Guid eventId,[FromQuery]bool archived,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<VenueDto>>.Ok(await service.ListAsync(eventId,archived,ct)));
 [HttpGet("{id:guid}")]public async Task<ActionResult<ApiResponse<VenueDto>>>Get(Guid eventId,Guid id,CancellationToken ct)=>Ok(ApiResponse<VenueDto>.Ok(await service.GetAsync(eventId,id,ct)));
 [HttpPost]public async Task<ActionResult<ApiResponse<VenueDto>>>Create(Guid eventId,CreateVenueRequest r,CancellationToken ct){var x=await service.CreateAsync(eventId,r,ct);return CreatedAtAction(nameof(Get),new{eventId,id=x.Id},ApiResponse<VenueDto>.Ok(x,"Venue created."));}
 [HttpPut("{id:guid}")]public async Task<ActionResult<ApiResponse<VenueDto>>>Update(Guid eventId,Guid id,UpdateVenueRequest r,CancellationToken ct)=>Ok(ApiResponse<VenueDto>.Ok(await service.UpdateAsync(eventId,id,r,ct),"Venue updated."));
 [HttpDelete("{id:guid}")]public async Task<ActionResult<ApiResponse<object>>>Archive(Guid eventId,Guid id,CancellationToken ct){await service.ArchiveAsync(eventId,id,ct);return Ok(ApiResponse<object>.Ok(new{},"Venue archived."));}
 [HttpPost("{id:guid}/restore")]public async Task<ActionResult<ApiResponse<VenueDto>>>Restore(Guid eventId,Guid id,CancellationToken ct)=>Ok(ApiResponse<VenueDto>.Ok(await service.RestoreAsync(eventId,id,ct),"Venue restored."));
}
[ApiController,Authorize,Route("api/events/{eventId:guid}/schedule")]public sealed class ScheduleController(IScheduleService service):ControllerBase
{
 [HttpGet("types")]public async Task<ActionResult<ApiResponse<IReadOnlyList<OperationTypeDto>>>>Types(Guid eventId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<OperationTypeDto>>.Ok(await service.TypesAsync(eventId,ct)));
 [HttpGet]public async Task<ActionResult<ApiResponse<IReadOnlyList<ScheduleDto>>>>List(Guid eventId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<ScheduleDto>>.Ok(await service.ListAsync(eventId,ct)));
 [HttpGet("calendar")]public async Task<ActionResult<ApiResponse<IReadOnlyList<CalendarEntryDto>>>>Calendar(Guid eventId,[FromQuery]DateTime?from,[FromQuery]DateTime?to,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<CalendarEntryDto>>.Ok(await service.CalendarAsync(eventId,from,to,ct)));
 [HttpPost]public async Task<ActionResult<ApiResponse<ScheduleDto>>>Create(Guid eventId,CreateScheduleRequest r,CancellationToken ct)=>Ok(ApiResponse<ScheduleDto>.Ok(await service.CreateAsync(eventId,r,ct),"Schedule item created."));
 [HttpPut("{id:guid}")]public async Task<ActionResult<ApiResponse<ScheduleDto>>>Update(Guid eventId,Guid id,UpdateScheduleRequest r,CancellationToken ct)=>Ok(ApiResponse<ScheduleDto>.Ok(await service.UpdateAsync(eventId,id,r,ct),"Schedule item updated."));
 [HttpDelete("{id:guid}")]public async Task<ActionResult<ApiResponse<object>>>Archive(Guid eventId,Guid id,CancellationToken ct){await service.ArchiveAsync(eventId,id,ct);return Ok(ApiResponse<object>.Ok(new{},"Schedule item archived."));}
 [HttpPut("reorder")]public async Task<ActionResult<ApiResponse<object>>>Reorder(Guid eventId,ReorderScheduleRequest r,CancellationToken ct){await service.ReorderAsync(eventId,r,ct);return Ok(ApiResponse<object>.Ok(new{},"Schedule reordered."));}
}
