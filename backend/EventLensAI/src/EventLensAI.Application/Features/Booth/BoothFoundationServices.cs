using System.Collections.Concurrent;
using AutoMapper;
using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Application.Features.Booth;

public abstract class BoothTenantService(IOrganizationRepository organizations,ICurrentUserService current)
{
    protected async Task Authorize(Guid organizationId,bool write,CancellationToken ct)
    {
        if(current.Roles.Contains(SystemRoles.SuperAdmin))return;
        var userId=current.UserId??throw new UnauthorizedException("Authentication required.");
        var member=await organizations.GetMemberAsync(organizationId,userId,ct)??throw new UnauthorizedException("Organization access denied.");
        if(write&&member.Role.Name is not(SystemRoles.Owner or SystemRoles.Manager or SystemRoles.Photographer))throw new UnauthorizedException("Booth configuration permission denied.");
    }
}
public sealed class BoothConfigurationService(IBoothFoundationRepository repository,IOrganizationRepository organizations,ICurrentUserService current,IUnitOfWork unitOfWork,IMapper mapper):BoothTenantService(organizations,current),IBoothConfigurationService
{
    public async Task<BoothConfigurationDto> GetAsync(Guid organizationId,CancellationToken ct){await Authorize(organizationId,false,ct);var value=await repository.GetConfigurationAsync(organizationId,ct)??new BoothConfiguration(organizationId,"Main booth");return mapper.Map<BoothConfigurationDto>(value);}
    public async Task<BoothConfigurationDto> UpdateAsync(UpdateBoothConfigurationRequest request,CancellationToken ct){await Authorize(request.OrganizationId,true,ct);var value=await repository.GetConfigurationAsync(request.OrganizationId,ct);if(value is null){value=new BoothConfiguration(request.OrganizationId,request.BoothName);await repository.AddConfigurationAsync(value,ct);}value.Update(request.BoothName,request.BoothMode,request.DefaultCamera,request.DefaultResolution,request.DefaultAspectRatio,request.Language,request.Theme,request.CountdownDefault,request.CaptureCountDefault,request.PrivacyMode,request.AutoSave,request.OfflineEnabled,request.MirrorPreview,request.AutoRotate,request.FullscreenMode,request.DebugMode);await unitOfWork.SaveChangesAsync(ct);return mapper.Map<BoothConfigurationDto>(value);}
}
public sealed class BrowserCapabilityService(IBoothFoundationRepository repository,IOrganizationRepository organizations,ICurrentUserService current):BoothTenantService(organizations,current),IBrowserCapabilityService
{public async Task<IReadOnlyList<BoothCapabilityDto>>GetAsync(Guid organizationId,CancellationToken ct){await Authorize(organizationId,false,ct);return(await repository.GetCapabilitiesAsync(organizationId,ct)).Select(x=>new BoothCapabilityDto(x.Name,x.IsSupported,x.Source,x.DetectedAt)).ToArray();}}
public sealed class HardwareDetectionService(IBoothFoundationRepository repository,IOrganizationRepository organizations,ICurrentUserService current):BoothTenantService(organizations,current),IHardwareDetectionService
{public async Task<IReadOnlyList<BoothDeviceDto>>GetAsync(Guid organizationId,CancellationToken ct){await Authorize(organizationId,false,ct);return(await repository.GetDevicesAsync(organizationId,ct)).Select(x=>new BoothDeviceDto(x.DeviceKeyHash,x.Kind,x.Label,x.MetadataJson,x.LastSeenAt)).ToArray();}}
public sealed class PermissionService(IOrganizationRepository organizations,ICurrentUserService current):BoothTenantService(organizations,current),IPermissionService
{
    private static readonly string[] Names=["camera","microphone","notifications","file-system"];
    public async Task<IReadOnlyList<BoothPermissionDto>>GetAsync(Guid organizationId,CancellationToken ct){await Authorize(organizationId,false,ct);return Names.Select(x=>Unknown(x)).ToArray();}
    public async Task<BoothPermissionDto>RequestAsync(PermissionRequest request,CancellationToken ct){await Authorize(request.OrganizationId,true,ct);if(!Names.Contains(request.Permission,StringComparer.OrdinalIgnoreCase))throw new ConflictException("Unsupported booth permission.");return Unknown(request.Permission);}
    private static BoothPermissionDto Unknown(string name)=>new(name,BoothPermissionState.Unknown,"Permission state is determined securely by this browser.",true);
}
public sealed class BoothStateService:IBoothStateService
{
    private readonly ConcurrentDictionary<Guid,BoothStateDto> states=new();
    public BoothStateDto Get(Guid organizationId)=>states.GetOrAdd(organizationId,id=>new(id,BoothRuntimeState.Initializing,null,DateTime.UtcNow));
    public BoothStateDto Transition(Guid organizationId,BoothRuntimeState next,string? reason=null){var current=Get(organizationId);if(!Allowed(current.State,next))throw new ConflictException($"Cannot transition booth from {current.State} to {next}.");var result=new BoothStateDto(organizationId,next,reason,DateTime.UtcNow);states[organizationId]=result;return result;}
    private static bool Allowed(BoothRuntimeState from,BoothRuntimeState to)=>from==to||to is BoothRuntimeState.Error or BoothRuntimeState.Offline or BoothRuntimeState.Maintenance||(from,to) switch{(BoothRuntimeState.Initializing,BoothRuntimeState.CheckingPermissions)=>true,(BoothRuntimeState.CheckingPermissions,BoothRuntimeState.DetectingHardware)=>true,(BoothRuntimeState.DetectingHardware,BoothRuntimeState.Ready)=>true,(BoothRuntimeState.Ready,BoothRuntimeState.Busy)=>true,(BoothRuntimeState.Busy,BoothRuntimeState.Ready)=>true,(BoothRuntimeState.Offline,BoothRuntimeState.Initializing)=>true,(BoothRuntimeState.Maintenance,BoothRuntimeState.Initializing)=>true,(BoothRuntimeState.Error,BoothRuntimeState.Initializing)=>true,_=>false};
}
public sealed class BoothDiagnosticService(IBoothFoundationRepository repository,IOrganizationRepository organizations,ICurrentUserService current):BoothTenantService(organizations,current),IBoothDiagnosticService
{public async Task<BoothDiagnosticsDto>GetAsync(Guid organizationId,CancellationToken ct){await Authorize(organizationId,false,ct);var checks=(await repository.GetHealthAsync(organizationId,ct)).Select(x=>new BoothHealthItemDto(x.Category,x.Status,x.Score,x.Message,x.CheckedAt)).ToArray();var score=checks.Length==0?0:(int)Math.Round(checks.Average(x=>x.Score));return new(score,checks.Count(x=>x.Status=="Warning"),checks.Count(x=>x.Status=="Error"),checks);}}
public sealed class OfflineFoundationService(IOrganizationRepository organizations,ICurrentUserService current):BoothTenantService(organizations,current),IOfflineFoundationService
{public async Task<BoothOfflineDto>GetAsync(Guid organizationId,CancellationToken ct){await Authorize(organizationId,false,ct);return new(true,false,true,"Offline cache initialization is owned by the active browser. Synchronization is not enabled in Phase 0.");}}
