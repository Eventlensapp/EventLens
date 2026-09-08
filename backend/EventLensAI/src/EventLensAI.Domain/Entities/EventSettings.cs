using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;

public sealed class EventSettings : BaseEntity
{
    private EventSettings() { }
    public EventSettings(Guid eventId) => EventId = eventId;
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    public int Countdown { get; private set; } = 3;
    public CaptureMode CaptureMode { get; private set; } = CaptureMode.SinglePhoto;
    public Guid? TemplateId { get; private set; }
    public string Language { get; private set; } = "en";
    public string? Watermark { get; private set; }
    public string? DefaultFilter { get; private set; }
    public bool PrintEnabled { get; private set; }
    public bool GIFEnabled { get; private set; }
    public bool BoomerangEnabled { get; private set; }
    public bool VideoEnabled { get; private set; }
    public bool AIEnabled { get; private set; }
    public bool BackgroundRemovalEnabled { get; private set; }
    public bool FaceDetectionEnabled { get; private set; }
    public void Update(int countdown, CaptureMode mode, Guid? templateId, string language, string? watermark,
        string? filter, bool print, bool gif, bool boomerang, bool video, bool ai, bool backgroundRemoval, bool faceDetection)
    {
        Countdown = countdown; CaptureMode = mode; TemplateId = templateId; Language = language;
        Watermark = watermark; DefaultFilter = filter; PrintEnabled = print; GIFEnabled = gif;
        BoomerangEnabled = boomerang; VideoEnabled = video; AIEnabled = ai;
        BackgroundRemovalEnabled = backgroundRemoval; FaceDetectionEnabled = faceDetection;
    }
}
