using EventLensAI.Domain.Entities;

namespace EventLensAI.Application.Features.Booth;

public enum BoothRuntimeState { Initializing,CheckingPermissions,DetectingHardware,Ready,Busy,Offline,Maintenance,Error }
public enum BoothPermissionState { Granted,Prompt,Denied,Unknown }
public enum CameraState { Initializing,Ready,Streaming,Paused,Stopped,Disconnected,Error }
public sealed record BoothConfigurationDto(Guid Id,Guid OrganizationId,string BoothName,string BoothMode,string? DefaultCamera,string DefaultResolution,string DefaultAspectRatio,string Language,string Theme,int CountdownDefault,int CaptureCountDefault,bool PrivacyMode,bool AutoSave,bool OfflineEnabled,bool MirrorPreview,bool AutoRotate,bool FullscreenMode,bool DebugMode);
public sealed record UpdateBoothConfigurationRequest(Guid OrganizationId,string BoothName,string BoothMode,string? DefaultCamera,string DefaultResolution,string DefaultAspectRatio,string Language,string Theme,int CountdownDefault,int CaptureCountDefault,bool PrivacyMode,bool AutoSave,bool OfflineEnabled,bool MirrorPreview,bool AutoRotate,bool FullscreenMode,bool DebugMode);
public sealed record BoothCapabilityDto(string Name,bool Supported,string Source,DateTime DetectedAt);
public sealed record BoothDeviceDto(string DeviceKeyHash,string Kind,string Label,string MetadataJson,DateTime LastSeenAt);
public sealed record BoothPermissionDto(string Permission,BoothPermissionState State,string Message,bool CanRetry);
public sealed record PermissionRequest(Guid OrganizationId,string Permission);
public sealed record BoothStateDto(Guid OrganizationId,BoothRuntimeState State,string? Reason,DateTime ChangedAt);
public sealed record BoothHealthItemDto(string Category,string Status,int Score,string Message,DateTime CheckedAt);
public sealed record BoothDiagnosticsDto(int HealthScore,int Warnings,int Errors,IReadOnlyList<BoothHealthItemDto> Checks);
public sealed record BoothOfflineDto(bool Supported,bool CacheInitialized,bool Online,string Message);
public sealed record CameraDeviceDto(string DeviceId,string Label,string Kind,string? GroupId,string? FacingMode,bool IsDefault,bool IsAvailable);
public sealed record CameraCapabilityDto(string Name,bool Supported,double? Minimum,double? Maximum,double? Step,IReadOnlyList<string> Values);
public sealed record CameraPreferenceDto(Guid Id,Guid OrganizationId,string? PreferredCameraId,string PreferredResolution,bool MirrorPreview,string AspectRatio);
public sealed record UpdateCameraPreferenceRequest(Guid OrganizationId,string? PreferredCameraId,string PreferredResolution,bool MirrorPreview,string AspectRatio);
public sealed record CameraPreviewDto(Guid OrganizationId,CameraState State,string? CurrentCameraId,string Resolution,bool MirrorPreview,string AspectRatio,string? ErrorCode,DateTime ChangedAt);

public interface IBoothConfigurationService { Task<BoothConfigurationDto> GetAsync(Guid organizationId,CancellationToken ct);Task<BoothConfigurationDto> UpdateAsync(UpdateBoothConfigurationRequest request,CancellationToken ct); }
public interface IBrowserCapabilityService { Task<IReadOnlyList<BoothCapabilityDto>> GetAsync(Guid organizationId,CancellationToken ct); }
public interface IHardwareDetectionService { Task<IReadOnlyList<BoothDeviceDto>> GetAsync(Guid organizationId,CancellationToken ct); }
public interface IPermissionService { Task<IReadOnlyList<BoothPermissionDto>> GetAsync(Guid organizationId,CancellationToken ct);Task<BoothPermissionDto> RequestAsync(PermissionRequest request,CancellationToken ct); }
public interface IBoothStateService { BoothStateDto Get(Guid organizationId);BoothStateDto Transition(Guid organizationId,BoothRuntimeState next,string? reason=null); }
public interface IBoothDiagnosticService { Task<BoothDiagnosticsDto> GetAsync(Guid organizationId,CancellationToken ct); }
public interface IOfflineFoundationService { Task<BoothOfflineDto> GetAsync(Guid organizationId,CancellationToken ct); }
public interface ICameraManager { CameraPreviewDto Get(Guid organizationId);CameraPreviewDto Initialize(Guid organizationId,string? cameraId,string resolution,bool mirror,string aspectRatio);CameraPreviewDto StartPreview(Guid organizationId);CameraPreviewDto StopPreview(Guid organizationId);CameraPreviewDto PausePreview(Guid organizationId);CameraPreviewDto ResumePreview(Guid organizationId);CameraPreviewDto Release(Guid organizationId);CameraPreviewDto Restart(Guid organizationId);CameraPreviewDto Fail(Guid organizationId,string errorCode); }
public interface ICameraDiscoveryService { Task<IReadOnlyList<CameraDeviceDto>> GetAsync(Guid organizationId,CancellationToken ct);Task<IReadOnlyList<CameraDeviceDto>> RefreshAsync(Guid organizationId,CancellationToken ct); }
public interface ICameraCapabilityService { Task<IReadOnlyList<CameraCapabilityDto>> GetAsync(Guid organizationId,CancellationToken ct); }
public interface ICameraPreferenceService { Task<CameraPreferenceDto> GetAsync(Guid organizationId,CancellationToken ct);Task<CameraPreferenceDto> UpdateAsync(UpdateCameraPreferenceRequest request,CancellationToken ct); }
public interface IBoothFoundationRepository
{
    Task<BoothConfiguration?> GetConfigurationAsync(Guid organizationId,CancellationToken ct);Task AddConfigurationAsync(BoothConfiguration value,CancellationToken ct);
    Task<IReadOnlyList<BoothCapability>> GetCapabilitiesAsync(Guid organizationId,CancellationToken ct);Task<IReadOnlyList<BoothDevice>> GetDevicesAsync(Guid organizationId,CancellationToken ct);
    Task<IReadOnlyList<BoothHealthCheck>> GetHealthAsync(Guid organizationId,CancellationToken ct);
    Task<CameraPreference?> GetCameraPreferenceAsync(Guid organizationId,CancellationToken ct);Task AddCameraPreferenceAsync(CameraPreference value,CancellationToken ct);
}
