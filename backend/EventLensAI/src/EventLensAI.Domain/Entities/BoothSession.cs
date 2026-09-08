using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;
using System.Security.Cryptography;

namespace EventLensAI.Domain.Entities;

public sealed class BoothSession : BaseEntity
{
    private BoothSession() { }
    public BoothSession(Guid eventId, Guid? guestId, CaptureMode captureMode, int countdown, Guid? templateId)
    {
        EventId = eventId; GuestId = guestId; CaptureMode = captureMode; Countdown = countdown;
        TemplateId = templateId; Status = BoothSessionStatus.Active; StartedAt = DateTime.UtcNow;
        LastActivityAt = StartedAt;
    }
    public BoothSession(Guid organizationId,Guid eventId,Guid boothId,BoothSessionType sessionType,string? guestName,string? guestEmail,string deviceInformation,string metadataJson)
    {
        OrganizationId=organizationId;EventId=eventId;BoothId=boothId;SessionType=sessionType;GuestName=Clean(guestName);GuestEmail=Clean(guestEmail);
        DeviceInformation=deviceInformation;MetadataJson=metadataJson;SessionToken=Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).TrimEnd('=').Replace('+','-').Replace('/','_');
        Status=BoothSessionStatus.Created;LastActivityAt=DateTime.UtcNow;CaptureMode=CaptureMode.SinglePhoto;
    }
    public Guid SessionId => Id;
    public Guid EventId { get; private set; }
    public Guid? OrganizationId { get; private set; }
    public Guid? BoothId { get; private set; }
    public string? SessionToken { get; private set; }
    public BoothSessionType SessionType { get; private set; }=BoothSessionType.Guest;
    public Event Event { get; private set; } = null!;
    public Guid? GuestId { get; private set; }
    public BoothSessionStatus Status { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public DateTime LastActivityAt { get; private set; }
    public string DeviceInformation { get; private set; }="";
    public string? GuestName { get; private set; }
    public string? GuestEmail { get; private set; }
    public string MetadataJson { get; private set; }="{}";
    public CaptureMode CaptureMode { get; private set; }
    public int PhotoCount { get; private set; }
    public int Countdown { get; private set; }
    public Guid? TemplateId { get; private set; }
    public void BeginCapture() => Status = BoothSessionStatus.Capturing;
    public void RecordCapture(int count) { if (count < 1) throw new ArgumentOutOfRangeException(nameof(count)); PhotoCount += count; Status = BoothSessionStatus.Reviewing; }
    public void Complete() { Status = BoothSessionStatus.Completed; CompletedAt = DateTime.UtcNow; }
    public void Abandon() { Status = BoothSessionStatus.Abandoned; CompletedAt = DateTime.UtcNow; }
    public void StartManaged(){Move(BoothSessionStatus.Initializing,BoothSessionStatus.Created);StartedAt=DateTime.UtcNow;Touch();Move(BoothSessionStatus.Active,BoothSessionStatus.Initializing);}
    public void Pause(){Move(BoothSessionStatus.Paused,BoothSessionStatus.Active);}
    public void Resume(){Move(BoothSessionStatus.Active,BoothSessionStatus.Paused);}
    public void CompleteManaged(){Move(BoothSessionStatus.Completed,BoothSessionStatus.Active,BoothSessionStatus.Paused);EndedAt=CompletedAt=DateTime.UtcNow;}
    public void Cancel(){Move(BoothSessionStatus.Cancelled,BoothSessionStatus.Created,BoothSessionStatus.Initializing,BoothSessionStatus.Active,BoothSessionStatus.Paused,BoothSessionStatus.Error);EndedAt=DateTime.UtcNow;}
    public void Expire(){Move(BoothSessionStatus.Expired,BoothSessionStatus.Active,BoothSessionStatus.Paused,BoothSessionStatus.Initializing);EndedAt=DateTime.UtcNow;}
    public void Touch(){if(Status is BoothSessionStatus.Completed or BoothSessionStatus.Expired or BoothSessionStatus.Cancelled)throw new InvalidOperationException("A finished session cannot receive activity.");LastActivityAt=DateTime.UtcNow;}
    public bool CanRecover(DateTime now,int timeoutSeconds)=>(Status is BoothSessionStatus.Active or BoothSessionStatus.Paused or BoothSessionStatus.Initializing)&&LastActivityAt.AddSeconds(timeoutSeconds)>now;
    private void Move(BoothSessionStatus next,params BoothSessionStatus[] allowed){if(!allowed.Contains(Status))throw new InvalidOperationException($"Cannot move session from {Status} to {next}.");LastActivityAt=DateTime.UtcNow;Status=next;}
    private static string? Clean(string? value)=>string.IsNullOrWhiteSpace(value)?null:value.Trim();
}

public sealed class BoothSessionActivity : BaseEntity
{
    private BoothSessionActivity(){}
    public BoothSessionActivity(Guid sessionId,string action,string metadata){SessionId=sessionId;Action=action;Metadata=metadata;Timestamp=DateTime.UtcNow;}
    public Guid SessionId{get;private set;}public BoothSession Session{get;private set;}=null!;public string Action{get;private set;}="";public DateTime Timestamp{get;private set;}public string Metadata{get;private set;}="{}";
}
