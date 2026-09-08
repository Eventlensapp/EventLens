using EventLensAI.Application.Common;using EventLensAI.Application.Features.Booth;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;
namespace EventLensAI.API.Controllers;
[ApiController,Authorize(Policy="StartBooth"),Route("api/booth/camera-controls")]
public sealed class CameraControlsController(ICameraControlService service):ControllerBase
{
 [HttpGet("capabilities")]public async Task<ActionResult<ApiResponse<IReadOnlyList<ProfessionalCameraCapabilityDto>>>>Capabilities([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<ProfessionalCameraCapabilityDto>>.Ok(await service.CapabilitiesAsync(organizationId,ct)));
 [HttpGet("settings")]public async Task<ActionResult<ApiResponse<CameraControlSettingsDto>>>Settings([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<CameraControlSettingsDto>.Ok(await service.GetAsync(organizationId,ct)));
 [HttpPut("settings")]public async Task<ActionResult<ApiResponse<CameraControlSettingsDto>>>Settings(UpdateCameraControlSettingsRequest request,CancellationToken ct)=>Ok(ApiResponse<CameraControlSettingsDto>.Ok(await service.UpdateAsync(request,null,ct),"Camera settings recorded."));
 [HttpPost("reset")]public async Task<ActionResult<ApiResponse<CameraControlSettingsDto>>>Reset([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<CameraControlSettingsDto>.Ok(await service.ResetAsync(organizationId,ct),"Camera controls reset."));
}
[ApiController,Authorize(Policy="StartBooth"),Route("api/booth/camera-profiles")]
public sealed class CameraProfilesController(ICameraProfileService service):ControllerBase
{
 [HttpGet]public async Task<ActionResult<ApiResponse<IReadOnlyList<CameraProfileDto>>>>List([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<CameraProfileDto>>.Ok(await service.ListAsync(organizationId,ct)));
 [HttpPost]public async Task<ActionResult<ApiResponse<CameraProfileDto>>>Create(CreateCameraProfileRequest request,CancellationToken ct)=>StatusCode(201,ApiResponse<CameraProfileDto>.Ok(await service.CreateAsync(request,ct),"Camera profile created."));
 [HttpPut("{id:guid}")]public async Task<ActionResult<ApiResponse<CameraProfileDto>>>Update(Guid id,CreateCameraProfileRequest request,CancellationToken ct)=>Ok(ApiResponse<CameraProfileDto>.Ok(await service.UpdateAsync(id,request,ct),"Camera profile updated."));
 [HttpDelete("{id:guid}")]public async Task<ActionResult<ApiResponse<object>>>Delete(Guid id,CancellationToken ct){await service.DeleteAsync(id,ct);return Ok(ApiResponse<object>.Ok(new{},"Camera profile deleted."));}
 [HttpPost("{id:guid}/apply")]public async Task<ActionResult<ApiResponse<CameraControlSettingsDto>>>Apply(Guid id,CancellationToken ct)=>Ok(ApiResponse<CameraControlSettingsDto>.Ok(await service.ApplyAsync(id,ct),"Camera profile applied."));
}
