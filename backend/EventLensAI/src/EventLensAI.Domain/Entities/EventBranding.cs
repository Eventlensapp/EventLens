using EventLensAI.Domain.Common;

namespace EventLensAI.Domain.Entities;

public sealed class EventBranding : BaseEntity
{
    private EventBranding() { }
    public EventBranding(Guid eventId) => EventId = eventId;
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    public string? Logo { get; private set; }
    public string? Watermark { get; private set; }
    public string? Background { get; private set; }
    public string? SplashScreen { get; private set; }
    public string? BrandFontsJson { get; private set; }
    public string? BrandColorsJson { get; private set; }
    public string? CustomCss { get; private set; }
}
