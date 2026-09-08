using EventLensAI.Domain.Common;

namespace EventLensAI.Domain.Entities;

public sealed class Gallery : BaseEntity
{
    private Gallery() { }
    public Gallery(Guid eventId, string publicUrl, string qrCode, bool isPrivate)
    {
        EventId = eventId;
        PublicUrl = publicUrl;
        QRCode = qrCode;
        IsPrivate = isPrivate;
    }
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    public string PublicUrl { get; private set; } = string.Empty;
    public string QRCode { get; private set; } = string.Empty;
    public bool IsPrivate { get; private set; }
}
