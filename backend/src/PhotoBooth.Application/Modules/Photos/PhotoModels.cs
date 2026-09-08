namespace PhotoBooth.Application.Modules.Photos;

public sealed record PhotoResponse(Guid Id, Guid EventId, Guid TemplateId, string StorageUrl);
public sealed record CreatePhotoRequest(Guid EventId, Guid TemplateId, string StorageUrl);
