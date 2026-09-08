using PhotoBooth.Domain.Common;

namespace PhotoBooth.Domain.Entities;

public sealed class Photo : Entity
{
    private Photo() { }
    public Photo(Guid eventId, Guid templateId, string storageUrl)
    {
        EventId = eventId;
        TemplateId = templateId;
        StorageUrl = string.IsNullOrWhiteSpace(storageUrl)
            ? throw new ArgumentException("Storage URL is required.")
            : storageUrl;
    }

    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    public Guid TemplateId { get; private set; }
    public Template Template { get; private set; } = null!;
    public string StorageUrl { get; private set; } = string.Empty;
}
