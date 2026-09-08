using EventLensAI.Application.Common;
using EventLensAI.Application.Features.Booth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventLensAI.API.Controllers;

[ApiController,Authorize(Policy="StartBooth"),Route("api/booth"),Produces("application/json")]
public sealed class BoothFoundationController(IBoothConfigurationService configuration,IBrowserCapabilityService capabilities,IHardwareDetectionService hardware,IPermissionService permissions,IBoothStateService state,IBoothDiagnosticService diagnostics,IOfflineFoundationService offline,ICameraDiscoveryService cameras,ICameraCapabilityService cameraCapabilities,ICameraPreferenceService cameraPreferences):ControllerBase
{
    [HttpGet("configuration")]public async Task<ActionResult<ApiResponse<BoothConfigurationDto>>>GetConfiguration([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<BoothConfigurationDto>.Ok(await configuration.GetAsync(organizationId,ct)));
    [HttpPut("configuration")]public async Task<ActionResult<ApiResponse<BoothConfigurationDto>>>UpdateConfiguration(UpdateBoothConfigurationRequest request,CancellationToken ct)=>Ok(ApiResponse<BoothConfigurationDto>.Ok(await configuration.UpdateAsync(request,ct),"Booth configuration saved."));
    [HttpGet("capabilities")]public async Task<ActionResult<ApiResponse<IReadOnlyList<BoothCapabilityDto>>>>GetCapabilities([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<BoothCapabilityDto>>.Ok(await capabilities.GetAsync(organizationId,ct)));
    [HttpGet("devices")]public async Task<ActionResult<ApiResponse<IReadOnlyList<BoothDeviceDto>>>>GetDevices([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<BoothDeviceDto>>.Ok(await hardware.GetAsync(organizationId,ct)));
    [HttpGet("permissions")]public async Task<ActionResult<ApiResponse<IReadOnlyList<BoothPermissionDto>>>>GetPermissions([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<BoothPermissionDto>>.Ok(await permissions.GetAsync(organizationId,ct)));
    [HttpPost("permissions/request")]public async Task<ActionResult<ApiResponse<BoothPermissionDto>>>RequestPermission(PermissionRequest request,CancellationToken ct)=>Ok(ApiResponse<BoothPermissionDto>.Ok(await permissions.RequestAsync(request,ct)));
    [HttpGet("diagnostics")]public async Task<ActionResult<ApiResponse<BoothDiagnosticsDto>>>GetDiagnostics([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<BoothDiagnosticsDto>.Ok(await diagnostics.GetAsync(organizationId,ct)));
    [HttpGet("state")]public ActionResult<ApiResponse<BoothStateDto>>GetState([FromQuery]Guid organizationId)=>Ok(ApiResponse<BoothStateDto>.Ok(state.Get(organizationId)));
    [HttpGet("offline")]public async Task<ActionResult<ApiResponse<BoothOfflineDto>>>GetOffline([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<BoothOfflineDto>.Ok(await offline.GetAsync(organizationId,ct)));
    [HttpGet("cameras")]public async Task<ActionResult<ApiResponse<IReadOnlyList<CameraDeviceDto>>>>GetCameras([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<CameraDeviceDto>>.Ok(await cameras.GetAsync(organizationId,ct)));
    [HttpGet("cameras/capabilities")]public async Task<ActionResult<ApiResponse<IReadOnlyList<CameraCapabilityDto>>>>GetCameraCapabilities([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<CameraCapabilityDto>>.Ok(await cameraCapabilities.GetAsync(organizationId,ct)));
    [HttpGet("cameras/preferences")]public async Task<ActionResult<ApiResponse<CameraPreferenceDto>>>GetCameraPreferences([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<CameraPreferenceDto>.Ok(await cameraPreferences.GetAsync(organizationId,ct)));
    [HttpPut("cameras/preferences")]public async Task<ActionResult<ApiResponse<CameraPreferenceDto>>>UpdateCameraPreferences(UpdateCameraPreferenceRequest request,CancellationToken ct)=>Ok(ApiResponse<CameraPreferenceDto>.Ok(await cameraPreferences.UpdateAsync(request,ct),"Camera preference saved."));
    [HttpPost("cameras/refresh")]public async Task<ActionResult<ApiResponse<IReadOnlyList<CameraDeviceDto>>>>RefreshCameras([FromQuery]Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<CameraDeviceDto>>.Ok(await cameras.RefreshAsync(organizationId,ct)));
}
