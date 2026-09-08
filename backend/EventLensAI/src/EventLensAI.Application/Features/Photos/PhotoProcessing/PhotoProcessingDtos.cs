using System.Text.Json;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Application.Features.Photos.PhotoProcessing;

public sealed record PhotoDto(Guid Id, Guid EventId, Guid? SessionId, string OriginalImageUrl,
    string? ProcessedImageUrl, string? ThumbnailUrl, int Width, int Height, long FileSize,
    PhotoFormat Format, PhotoProcessingStatus Status, PhotoProcessingType ProcessingType, DateTime CreatedAt);
public sealed record RegisterPhotoRequest(Guid EventId, Guid? SessionId, string OriginalImageUrl,
    int Width, int Height, long FileSize, PhotoFormat Format);
public sealed record ProcessPhotoRequest(Guid PhotoId, PhotoProcessingType ProcessingType, RenderDocument Document);
public sealed record ExportPhotoRequest(Guid PhotoId, PhotoFormat Format, ExportPreset Preset, int Quality = 92);
public sealed record RenderRequest(Guid EventId, Guid? SessionId, IReadOnlyList<Guid> PhotoIds, RenderDocument Document, PhotoFormat Format);
public sealed record RenderResult(Guid PhotoId, string Url, string? ThumbnailUrl, int Width, int Height, long FileSize, PhotoFormat Format);
public sealed record ProcessingStatusDto(Guid PhotoId, PhotoProcessingStatus Status, string? Url, string? Error);
public enum ExportPreset { HighQuality, Instagram, WhatsApp, A4Print, PhotoBoothPrint }
public enum StripLayout { Single, Vertical, Horizontal, Grid, Polaroid, Magazine }
public sealed record RenderDocument(int Width, int Height, string Background, StripLayout Layout,
    int Padding, int Spacing, int BorderRadius, string BorderColor, bool Shadow,
    IReadOnlyList<RenderLayer> Layers);
public sealed record RenderLayer(string Id, string Type, int Order, double X, double Y,
    double Width, double Height, double Rotation, double Opacity, JsonElement Properties);
