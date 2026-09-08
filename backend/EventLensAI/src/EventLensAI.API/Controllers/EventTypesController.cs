using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Events;
using EventLensAI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventLensAI.API.Controllers;

[ApiController,Authorize,Route("api/event-types"),Produces("application/json")]
public sealed class EventTypesController(IEventTypeService service):ControllerBase
{
    /// <summary>Lists active system event types and optional organization-specific types.</summary>
    [HttpGet]public async Task<ActionResult<ApiResponse<IReadOnlyList<EventTypeDto>>>>List([FromQuery]Guid? organizationId,CancellationToken ct)=>
        Ok(ApiResponse<IReadOnlyList<EventTypeDto>>.Ok(await service.ListAsync(organizationId,ct)));
    [HttpPost,Authorize(Policy="ManageEvents")]public async Task<ActionResult<ApiResponse<EventTypeDto>>>Create(CreateEventTypeRequest request,CancellationToken ct)
    {var x=await service.CreateAsync(request,ct);return CreatedAtAction(nameof(List),new{organizationId=request.OrganizationId},ApiResponse<EventTypeDto>.Ok(x,"Event type created."));}
    [HttpPut("{id:guid}"),Authorize(Policy="ManageEvents")]public async Task<ActionResult<ApiResponse<EventTypeDto>>>Update(Guid id,UpdateEventTypeRequest request,CancellationToken ct)=>
        Ok(ApiResponse<EventTypeDto>.Ok(await service.UpdateAsync(id,request,ct),"Event type updated."));
    [HttpDelete("{id:guid}"),Authorize(Policy="ManageEvents")]public async Task<ActionResult<ApiResponse<object>>>Archive(Guid id,CancellationToken ct)
    {await service.ArchiveAsync(id,ct);return Ok(ApiResponse<object>.Ok(new{},"Event type archived."));}
}
