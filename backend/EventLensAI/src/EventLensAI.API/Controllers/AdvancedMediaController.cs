using EventLensAI.Application.Common;using EventLensAI.Application.Features.Booth;using EventLensAI.Domain.Enums;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;
namespace EventLensAI.API.Controllers;
[ApiController,Authorize(Policy="StartBooth"),Route("api/booth/media")]
public sealed class AdvancedMediaController(IMediaCaptureService service):ControllerBase
{
 [HttpGet("modes")]public ActionResult<ApiResponse<IReadOnlyList<CaptureModeDto>>>Modes()=>Ok(ApiResponse<IReadOnlyList<CaptureModeDto>>.Ok([new(CaptureMode.Photo,"Photo","image/jpeg",true),new(CaptureMode.GIF,"GIF","image/gif",true),new(CaptureMode.Boomerang,"Boomerang","video/webm",true),new(CaptureMode.Video,"Video","video/webm",true),new(CaptureMode.Burst,"Burst","image/jpeg",true),new(CaptureMode.TimeLapse,"Time lapse","video/webm",true),new(CaptureMode.LivePhoto,"Live photo","application/vnd.eventlens.live-photo+json",true)]));
 [HttpPost("start")]public async Task<ActionResult<ApiResponse<CapturedMediaDto>>>Start(MediaCaptureRequest request,CancellationToken ct)=>StatusCode(201,ApiResponse<CapturedMediaDto>.Ok(await service.StartAsync(request,ct),"Media capture started."));
 [HttpPost("{id:guid}/stop")]public async Task<ActionResult<ApiResponse<CapturedMediaDto>>>Stop(Guid id,StopMediaRequest request,CancellationToken ct)=>Ok(ApiResponse<CapturedMediaDto>.Ok(await service.StopAsync(id,request,ct),"Media capture completed."));
 [HttpPost("{id:guid}/cancel")]public async Task<ActionResult<ApiResponse<CapturedMediaDto>>>Cancel(Guid id,CancellationToken ct)=>Ok(ApiResponse<CapturedMediaDto>.Ok(await service.CancelAsync(id,ct),"Media capture cancelled."));
 [HttpGet("{sessionId:guid}")]public async Task<ActionResult<ApiResponse<IReadOnlyList<CapturedMediaDto>>>>List(Guid sessionId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<CapturedMediaDto>>.Ok(await service.ListAsync(sessionId,ct)));
 [HttpGet("status/{id:guid}")]public async Task<ActionResult<ApiResponse<MediaProcessingStatusDto>>>Status(Guid id,CancellationToken ct)=>Ok(ApiResponse<MediaProcessingStatusDto>.Ok(await service.StatusAsync(id,ct)));
}
