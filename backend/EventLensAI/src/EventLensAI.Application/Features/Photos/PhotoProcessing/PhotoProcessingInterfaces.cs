using EventLensAI.Domain.Enums;

namespace EventLensAI.Application.Features.Photos.PhotoProcessing;

public interface IStorageService
{
    Task<string> UploadAsync(Stream content, string fileName, string contentType, CancellationToken ct);
    Task DeleteAsync(string storageKey, CancellationToken ct);
    string GetUrl(string storageKey);
    Task<Stream> OpenReadAsync(string storageKey, CancellationToken ct);
}
public sealed record ImageRenderInput(IReadOnlyList<Stream> Sources, RenderDocument Document, PhotoFormat Format, ExportPreset Preset, int Quality);
public sealed record ImageRenderOutput(Stream Content, int Width, int Height, long Length, string ContentType, string Extension);
public interface IImageProcessingService
{
    Task<ImageRenderOutput> RenderAsync(ImageRenderInput input, CancellationToken ct);
}
