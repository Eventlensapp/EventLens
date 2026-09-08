using EventLensAI.Domain.Enums;

namespace EventLensAI.Application.Features.AI;

public sealed record CreateAIJobRequest(Guid EventId,Guid? SessionId,Guid? PhotoId,string InputImage,
    string? Prompt,string? NegativePrompt,string? Style,string? Background,string? MaskImage,
    int? UpscaleFactor,IReadOnlyList<string>? Props);
public sealed record MemoryBookRequest(Guid EventId,string? Prompt,bool IncludeTimeline,bool IncludeStory,bool IncludeHighlightMetadata);
public sealed record AIJobDto(Guid Id,Guid EventId,Guid? SessionId,Guid? PhotoId,AIJobType JobType,
    string Provider,AIJobStatus Status,int Progress,string? Prompt,string InputImage,string? OutputImage,
    DateTime? StartedAt,DateTime? CompletedAt,string? ErrorMessage,int RetryCount);
public sealed record AIProviderRequest(Guid JobId,AIJobType JobType,string InputImage,string? Prompt,
    string? NegativePrompt,IReadOnlyDictionary<string,string> Parameters);
public sealed record AIProviderResult(string OutputImage,IReadOnlyDictionary<string,string>? Metadata=null);
public sealed record SmartPhotoScore(Guid PhotoId,double Smile,double EyesOpen,double Sharpness,
    double FaceVisibility,double Pose,double Composition,double Total,PhotoQualityRating Rating);

public interface IAIProvider
{
    string Name{get;}
    IReadOnlySet<AIJobType> SupportedJobTypes{get;}
    Task<AIProviderResult> ProcessAsync(AIProviderRequest request,IProgress<int> progress,CancellationToken ct);
}
public interface IAIProviderResolver{IAIProvider Resolve(string? providerName,AIJobType jobType);}
public interface IAIProviderSelection{string Select(AIJobType jobType);}
public interface IAIJobQueue{ValueTask QueueAsync(Guid jobId,CancellationToken ct);ValueTask<Guid> DequeueAsync(CancellationToken ct);}
public interface IAIJobNotifier
{
    Task StartedAsync(AIJobDto job,CancellationToken ct);
    Task ProgressAsync(Guid eventId,Guid jobId,int progress,CancellationToken ct);
    Task CompletedAsync(AIJobDto job,CancellationToken ct);
    Task FailedAsync(AIJobDto job,CancellationToken ct);
}
