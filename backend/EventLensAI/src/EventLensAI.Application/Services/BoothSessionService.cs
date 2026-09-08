using EventLensAI.Application.DTOs.Booth;
using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;
using EventLensAI.Application.Features.Booth;

namespace EventLensAI.Application.Services;

public interface IBoothSessionService
{
    Task<BoothBootstrapDto> BootstrapAsync(string eventSlug, CancellationToken ct);
    Task<BoothSessionDto> StartAsync(StartBoothSessionRequest request, CancellationToken ct);
    Task<BoothSessionDto> GetAsync(Guid sessionId, CancellationToken ct);
    Task<BoothSessionDto> BeginCaptureAsync(Guid sessionId, CancellationToken ct);
    Task<BoothSessionDto> RecordCaptureAsync(Guid sessionId, RecordCaptureRequest request, CancellationToken ct);
    Task<BoothSessionDto> CompleteAsync(Guid sessionId, CancellationToken ct);
    Task AbandonAsync(Guid sessionId, CancellationToken ct);
    Task<BoothSessionDto>CreateManagedAsync(CreateSessionRequest request,CancellationToken ct);
    Task<BoothSessionDto>StartManagedAsync(Guid sessionId,CancellationToken ct);
    Task<BoothSessionDto>PauseAsync(Guid sessionId,CancellationToken ct);
    Task<BoothSessionDto>ResumeAsync(Guid sessionId,CancellationToken ct);
    Task<BoothSessionDto>CompleteManagedAsync(Guid sessionId,CancellationToken ct);
    Task<BoothSessionDto>CancelAsync(Guid sessionId,CancellationToken ct);
    Task<int>ExpireInactiveAsync(int timeoutSeconds,CancellationToken ct);
}

public sealed class BoothSessionService(IEventRepository events, IRepository<BoothSession> sessions,IBoothSessionEngineRepository engine,IBoothRuntimeService runtime,IUnitOfWork unitOfWork) : IBoothSessionService
{
    public async Task<BoothBootstrapDto> BootstrapAsync(string eventSlug, CancellationToken ct)
    {
        var e = await events.GetBySlugAsync(eventSlug, ct) ?? throw new NotFoundException("Event was not found.");
        return new(e.Id, e.Slug, e.Name, e.PrimaryColor, e.SecondaryColor, e.PublicGalleryEnabled,
            e.AllowDownloads, e.EnableQRCode, e.Settings?.Countdown ?? 3,
            e.Settings?.CaptureMode ?? CaptureMode.SinglePhoto);
    }
    public async Task<BoothSessionDto> StartAsync(StartBoothSessionRequest request, CancellationToken ct)
    {
        _ = await events.GetAsync(request.EventId, ct) ?? throw new NotFoundException("Event was not found.");
        var session = new BoothSession(request.EventId, request.GuestId, request.CaptureMode, request.Countdown, request.TemplateId);
        await sessions.AddAsync(session, ct); await unitOfWork.SaveChangesAsync(ct); return Map(session);
    }
    public async Task<BoothSessionDto> GetAsync(Guid id, CancellationToken ct) => Map(await Find(id, ct));
    public async Task<BoothSessionDto> BeginCaptureAsync(Guid id, CancellationToken ct) { var s = await Find(id, ct); s.BeginCapture(); await unitOfWork.SaveChangesAsync(ct); return Map(s); }
    public async Task<BoothSessionDto> RecordCaptureAsync(Guid id, RecordCaptureRequest request, CancellationToken ct) { var s = await Find(id, ct); s.RecordCapture(request.PhotoCount); await unitOfWork.SaveChangesAsync(ct); return Map(s); }
    public async Task<BoothSessionDto> CompleteAsync(Guid id, CancellationToken ct) { var s = await Find(id, ct); s.Complete(); await unitOfWork.SaveChangesAsync(ct); return Map(s); }
    public async Task AbandonAsync(Guid id, CancellationToken ct) { var s = await Find(id, ct); s.Abandon(); await unitOfWork.SaveChangesAsync(ct); }
    public async Task<BoothSessionDto>CreateManagedAsync(CreateSessionRequest request,CancellationToken ct)
    {
        var e=await events.GetAsync(request.EventId,ct)??throw new NotFoundException("Event was not found.");if(e.OrganizationId!=request.OrganizationId)throw new UnauthorizedException("Event does not belong to this organization.");
        var s=new BoothSession(request.OrganizationId,request.EventId,request.BoothId,request.SessionType,request.GuestName,request.GuestEmail,request.DeviceInformation,request.MetadataJson);
        await sessions.AddAsync(s,ct);await engine.AddActivityAsync(new BoothSessionActivity(s.Id,"Created","{}"),ct);await unitOfWork.SaveChangesAsync(ct);return Map(s);
    }
    public Task<BoothSessionDto>StartManagedAsync(Guid id,CancellationToken ct)=>Change(id,"Started",s=>s.StartManaged(),ct,true);
    public Task<BoothSessionDto>PauseAsync(Guid id,CancellationToken ct)=>Change(id,"Paused",s=>s.Pause(),ct);
    public Task<BoothSessionDto>ResumeAsync(Guid id,CancellationToken ct)=>Change(id,"Resumed",s=>s.Resume(),ct);
    public Task<BoothSessionDto>CompleteManagedAsync(Guid id,CancellationToken ct)=>Change(id,"Completed",s=>s.CompleteManaged(),ct);
    public Task<BoothSessionDto>CancelAsync(Guid id,CancellationToken ct)=>Change(id,"Cancelled",s=>s.Cancel(),ct);
    public async Task<int>ExpireInactiveAsync(int timeoutSeconds,CancellationToken ct){var values=await engine.ListExpiredCandidatesAsync(DateTime.UtcNow.AddSeconds(-timeoutSeconds),ct);foreach(var s in values){s.Expire();await engine.AddActivityAsync(new BoothSessionActivity(s.Id,"Expired","{}"),ct);if(s.OrganizationId.HasValue)runtime.Reset(s.OrganizationId.Value);}if(values.Count>0)await unitOfWork.SaveChangesAsync(ct);return values.Count;}
    private async Task<BoothSessionDto>Change(Guid id,string action,Action<BoothSession> mutation,CancellationToken ct,bool start=false){var s=await engine.GetAsync(id,ct)??throw new NotFoundException("Booth session was not found.");try{mutation(s);}catch(InvalidOperationException ex){throw new ConflictException(ex.Message);}await engine.AddActivityAsync(new BoothSessionActivity(s.Id,action,"{}"),ct);if(s.OrganizationId.HasValue){if(start){var state=runtime.Get(s.OrganizationId.Value);if(state.State==ManagedBoothRuntimeState.Idle){runtime.Transition(s.OrganizationId.Value,ManagedBoothRuntimeState.Preparing,s.Id);runtime.Transition(s.OrganizationId.Value,ManagedBoothRuntimeState.Ready,s.Id);}runtime.Transition(s.OrganizationId.Value,ManagedBoothRuntimeState.Active,s.Id);}else if(action=="Paused")runtime.Transition(s.OrganizationId.Value,ManagedBoothRuntimeState.Paused,s.Id);else if(action=="Resumed")runtime.Transition(s.OrganizationId.Value,ManagedBoothRuntimeState.Active,s.Id);else if(action is "Completed" or "Cancelled"){var state=runtime.Get(s.OrganizationId.Value);if(state.State is ManagedBoothRuntimeState.Active or ManagedBoothRuntimeState.Paused)runtime.Transition(s.OrganizationId.Value,ManagedBoothRuntimeState.Completing,s.Id);runtime.Transition(s.OrganizationId.Value,ManagedBoothRuntimeState.Ready);}}await unitOfWork.SaveChangesAsync(ct);return Map(s);}
    private async Task<BoothSession> Find(Guid id, CancellationToken ct) => await sessions.GetByIdAsync(id, ct) ?? throw new NotFoundException("Booth session was not found.");
    private static BoothSessionDto Map(BoothSession s) => new(s.Id, s.EventId, s.GuestId, s.Status, s.StartedAt, s.CompletedAt, s.CaptureMode, s.PhotoCount, s.Countdown, s.TemplateId){OrganizationId=s.OrganizationId,BoothId=s.BoothId,SessionToken=s.SessionToken,SessionType=s.SessionType,EndedAt=s.EndedAt,LastActivityAt=s.LastActivityAt,GuestName=s.GuestName,MetadataJson=s.MetadataJson};
}
