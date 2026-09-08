using EventLensAI.Application.Common;
using EventLensAI.Application.Features.CRM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EventLensAI.API.Controllers;
[ApiController,Authorize(Policy="ManageCrm"),Route("api/crm")]
public sealed class CrmController(ICrmService service):ControllerBase
{
 [HttpGet("guests")]public async Task<ActionResult<ApiResponse<PagedResult<GuestDto>>>> Guests([FromQuery]GuestSearch r,CancellationToken ct)=>Ok(ApiResponse<PagedResult<GuestDto>>.Ok(await service.SearchGuestsAsync(r,ct)));
 [HttpGet("guests/{id:guid}")]public async Task<ActionResult<ApiResponse<GuestDto>>> Guest(Guid id,CancellationToken ct)=>Ok(ApiResponse<GuestDto>.Ok(await service.GetGuestAsync(id,ct)));
 [HttpPost("guests")]public async Task<ActionResult<ApiResponse<GuestDto>>> CreateGuest(UpsertGuestRequest r,CancellationToken ct){var x=await service.CreateGuestAsync(r,HttpContext.Connection.RemoteIpAddress?.ToString(),ct);return CreatedAtAction(nameof(Guest),new{id=x.Id},ApiResponse<GuestDto>.Ok(x,"Guest created."));}
 [HttpPut("guests/{id:guid}")]public async Task<ActionResult<ApiResponse<GuestDto>>> UpdateGuest(Guid id,UpsertGuestRequest r,CancellationToken ct)=>Ok(ApiResponse<GuestDto>.Ok(await service.UpdateGuestAsync(id,r,HttpContext.Connection.RemoteIpAddress?.ToString(),ct)));
 [HttpGet("guests/{id:guid}/timeline")]public async Task<ActionResult<ApiResponse<IReadOnlyList<ActivityDto>>>> Timeline(Guid id,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<ActivityDto>>.Ok(await service.TimelineAsync(id,ct)));
 [HttpPost("check-ins")]public async Task<ActionResult<ApiResponse<object>>> CheckIn(CheckInRequest r,CancellationToken ct){await service.CheckInAsync(r,ct);return Ok(ApiResponse<object>.Ok(new{},"Guest checked in."));}
 [HttpGet("forms")]public async Task<ActionResult<ApiResponse<IReadOnlyList<LeadFormDto>>>> Forms([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<LeadFormDto>>.Ok(await service.FormsAsync(organizationId,ct)));
 [HttpPost("forms")]public async Task<ActionResult<ApiResponse<LeadFormDto>>> CreateForm(CreateLeadFormRequest r,CancellationToken ct)=>Ok(ApiResponse<LeadFormDto>.Ok(await service.CreateFormAsync(r,ct)));
 [HttpPost("forms/{id:guid}/responses")]public async Task<ActionResult<ApiResponse<object>>> Respond(Guid id,SubmitLeadResponseRequest r,CancellationToken ct){await service.SubmitFormAsync(id,r,ct);return Ok(ApiResponse<object>.Ok(new{}));}
 [HttpGet("dashboard")]public async Task<ActionResult<ApiResponse<CrmDashboardDto>>> Dashboard([FromQuery]Guid organizationId,[FromQuery]DateTime? from,[FromQuery]DateTime? to,CancellationToken ct)=>Ok(ApiResponse<CrmDashboardDto>.Ok(await service.DashboardAsync(organizationId,from,to,ct)));
 [HttpPost("segments")]public async Task<ActionResult<ApiResponse<SegmentDto>>> Segment(CreateSegmentRequest r,CancellationToken ct)=>Ok(ApiResponse<SegmentDto>.Ok(await service.CreateSegmentAsync(r,ct)));
 [HttpPost("campaigns")]public async Task<ActionResult<ApiResponse<CampaignDto>>> Campaign(CreateCampaignRequest r,CancellationToken ct)=>Ok(ApiResponse<CampaignDto>.Ok(await service.CreateCampaignAsync(r,ct)));
}
