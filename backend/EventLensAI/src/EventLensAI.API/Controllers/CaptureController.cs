using EventLensAI.Application.Common;
using EventLensAI.Application.Features.Booth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EventLensAI.API.Controllers;
[ApiController,Authorize(Policy="StartBooth"),Route("api/booth"),Produces("application/json")]
public sealed class CaptureController(ICaptureService capture):ControllerBase
{
 [HttpGet("capture/configuration")]public async Task<ActionResult<ApiResponse<CaptureConfigurationDto>>>GetConfiguration([FromQuery]Guid organizationId,[FromQuery]Guid eventId,CancellationToken ct)=>Ok(ApiResponse<CaptureConfigurationDto>.Ok(await capture.GetConfigurationAsync(organizationId,eventId,ct)));
 [HttpPut("capture/configuration")]public async Task<ActionResult<ApiResponse<CaptureConfigurationDto>>>PutConfiguration(UpdateCaptureConfigurationRequest request,CancellationToken ct)=>Ok(ApiResponse<CaptureConfigurationDto>.Ok(await capture.UpdateConfigurationAsync(request,ct),"Capture configuration saved."));
 [HttpPost("capture/start")]public async Task<ActionResult<ApiResponse<CapturedPhotoDto>>>Start(StartCaptureRequest request,CancellationToken ct)=>Ok(ApiResponse<CapturedPhotoDto>.Ok(await capture.RegisterAsync(request,ct),"Capture metadata saved."));
 [HttpPost("capture/cancel")]public async Task<ActionResult<ApiResponse<CaptureStatusDto>>>Cancel(CancelCaptureRequest request,CancellationToken ct)=>Ok(ApiResponse<CaptureStatusDto>.Ok(await capture.CancelAsync(request,ct),"Capture cancelled."));
 [HttpGet("capture/status")]public async Task<ActionResult<ApiResponse<CaptureStatusDto>>>Status([FromQuery]Guid organizationId,[FromQuery]Guid sessionId,CancellationToken ct)=>Ok(ApiResponse<CaptureStatusDto>.Ok(await capture.GetStatusAsync(organizationId,sessionId,ct)));
 [HttpGet("captures/session/{sessionId:guid}")]public async Task<ActionResult<ApiResponse<IReadOnlyList<CapturedPhotoDto>>>>Session(Guid sessionId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<CapturedPhotoDto>>.Ok(await capture.ListAsync(sessionId,ct)));
}
