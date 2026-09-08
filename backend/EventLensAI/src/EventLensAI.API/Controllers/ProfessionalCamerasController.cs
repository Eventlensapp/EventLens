using EventLensAI.Application.Common;using EventLensAI.Application.Features.Booth;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;
namespace EventLensAI.API.Controllers;
[ApiController,Authorize(Policy="StartBooth"),Route("api/booth/cameras")]
public sealed class ProfessionalCamerasController(IProfessionalCameraService service,ICameraHealthService health):ControllerBase
{
 [HttpGet("providers")]public ActionResult<ApiResponse<IReadOnlyList<CameraProviderDto>>>Providers()=>Ok(ApiResponse<IReadOnlyList<CameraProviderDto>>.Ok(service.Providers()));
 [HttpGet]public async Task<ActionResult<ApiResponse<IReadOnlyList<ProfessionalCameraDto>>>>List([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<ProfessionalCameraDto>>.Ok(await service.ListAsync(organizationId,ct)));
 [HttpPost]public async Task<ActionResult<ApiResponse<ProfessionalCameraDto>>>Register(RegisterProfessionalCameraRequest request,CancellationToken ct)=>StatusCode(201,ApiResponse<ProfessionalCameraDto>.Ok(await service.RegisterAsync(request,ct),"Camera registered."));
 [HttpPost("{id:guid}/connect")]public async Task<ActionResult<ApiResponse<CameraConnectionDto>>>Connect(Guid id,CancellationToken ct)=>Ok(ApiResponse<CameraConnectionDto>.Ok(await service.ConnectAsync(id,ct),"Camera connected."));
 [HttpPost("{id:guid}/disconnect")]public async Task<ActionResult<ApiResponse<CameraConnectionDto>>>Disconnect(Guid id,CancellationToken ct)=>Ok(ApiResponse<CameraConnectionDto>.Ok(await service.DisconnectAsync(id,ct),"Camera disconnected."));
 [HttpGet("{id:guid}/status")]public async Task<ActionResult<ApiResponse<CameraConnectionDto>>>Status(Guid id,CancellationToken ct)=>Ok(ApiResponse<CameraConnectionDto>.Ok(await health.CheckAsync(id,ct)));
 [HttpGet("{id:guid}/capabilities")]public async Task<ActionResult<ApiResponse<CameraCapabilityProfileDto>>>Capabilities(Guid id,CancellationToken ct)=>Ok(ApiResponse<CameraCapabilityProfileDto>.Ok(await service.CapabilitiesAsync(id,ct)));
}
