using System.Collections.Concurrent;
using AutoMapper;
using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EventLensAI.Application.Features.Booth;

public sealed class CameraManager(ILogger<CameraManager> logger) : ICameraManager
{
    private readonly ConcurrentDictionary<Guid, CameraPreviewDto> states = new();
    public CameraPreviewDto Get(Guid organizationId) => states.GetOrAdd(organizationId, id => New(id, CameraState.Stopped, null, "1280x720", true, "16:9"));
    public CameraPreviewDto Initialize(Guid id,string? cameraId,string resolution,bool mirror,string aspectRatio)=>Set(New(id,CameraState.Initializing,cameraId,resolution,mirror,aspectRatio));
    public CameraPreviewDto StartPreview(Guid id)=>Move(id,CameraState.Streaming,CameraState.Initializing,CameraState.Ready,CameraState.Paused,CameraState.Stopped,CameraState.Disconnected);
    public CameraPreviewDto StopPreview(Guid id)=>Move(id,CameraState.Stopped,CameraState.Streaming,CameraState.Paused,CameraState.Ready,CameraState.Error,CameraState.Disconnected);
    public CameraPreviewDto PausePreview(Guid id)=>Move(id,CameraState.Paused,CameraState.Streaming);
    public CameraPreviewDto ResumePreview(Guid id)=>Move(id,CameraState.Streaming,CameraState.Paused);
    public CameraPreviewDto Release(Guid id)=>Move(id,CameraState.Stopped,CameraState.Initializing,CameraState.Ready,CameraState.Streaming,CameraState.Paused,CameraState.Error,CameraState.Disconnected,CameraState.Stopped);
    public CameraPreviewDto Restart(Guid id){var current=Get(id);Set(current with{State=CameraState.Initializing,ErrorCode=null,ChangedAt=DateTime.UtcNow});return StartPreview(id);}
    public CameraPreviewDto Fail(Guid id,string errorCode){logger.LogWarning("Camera runtime entered error state for organization {OrganizationId}. Code: {ErrorCode}",id,errorCode);var current=Get(id);return Set(current with{State=errorCode=="CameraDisconnected"?CameraState.Disconnected:CameraState.Error,ErrorCode=errorCode,ChangedAt=DateTime.UtcNow});}
    private CameraPreviewDto Move(Guid id,CameraState next,params CameraState[] allowed){var current=Get(id);if(!allowed.Contains(current.State))throw new ConflictException($"Cannot move camera from {current.State} to {next}.");return Set(current with{State=next,ErrorCode=null,ChangedAt=DateTime.UtcNow});}
    private CameraPreviewDto Set(CameraPreviewDto value){states[value.OrganizationId]=value;return value;}
    private static CameraPreviewDto New(Guid id,CameraState state,string? camera,string resolution,bool mirror,string aspect)=>new(id,state,camera,resolution,mirror,aspect,null,DateTime.UtcNow);
}

public sealed class CameraDiscoveryService(IBoothFoundationRepository repository,IOrganizationRepository organizations,ICurrentUserService current):BoothTenantService(organizations,current),ICameraDiscoveryService
{
    public async Task<IReadOnlyList<CameraDeviceDto>>GetAsync(Guid organizationId,CancellationToken ct)
    {
        await Authorize(organizationId,false,ct);
        var devices=await repository.GetDevicesAsync(organizationId,ct);
        return devices.Where(x=>x.Kind=="videoinput").Select((x,index)=>new CameraDeviceDto(x.DeviceKeyHash,x.Label,x.Kind,null,null,index==0,true)).ToArray();
    }
    public Task<IReadOnlyList<CameraDeviceDto>>RefreshAsync(Guid organizationId,CancellationToken ct)=>GetAsync(organizationId,ct);
}

public sealed class CameraCapabilityService(IOrganizationRepository organizations,ICurrentUserService current):BoothTenantService(organizations,current),ICameraCapabilityService
{
    private static readonly CameraCapabilityDto[] BrowserCapabilities=[
        new("width",true,640,3840,1,[]),new("height",true,480,2160,1,[]),new("frameRate",true,1,60,1,[]),
        new("zoom",false,null,null,null,[]),new("focusMode",false,null,null,null,["manual","single-shot","continuous"]),
        new("torch",false,null,null,null,[]),new("exposureMode",false,null,null,null,[]),new("whiteBalanceMode",false,null,null,null,[]),
        new("brightness",false,null,null,null,[]),new("contrast",false,null,null,null,[])
    ];
    public async Task<IReadOnlyList<CameraCapabilityDto>>GetAsync(Guid organizationId,CancellationToken ct){await Authorize(organizationId,false,ct);return BrowserCapabilities;}
}

public sealed class CameraPreferenceService(IBoothFoundationRepository repository,IOrganizationRepository organizations,ICurrentUserService current,IUnitOfWork unitOfWork,IMapper mapper):BoothTenantService(organizations,current),ICameraPreferenceService
{
    public async Task<CameraPreferenceDto>GetAsync(Guid organizationId,CancellationToken ct){await Authorize(organizationId,false,ct);var value=await repository.GetCameraPreferenceAsync(organizationId,ct)??new CameraPreference(organizationId,null,"1280x720",true,"16:9");return mapper.Map<CameraPreferenceDto>(value);}
    public async Task<CameraPreferenceDto>UpdateAsync(UpdateCameraPreferenceRequest request,CancellationToken ct){await Authorize(request.OrganizationId,true,ct);var value=await repository.GetCameraPreferenceAsync(request.OrganizationId,ct);if(value is null){value=new CameraPreference(request.OrganizationId,request.PreferredCameraId,request.PreferredResolution,request.MirrorPreview,request.AspectRatio);await repository.AddCameraPreferenceAsync(value,ct);}else value.Update(request.PreferredCameraId,request.PreferredResolution,request.MirrorPreview,request.AspectRatio);await unitOfWork.SaveChangesAsync(ct);return mapper.Map<CameraPreferenceDto>(value);}
}
