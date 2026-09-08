using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;

public sealed class Template : BaseEntity
{
    private Template() { }
    public Template(Guid organizationId, string name, TemplateCategory category, string configurationJson, Guid creatorId)
    { OrganizationId = organizationId; Update(name, category, null, configurationJson, false); CreatedBy = creatorId; }
    public Guid? OrganizationId { get; private set; }
    public Organization? Organization { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public TemplateCategory Category { get; private set; }
    public string? PreviewImage { get; private set; }
    public string ConfigurationJson { get; private set; } = "{}";
    public bool IsPremium { get; private set; }
    public void Update(string name, TemplateCategory category, string? previewImage, string configurationJson, bool isPremium)
    {
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Template name is required.") : name.Trim();
        Category = category; PreviewImage = previewImage;
        ConfigurationJson = string.IsNullOrWhiteSpace(configurationJson) ? "{}" : configurationJson;
        IsPremium = isPremium;
    }
}
