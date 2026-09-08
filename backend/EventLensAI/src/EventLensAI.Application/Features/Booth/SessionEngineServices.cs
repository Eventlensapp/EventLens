using System.Collections.Concurrent;
using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Entities;

namespace EventLensAI.Application.Features.Booth;

public sealed class BoothRuntimeService:IBoothRuntimeService
{
    private readonly ConcurrentDictionary<Guid,BoothRuntimeStateDto> states=new();
    public BoothRuntimeStateDto Get(Guid id)=>states.GetOrAdd(id,x=>New(x,ManagedBoothRuntimeState.Idle,null,null));
    public BoothRuntimeStateDto Transition(Guid id,ManagedBoothRuntimeState next,Guid? sessionId=null,string? reason=null){var current=Get(id);if(!Allowed(current.State,next))throw new ConflictException($"Cannot move booth runtime from {current.State} to {next}.");var value=New(id,next,sessionId,reason);states[id]=value;return value;}
    public BoothRuntimeStateDto Reset(Guid id){var value=New(id,ManagedBoothRuntimeState.Ready,null,"Operator reset");states[id]=value;return value;}
    private static BoothRuntimeStateDto New(Guid id,ManagedBoothRuntimeState state,Guid? session,string? reason)=>new(id,state,session,reason,DateTime.UtcNow);
    private static bool Allowed(ManagedBoothRuntimeState from,ManagedBoothRuntimeState to)=>from==to||to is ManagedBoothRuntimeState.Error or ManagedBoothRuntimeState.Maintenance or ManagedBoothRuntimeState.Offline||(from,to) switch{(ManagedBoothRuntimeState.Idle,ManagedBoothRuntimeState.Preparing)=>true,(ManagedBoothRuntimeState.Preparing,ManagedBoothRuntimeState.Ready)=>true,(ManagedBoothRuntimeState.Ready,ManagedBoothRuntimeState.Active)=>true,(ManagedBoothRuntimeState.Active,ManagedBoothRuntimeState.Paused)=>true,(ManagedBoothRuntimeState.Paused,ManagedBoothRuntimeState.Active)=>true,(ManagedBoothRuntimeState.Active,ManagedBoothRuntimeState.Completing)=>true,(ManagedBoothRuntimeState.Paused,ManagedBoothRuntimeState.Completing)=>true,(ManagedBoothRuntimeState.Completing,ManagedBoothRuntimeState.Ready)=>true,(ManagedBoothRuntimeState.Maintenance,ManagedBoothRuntimeState.Preparing)=>true,(ManagedBoothRuntimeState.Offline,ManagedBoothRuntimeState.Preparing)=>true,(ManagedBoothRuntimeState.Error,ManagedBoothRuntimeState.Preparing)=>true,_=>false};
}

public sealed class SessionRecoveryService(IBoothSessionEngineRepository repository,IEventRepository events,IOrganizationRepository organizations,ICurrentUserService current):BoothTenantService(organizations,current),ISessionRecoveryService
{
    public async Task<ManagedSessionDto?>RecoverAsync(RecoverSessionRequest request,CancellationToken ct){await Authorize(request.OrganizationId,false,ct);var session=await repository.GetByTokenAsync(request.SessionToken,ct);if(session is null)return null;var e=await events.GetAsync(session.EventId,ct);if(e?.OrganizationId!=request.OrganizationId)throw new UnauthorizedException("Session access denied.");return session.CanRecover(DateTime.UtcNow,request.TimeoutSeconds)?SessionEngineMapper.Map(session):null;}
}
public sealed class SessionActivityService(IBoothSessionEngineRepository repository,IEventRepository events,IOrganizationRepository organizations,ICurrentUserService current):BoothTenantService(organizations,current),ISessionActivityService
{
    public async Task<IReadOnlyList<SessionActivityDto>>ListAsync(Guid sessionId,CancellationToken ct){var session=await repository.GetAsync(sessionId,ct)??throw new NotFoundException("Booth session was not found.");var e=await events.GetAsync(session.EventId,ct)??throw new NotFoundException("Event was not found.");await Authorize(e.OrganizationId,false,ct);return(await repository.ListActivitiesAsync(sessionId,ct)).Select(x=>new SessionActivityDto(x.Id,x.SessionId,x.Action,x.Timestamp,x.Metadata)).ToArray();}
}
public static class SessionEngineMapper
{
    public static ManagedSessionDto Map(BoothSession s)=>new(s.Id,s.OrganizationId??Guid.Empty,s.EventId,s.BoothId??Guid.Empty,s.SessionToken??"",s.SessionType,s.Status,s.StartedAt==default?null:s.StartedAt,s.EndedAt,s.LastActivityAt,s.GuestName,s.MetadataJson);
}
