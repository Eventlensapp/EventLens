using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Events;
using EventLensAI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventLensAI.API.Controllers;

[ApiController,Authorize,Route("api/events/{eventId:guid}/members"),Produces("application/json")]
public sealed class EventMembersController(IEventMemberService service):ControllerBase
{
    [HttpGet]public async Task<ActionResult<ApiResponse<IReadOnlyList<EventMemberDto>>>>List(Guid eventId,CancellationToken ct)=>
        Ok(ApiResponse<IReadOnlyList<EventMemberDto>>.Ok(await service.ListAsync(eventId,ct)));
    [HttpPost,Authorize(Policy="ManageEvents")]public async Task<ActionResult<ApiResponse<EventMemberDto>>>Assign(Guid eventId,AssignEventMemberRequest request,CancellationToken ct)=>
        Ok(ApiResponse<EventMemberDto>.Ok(await service.AssignAsync(eventId,request,ct),"Member assigned."));
    [HttpPut("{userId:guid}"),Authorize(Policy="ManageEvents")]public async Task<ActionResult<ApiResponse<EventMemberDto>>>Change(Guid eventId,Guid userId,ChangeEventMemberRoleRequest request,CancellationToken ct)=>
        Ok(ApiResponse<EventMemberDto>.Ok(await service.ChangeRoleAsync(eventId,userId,request,ct),"Event role changed."));
    [HttpDelete("{userId:guid}"),Authorize(Policy="ManageEvents")]public async Task<ActionResult<ApiResponse<object>>>Remove(Guid eventId,Guid userId,CancellationToken ct)
    {await service.RemoveAsync(eventId,userId,ct);return Ok(ApiResponse<object>.Ok(new{},"Member removed."));}
}
