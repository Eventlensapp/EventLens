using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;

public sealed class AIJob : BaseEntity
{
    private AIJob() { }
    public AIJob(Guid eventId, Guid? sessionId, Guid? photoId, AIJobType jobType, string provider,
        string? prompt, string? negativePrompt, string inputImage, Guid actorId)
    {
        EventId=eventId;SessionId=sessionId;PhotoId=photoId;JobType=jobType;Provider=provider;
        Prompt=prompt;NegativePrompt=negativePrompt;InputImage=inputImage;CreatedBy=actorId;
    }
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    public Guid? SessionId { get; private set; }
    public Guid? PhotoId { get; private set; }
    public AIJobType JobType { get; private set; }
    public string Provider { get; private set; } = string.Empty;
    public AIJobStatus Status { get; private set; } = AIJobStatus.Queued;
    public int Progress { get; private set; }
    public string? Prompt { get; private set; }
    public string? NegativePrompt { get; private set; }
    public string InputImage { get; private set; } = string.Empty;
    public string? OutputImage { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int RetryCount { get; private set; }
    public void Start(){Status=AIJobStatus.Running;StartedAt??=DateTime.UtcNow;Progress=1;ErrorMessage=null;}
    public void ReportProgress(int progress)=>Progress=Math.Clamp(progress,1,99);
    public void Complete(string output){OutputImage=output;Progress=100;Status=AIJobStatus.Completed;CompletedAt=DateTime.UtcNow;}
    public void Fail(string error,bool retry){ErrorMessage=error[..Math.Min(error.Length,2000)];RetryCount++;Status=retry?AIJobStatus.RetryScheduled:AIJobStatus.Failed;if(!retry)CompletedAt=DateTime.UtcNow;}
    public void Requeue(){Status=AIJobStatus.Queued;Progress=0;}
}
