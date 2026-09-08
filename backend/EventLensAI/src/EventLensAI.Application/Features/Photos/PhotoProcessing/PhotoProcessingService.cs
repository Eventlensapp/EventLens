using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;
using EventLensAI.Application.Features.Billing;

namespace EventLensAI.Application.Features.Photos.PhotoProcessing;

public interface IPhotoProcessingService
{
    Task<PhotoDto> GetAsync(Guid id, CancellationToken ct);
    Task<PhotoDto> RegisterAsync(RegisterPhotoRequest request, CancellationToken ct);
    Task<RenderResult> ProcessAsync(ProcessPhotoRequest request, CancellationToken ct);
    Task<RenderResult> ExportAsync(ExportPhotoRequest request, CancellationToken ct);
    Task<RenderResult> RenderAsync(RenderRequest request, CancellationToken ct);
    Task<ProcessingStatusDto> StatusAsync(Guid id, CancellationToken ct);
}

public sealed class PhotoProcessingService(IPhotoRepository photos, IEventRepository events,
    IOrganizationRepository organizations, IStorageService storage, IImageProcessingService renderer,
    ICurrentUserService currentUser, IUnitOfWork unitOfWork,IFeaturePermissionService features) : IPhotoProcessingService
{
    public async Task<PhotoDto> GetAsync(Guid id, CancellationToken ct) => Map(await AuthorizedPhoto(id, ct));
    public async Task<PhotoDto> RegisterAsync(RegisterPhotoRequest request, CancellationToken ct)
    {
        var organizationId=await AuthorizeEvent(request.EventId, ct);
        await features.EnsureAsync(organizationId,BillingFeature.Storage,request.FileSize,ct);
        var photo = new Photo(request.EventId, request.SessionId, request.OriginalImageUrl,
            request.Width, request.Height, request.FileSize, request.Format);
        await photos.AddAsync(photo, ct); await unitOfWork.SaveChangesAsync(ct);await features.TrackAsync(organizationId,UsageMetric.PhotosCaptured,1,ct);await features.TrackAsync(organizationId,UsageMetric.StorageConsumed,request.FileSize,ct); return Map(photo);
    }
    public async Task<RenderResult> ProcessAsync(ProcessPhotoRequest request, CancellationToken ct) =>
        await RenderExisting(await AuthorizedPhoto(request.PhotoId, ct), request.Document,
            request.ProcessingType, PhotoFormat.Jpeg, ExportPreset.HighQuality, 92, ct);
    public async Task<RenderResult> ExportAsync(ExportPhotoRequest request, CancellationToken ct)
    {
        var photo = await AuthorizedPhoto(request.PhotoId, ct);
        var document = new RenderDocument(photo.Width, photo.Height, "#FFFFFF", StripLayout.Single, 0, 0, 0, "#000000", false, []);
        return await RenderExisting(photo, document, PhotoProcessingType.Export, request.Format, request.Preset, request.Quality, ct);
    }
    public async Task<RenderResult> RenderAsync(RenderRequest request, CancellationToken ct)
    {
        await AuthorizeEvent(request.EventId, ct);
        if (request.PhotoIds.Count == 0) throw new ConflictException("At least one photo is required.");
        if (request.PhotoIds.Count > 4) throw new ConflictException("Photo strips support up to four photos.");
        var sourcePhotos = new List<Photo>();
        foreach (var id in request.PhotoIds) sourcePhotos.Add(await AuthorizedPhoto(id, ct));
        if (sourcePhotos.Any(x => x.EventId != request.EventId)) throw new UnauthorizedException("All photos must belong to the requested event.");
        return await RenderPhotos(sourcePhotos, request.Document, PhotoProcessingType.Strip,
            request.Format, ExportPreset.HighQuality, 94, ct);
    }
    public async Task<ProcessingStatusDto> StatusAsync(Guid id, CancellationToken ct)
    { var p = await AuthorizedPhoto(id, ct); return new(p.Id, p.Status, p.ProcessedImageUrl, p.FailureReason); }
    private async Task<RenderResult> RenderExisting(Photo photo, RenderDocument document, PhotoProcessingType type,
        PhotoFormat format, ExportPreset preset, int quality, CancellationToken ct)
    {
        return await RenderPhotos([photo], document, type, format, preset, quality, ct);
    }
    private async Task<RenderResult> RenderPhotos(IReadOnlyList<Photo> sourcePhotos, RenderDocument document,
        PhotoProcessingType type, PhotoFormat format, ExportPreset preset, int quality, CancellationToken ct)
    {
        var photo = sourcePhotos[0];
        photo.BeginProcessing(type); await unitOfWork.SaveChangesAsync(ct);
        try
        {
            var streams = new List<Stream>();
            try
            {
                foreach (var sourcePhoto in sourcePhotos) streams.Add(await storage.OpenReadAsync(sourcePhoto.OriginalImageUrl, ct));
                var output = await renderer.RenderAsync(new(streams, document, format, preset, Math.Clamp(quality, 40, 100)), ct);
                await using var content = output.Content;
                var key = await storage.UploadAsync(content, $"{photo.Id:N}.{output.Extension}", output.ContentType, ct);
                photo.Complete(storage.GetUrl(key), null, output.Width, output.Height, output.Length, format);
                await unitOfWork.SaveChangesAsync(ct);
                return new(photo.Id, photo.ProcessedImageUrl!, photo.ThumbnailUrl, output.Width, output.Height, output.Length, format);
            }
            finally { foreach (var stream in streams) await stream.DisposeAsync(); }
        }
        catch (Exception ex) { photo.Fail(ex.Message); await unitOfWork.SaveChangesAsync(ct); throw; }
    }
    private async Task<Photo> AuthorizedPhoto(Guid id, CancellationToken ct)
    { var photo = await photos.GetAsync(id, ct) ?? throw new NotFoundException("Photo was not found."); await AuthorizeEvent(photo.EventId, ct); return photo; }
    private async Task<Guid> AuthorizeEvent(Guid eventId, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Authentication is required.");
        var e = await events.GetAsync(eventId, ct) ?? throw new NotFoundException("Event was not found.");
        if (await organizations.GetMemberAsync(e.OrganizationId, userId, ct) is null && !currentUser.Roles.Contains(SystemRoles.SuperAdmin))
            throw new UnauthorizedException("You cannot access photos for this event.");
        return e.OrganizationId;
    }
    private static PhotoDto Map(Photo p) => new(p.Id, p.EventId, p.SessionId, p.OriginalImageUrl,
        p.ProcessedImageUrl, p.ThumbnailUrl, p.Width, p.Height, p.FileSize, p.Format,
        p.Status, p.ProcessingType, p.CreatedAt);
}
