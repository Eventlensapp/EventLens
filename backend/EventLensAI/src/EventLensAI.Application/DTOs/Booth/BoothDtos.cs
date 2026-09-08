using EventLensAI.Domain.Enums;

namespace EventLensAI.Application.DTOs.Booth;

public sealed record StartBoothSessionRequest(Guid EventId, Guid? GuestId, CaptureMode CaptureMode, int Countdown, Guid? TemplateId);
public sealed record BoothSessionCreateEnvelope(Guid EventId,Guid? OrganizationId=null,Guid? BoothId=null,BoothSessionType? SessionType=null,string? GuestName=null,string? GuestEmail=null,string? DeviceInformation=null,string? MetadataJson=null,Guid? GuestId=null,CaptureMode? CaptureMode=null,int? Countdown=null,Guid? TemplateId=null);
public sealed record RecordCaptureRequest(int PhotoCount);
public sealed record BoothSessionDto(Guid SessionId, Guid EventId, Guid? GuestId, BoothSessionStatus Status, DateTime StartedAt, DateTime? CompletedAt, CaptureMode CaptureMode, int PhotoCount, int Countdown, Guid? TemplateId)
{
    public Guid? OrganizationId{get;init;}public Guid? BoothId{get;init;}public string? SessionToken{get;init;}public BoothSessionType SessionType{get;init;}
    public DateTime? EndedAt{get;init;}public DateTime LastActivityAt{get;init;}public string? GuestName{get;init;}public string MetadataJson{get;init;}="{}";
}
public sealed record BoothBootstrapDto(Guid EventId, string EventSlug, string EventName, string PrimaryColor, string SecondaryColor, bool PublicGalleryEnabled, bool AllowDownloads, bool EnableQRCode, int DefaultCountdown, CaptureMode DefaultCaptureMode);
