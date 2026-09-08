using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;

public sealed class Photo : BaseEntity
{
    private Photo() { }
    public Photo(Guid eventId, Guid? sessionId, string originalImageUrl, int width, int height,
        long fileSize, PhotoFormat format)
    {
        if (string.IsNullOrWhiteSpace(originalImageUrl)) throw new ArgumentException("Original image URL is required.");
        if (width < 1 || height < 1) throw new ArgumentOutOfRangeException(nameof(width));
        if (fileSize < 0) throw new ArgumentOutOfRangeException(nameof(fileSize));
        EventId = eventId; SessionId = sessionId; OriginalImageUrl = originalImageUrl;
        Width = width; Height = height; FileSize = fileSize; Format = format;
    }
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    public Guid? SessionId { get; private set; }
    public BoothSession? Session { get; private set; }
    public string OriginalImageUrl { get; private set; } = string.Empty;
    public string? ProcessedImageUrl { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public long FileSize { get; private set; }
    public PhotoFormat Format { get; private set; }
    public PhotoProcessingStatus Status { get; private set; } = PhotoProcessingStatus.Captured;
    public PhotoProcessingType ProcessingType { get; private set; } = PhotoProcessingType.Original;
    public string? FailureReason { get; private set; }
    public void BeginProcessing(PhotoProcessingType type) { ProcessingType = type; Status = PhotoProcessingStatus.Processing; FailureReason = null; }
    public void Complete(string processedUrl, string? thumbnailUrl, int width, int height, long fileSize, PhotoFormat format)
    { ProcessedImageUrl = processedUrl; ThumbnailUrl = thumbnailUrl; Width = width; Height = height; FileSize = fileSize; Format = format; Status = PhotoProcessingStatus.Completed; }
    public void Fail(string reason) { FailureReason = reason[..Math.Min(reason.Length, 1000)]; Status = PhotoProcessingStatus.Failed; }
}
