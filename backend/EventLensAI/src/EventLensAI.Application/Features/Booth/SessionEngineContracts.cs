using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Application.Features.Booth;

public enum ManagedBoothRuntimeState{Idle,Preparing,Ready,Active,Paused,Completing,Maintenance,Offline,Error}
public sealed record CreateSessionRequest(Guid OrganizationId,Guid EventId,Guid BoothId,BoothSessionType SessionType,string? GuestName,string? GuestEmail,string DeviceInformation,string MetadataJson,Guid? GuestId=null,CaptureMode? CaptureMode=null,int? Countdown=null,Guid? TemplateId=null);
public sealed record UpdateSessionRequest(string? MetadataJson=null,int? IdleTimeoutSeconds=null);
public sealed record ManagedSessionDto(Guid Id,Guid OrganizationId,Guid EventId,Guid BoothId,string SessionToken,BoothSessionType SessionType,BoothSessionStatus Status,DateTime? StartedAt,DateTime? EndedAt,DateTime LastActivityAt,string? GuestName,string MetadataJson);
public sealed record SessionActivityDto(Guid Id,Guid SessionId,string Action,DateTime Timestamp,string Metadata);
public sealed record BoothRuntimeStateDto(Guid OrganizationId,ManagedBoothRuntimeState State,Guid? SessionId,string? Reason,DateTime ChangedAt);
public sealed record RecoverSessionRequest(Guid OrganizationId,string SessionToken,int TimeoutSeconds=300);

public interface IBoothRuntimeService{BoothRuntimeStateDto Get(Guid organizationId);BoothRuntimeStateDto Transition(Guid organizationId,ManagedBoothRuntimeState next,Guid? sessionId=null,string? reason=null);BoothRuntimeStateDto Reset(Guid organizationId);}
public interface ISessionRecoveryService{Task<ManagedSessionDto?>RecoverAsync(RecoverSessionRequest request,CancellationToken ct);}
public interface ISessionActivityService{Task<IReadOnlyList<SessionActivityDto>>ListAsync(Guid sessionId,CancellationToken ct);}
public interface IBoothSessionEngineRepository
{
    Task<BoothSession?>GetAsync(Guid id,CancellationToken ct);Task<BoothSession?>GetByTokenAsync(string token,CancellationToken ct);Task AddActivityAsync(BoothSessionActivity activity,CancellationToken ct);
    Task<IReadOnlyList<BoothSessionActivity>>ListActivitiesAsync(Guid sessionId,CancellationToken ct);Task<IReadOnlyList<BoothSession>>ListExpiredCandidatesAsync(DateTime inactiveBefore,CancellationToken ct);
}
