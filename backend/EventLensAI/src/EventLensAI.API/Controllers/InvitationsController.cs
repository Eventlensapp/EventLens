using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Organizations;
using EventLensAI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EventLensAI.API.Controllers;
[ApiController,Route("api/invitations")]
public sealed class InvitationsController(IOrganizationService service):ControllerBase
{
    [Authorize,HttpGet("organization/{organizationId:guid}")] public async Task<ActionResult<ApiResponse<IReadOnlyList<InvitationDto>>>> List(Guid organizationId,CancellationToken ct)=>
        Ok(ApiResponse<IReadOnlyList<InvitationDto>>.Ok(await service.ListInvitationsAsync(organizationId,ct)));
    [Authorize,HttpPost("organization/{organizationId:guid}")] public async Task<ActionResult<ApiResponse<InvitationResponse>>> Create(Guid organizationId,InviteMemberRequest request,CancellationToken ct)=>
        Ok(ApiResponse<InvitationResponse>.Ok(await service.InviteAsync(organizationId,request,ct),"Invitation sent."));
    [AllowAnonymous,HttpGet("{token}")] public async Task<ActionResult<ApiResponse<InvitationDto>>> Get(string token,CancellationToken ct)=>
        Ok(ApiResponse<InvitationDto>.Ok(await service.GetInvitationAsync(token,ct)));
    [Authorize,HttpPost("{token}/accept")] public async Task<ActionResult<ApiResponse<object>>> Accept(string token,CancellationToken ct)
    {await service.AcceptInvitationAsync(token,ct);return Ok(ApiResponse<object>.Ok(new{},"Invitation accepted."));}
    [Authorize,HttpDelete("{id:guid}")] public async Task<ActionResult<ApiResponse<object>>> Cancel(Guid id,CancellationToken ct)
    {await service.CancelInvitationAsync(id,ct);return Ok(ApiResponse<object>.Ok(new{},"Invitation cancelled."));}
}
