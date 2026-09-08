using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;
using EventLensAI.Application.Features.Billing;

namespace EventLensAI.Application.Features.AI;

public interface IAIProcessingService
{
    Task<AIJobDto> QueueAsync(AIJobType type,CreateAIJobRequest request,CancellationToken ct);
    Task<AIJobDto> QueueMemoryBookAsync(MemoryBookRequest request,CancellationToken ct);
    Task<IReadOnlyList<AIJobDto>> ListAsync(Guid? eventId,AIJobStatus? status,CancellationToken ct);
    Task<AIJobDto> GetAsync(Guid id,CancellationToken ct);
    Task<AIJobDto> RetryAsync(Guid id,CancellationToken ct);
}
public sealed class AIProcessingService(IAIJobRepository jobs,IEventRepository events,
    IOrganizationRepository organizations,IAIPromptRepository prompts,IAIJobQueue queue,
    ICurrentUserService currentUser,IUnitOfWork unitOfWork,IAIProviderSelection providerSelection,
    IAuditRepository audit,IFeaturePermissionService features):IAIProcessingService
{
    public async Task<AIJobDto> QueueAsync(AIJobType type,CreateAIJobRequest request,CancellationToken ct)
    {
        var actor=await Authorize(request.EventId,ct);await EnforceQuota(request.EventId,ct);
        if(string.IsNullOrWhiteSpace(request.InputImage))throw new ConflictException("Input image is required.");
        var prompt=request.Prompt;
        if(string.IsNullOrWhiteSpace(prompt)){var stored=await prompts.GetAsync(type.ToString(),type,ct);prompt=stored?.Prompt;}
        var provider=providerSelection.Select(type);
        var job=new AIJob(request.EventId,request.SessionId,request.PhotoId,type,provider,prompt,request.NegativePrompt,request.InputImage,actor);
        await jobs.AddAsync(job,ct);await audit.AddAsync(new AuditLog(actor,"AIJob",job.Id,AuditAction.AIRequest,currentUser.IPAddress),ct);
        await unitOfWork.SaveChangesAsync(ct);await TrackQuota(request.EventId,ct);await queue.QueueAsync(job.Id,ct);return Map(job);
    }
    public Task<AIJobDto> QueueMemoryBookAsync(MemoryBookRequest request,CancellationToken ct)=>
        QueueAsync(AIJobType.MemoryBook,new(request.EventId,null,null,"event://all",request.Prompt,null,null,null,null,null,null),ct);
    public async Task<IReadOnlyList<AIJobDto>> ListAsync(Guid? eventId,AIJobStatus? status,CancellationToken ct)
    {var user=currentUser.UserId??throw new UnauthorizedException("Authentication is required.");return (await jobs.ListAsync(user,eventId,status,ct)).Select(Map).ToArray();}
    public async Task<AIJobDto> GetAsync(Guid id,CancellationToken ct){var job=await Find(id,ct);await Authorize(job.EventId,ct);return Map(job);}
    public async Task<AIJobDto> RetryAsync(Guid id,CancellationToken ct){var job=await Find(id,ct);await Authorize(job.EventId,ct);if(job.Status!=AIJobStatus.Failed)throw new ConflictException("Only failed jobs can be retried.");job.Requeue();await unitOfWork.SaveChangesAsync(ct);await queue.QueueAsync(job.Id,ct);return Map(job);}
    private async Task<Guid> Authorize(Guid eventId,CancellationToken ct){var user=currentUser.UserId??throw new UnauthorizedException("Authentication is required.");var e=await events.GetAsync(eventId,ct)??throw new NotFoundException("Event was not found.");if(await organizations.GetMemberAsync(e.OrganizationId,user,ct) is null)throw new UnauthorizedException("You cannot use AI for this event.");return user;}
    private async Task EnforceQuota(Guid eventId,CancellationToken ct){var e=await events.GetAsync(eventId,ct)??throw new NotFoundException("Event was not found.");await features.EnsureAsync(e.OrganizationId,BillingFeature.AICredits,1,ct);}
    private async Task TrackQuota(Guid eventId,CancellationToken ct){var e=await events.GetAsync(eventId,ct)??throw new NotFoundException("Event was not found.");await features.TrackAsync(e.OrganizationId,UsageMetric.AIGenerations,1,ct);}
    private async Task<AIJob> Find(Guid id,CancellationToken ct)=>await jobs.GetAsync(id,ct)??throw new NotFoundException("AI job was not found.");
    public static AIJobDto Map(AIJob x)=>new(x.Id,x.EventId,x.SessionId,x.PhotoId,x.JobType,x.Provider,x.Status,x.Progress,x.Prompt,x.InputImage,x.OutputImage,x.StartedAt,x.CompletedAt,x.ErrorMessage,x.RetryCount);
}
