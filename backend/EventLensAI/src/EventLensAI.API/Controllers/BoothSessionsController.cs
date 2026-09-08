using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Booth;
using EventLensAI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventLensAI.Application.Features.Booth;
using Microsoft.AspNetCore.RateLimiting;

namespace EventLensAI.API.Controllers;

[ApiController]
[Route("api/booth")]
[Produces("application/json")]
public sealed class BoothSessionsController(IBoothSessionService service,IBoothRuntimeService runtime,ISessionRecoveryService recovery,ISessionActivityService activity) : ControllerBase
{
    [AllowAnonymous, HttpGet("events/{eventSlug}")]
    public async Task<ActionResult<ApiResponse<BoothBootstrapDto>>> Bootstrap(string eventSlug, CancellationToken ct) =>
        Ok(ApiResponse<BoothBootstrapDto>.Ok(await service.BootstrapAsync(eventSlug, ct)));

    [Authorize(Policy = "StartBooth"),EnableRateLimiting("public-event"), HttpPost("sessions")]
    public async Task<ActionResult<ApiResponse<BoothSessionDto>>> Start(BoothSessionCreateEnvelope request, CancellationToken ct)
    {
        var result=request.OrganizationId.HasValue
            ?await service.CreateManagedAsync(new CreateSessionRequest(request.OrganizationId.Value,request.EventId,request.BoothId!.Value,request.SessionType!.Value,request.GuestName,request.GuestEmail,request.DeviceInformation??"",request.MetadataJson??"{}",request.GuestId,request.CaptureMode,request.Countdown,request.TemplateId),ct)
            :await service.StartAsync(new StartBoothSessionRequest(request.EventId,request.GuestId,request.CaptureMode!.Value,request.Countdown!.Value,request.TemplateId),ct);
        return CreatedAtAction(nameof(Get), new { sessionId = result.SessionId }, ApiResponse<BoothSessionDto>.Ok(result, "Booth session started."));
    }

    [Authorize(Policy = "StartBooth"), HttpGet("sessions/{sessionId:guid}")]
    public async Task<ActionResult<ApiResponse<BoothSessionDto>>> Get(Guid sessionId, CancellationToken ct) =>
        Ok(ApiResponse<BoothSessionDto>.Ok(await service.GetAsync(sessionId, ct)));

    [Authorize(Policy = "StartBooth"), HttpPost("sessions/{sessionId:guid}/capture")]
    public async Task<ActionResult<ApiResponse<BoothSessionDto>>> BeginCapture(Guid sessionId, CancellationToken ct) =>
        Ok(ApiResponse<BoothSessionDto>.Ok(await service.BeginCaptureAsync(sessionId, ct)));

    [Authorize(Policy = "StartBooth"), HttpPost("sessions/{sessionId:guid}/captures")]
    public async Task<ActionResult<ApiResponse<BoothSessionDto>>> RecordCapture(Guid sessionId, RecordCaptureRequest request, CancellationToken ct) =>
        Ok(ApiResponse<BoothSessionDto>.Ok(await service.RecordCaptureAsync(sessionId, request, ct)));

    [Authorize(Policy = "StartBooth"), HttpPost("sessions/{sessionId:guid}/complete")]
    public async Task<ActionResult<ApiResponse<BoothSessionDto>>> Complete(Guid sessionId, CancellationToken ct) =>
        Ok(ApiResponse<BoothSessionDto>.Ok((await service.GetAsync(sessionId,ct)).SessionToken is null?await service.CompleteAsync(sessionId, ct):await service.CompleteManagedAsync(sessionId,ct)));

    [Authorize(Policy="StartBooth"),HttpPost("sessions/{sessionId:guid}/start")]public async Task<ActionResult<ApiResponse<BoothSessionDto>>>StartManaged(Guid sessionId,CancellationToken ct)=>Ok(ApiResponse<BoothSessionDto>.Ok(await service.StartManagedAsync(sessionId,ct)));
    [Authorize(Policy="StartBooth"),HttpPost("sessions/{sessionId:guid}/pause")]public async Task<ActionResult<ApiResponse<BoothSessionDto>>>Pause(Guid sessionId,CancellationToken ct)=>Ok(ApiResponse<BoothSessionDto>.Ok(await service.PauseAsync(sessionId,ct)));
    [Authorize(Policy="StartBooth"),HttpPost("sessions/{sessionId:guid}/resume")]public async Task<ActionResult<ApiResponse<BoothSessionDto>>>Resume(Guid sessionId,CancellationToken ct)=>Ok(ApiResponse<BoothSessionDto>.Ok(await service.ResumeAsync(sessionId,ct)));
    [Authorize(Policy="StartBooth"),HttpPost("sessions/{sessionId:guid}/cancel")]public async Task<ActionResult<ApiResponse<BoothSessionDto>>>Cancel(Guid sessionId,CancellationToken ct)=>Ok(ApiResponse<BoothSessionDto>.Ok(await service.CancelAsync(sessionId,ct)));
    [Authorize(Policy="StartBooth"),HttpGet("sessions/{sessionId:guid}/activities")]public async Task<ActionResult<ApiResponse<IReadOnlyList<SessionActivityDto>>>>Activities(Guid sessionId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<SessionActivityDto>>.Ok(await activity.ListAsync(sessionId,ct)));
    [Authorize(Policy="StartBooth"),HttpPost("sessions/recover")]public async Task<ActionResult<ApiResponse<ManagedSessionDto?>>>Recover(RecoverSessionRequest request,CancellationToken ct)=>Ok(ApiResponse<ManagedSessionDto?>.Ok(await recovery.RecoverAsync(request,ct)));
    [Authorize(Policy="StartBooth"),HttpGet("runtime/state")]public ActionResult<ApiResponse<BoothRuntimeStateDto>>RuntimeState([FromQuery]Guid organizationId)=>Ok(ApiResponse<BoothRuntimeStateDto>.Ok(runtime.Get(organizationId)));
    [Authorize(Policy="StartBooth"),HttpPost("runtime/reset")]public ActionResult<ApiResponse<BoothRuntimeStateDto>>ResetRuntime([FromQuery]Guid organizationId)=>Ok(ApiResponse<BoothRuntimeStateDto>.Ok(runtime.Reset(organizationId),"Booth runtime reset."));

    [Authorize(Policy = "StartBooth"), HttpDelete("sessions/{sessionId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Abandon(Guid sessionId, CancellationToken ct)
    {
        await service.AbandonAsync(sessionId, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Booth session abandoned."));
    }
}
