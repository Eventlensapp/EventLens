using EventLensAI.Domain.Enums;

namespace EventLensAI.Application.Features.Photos.Templates;

public sealed record TemplateDto(Guid Id, Guid? OrganizationId, string Name, TemplateCategory Category,
    string? PreviewImage, string ConfigurationJson, bool IsPremium, Guid? CreatedBy, DateTime CreatedAt);
public sealed record UpsertTemplateRequest(Guid OrganizationId, string Name, TemplateCategory Category,
    string? PreviewImage, string ConfigurationJson, bool IsPremium);
