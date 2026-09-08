using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;

public sealed class EventTypeDefinition : BaseEntity
{
    private EventTypeDefinition() { }
    public EventTypeDefinition(string name, string? description, string? icon, string color, bool isSystemType, Guid? organizationId)
    {
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Event type name is required.") : name.Trim();
        Description = Clean(description); Icon = Clean(icon); SetColor(color); IsSystemType = isSystemType; OrganizationId = organizationId;
        if (isSystemType && organizationId.HasValue) throw new ArgumentException("System event types cannot belong to an organization.");
    }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Icon { get; private set; }
    public string Color { get; private set; } = "#6C5CE7";
    public bool IsSystemType { get; private set; }
    public Guid? OrganizationId { get; private set; }
    public Organization? Organization { get; private set; }
    public EventTypeStatus Status { get; private set; } = EventTypeStatus.Active;
    public bool IsActive { get; private set; } = true;
    public ICollection<Event> Events { get; private set; } = [];
    public void Update(string name,string? description,string? icon,string color,bool isActive)
    {
        if(IsSystemType)throw new InvalidOperationException("System event types are read-only.");
        Name=string.IsNullOrWhiteSpace(name)?throw new ArgumentException("Event type name is required."):name.Trim();
        Description=Clean(description);Icon=Clean(icon);SetColor(color);IsActive=isActive;
        Status=isActive?EventTypeStatus.Active:EventTypeStatus.Inactive;
    }
    public void Archive(Guid actorId)
    {
        if(IsSystemType)throw new InvalidOperationException("System event types cannot be archived.");
        IsActive=false;Status=EventTypeStatus.Inactive;SoftDelete(actorId);
    }
    private void SetColor(string color)
    {
        if(string.IsNullOrWhiteSpace(color)||color.Length!=7||color[0]!='#')throw new ArgumentException("A valid hex color is required.");
        Color=color.ToUpperInvariant();
    }
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class EventTemplate : BaseEntity
{
    private EventTemplate() { }
    public EventTemplate(Guid organizationId,string name,string? description,Guid eventTypeId,Guid? defaultBrandProfileId,
        TimeSpan defaultDuration,string configurationJson,bool isSystemTemplate=false)
    {
        OrganizationId=organizationId;IsSystemTemplate=isSystemTemplate;
        UpdateInternal(name,description,eventTypeId,defaultBrandProfileId,defaultDuration,configurationJson,true);
    }
    public Guid OrganizationId{get;private set;} public Organization Organization{get;private set;}=null!;
    public string Name{get;private set;}=string.Empty;public string?Description{get;private set;}
    public Guid EventTypeId{get;private set;}public EventTypeDefinition EventType{get;private set;}=null!;
    public Guid?DefaultBrandProfileId{get;private set;}public BrandTheme?DefaultBrandProfile{get;private set;}
    public TimeSpan DefaultDuration{get;private set;}=TimeSpan.FromHours(4);
    public string ConfigurationJson{get;private set;}="{}";public bool IsSystemTemplate{get;private set;}
    public bool IsActive{get;private set;}=true;public ICollection<TemplateUsage>Usages{get;private set;}=[];
    public void Update(string name,string?description,Guid eventTypeId,Guid?brandProfileId,TimeSpan duration,string json,bool active)
    {if(IsSystemTemplate)throw new InvalidOperationException("System templates are read-only.");UpdateInternal(name,description,eventTypeId,brandProfileId,duration,json,active);}
    public EventTemplate Duplicate(string name,Guid organizationId)=>new(organizationId,name,Description,EventTypeId,DefaultBrandProfileId,DefaultDuration,ConfigurationJson);
    public void Archive(Guid actorId){if(IsSystemTemplate)throw new InvalidOperationException("System templates cannot be archived.");IsActive=false;SoftDelete(actorId);}
    private void UpdateInternal(string name,string?description,Guid type,Guid?brand,TimeSpan duration,string json,bool active)
    {
        if(string.IsNullOrWhiteSpace(name))throw new ArgumentException("Template name is required.");
        if(duration<=TimeSpan.Zero||duration>TimeSpan.FromDays(31))throw new ArgumentException("Default duration must be between one minute and 31 days.");
        Name=name.Trim();Description=Clean(description);EventTypeId=type;DefaultBrandProfileId=brand;DefaultDuration=duration;
        ConfigurationJson=string.IsNullOrWhiteSpace(json)?"{}":json;IsActive=active;
    }
    private static string?Clean(string?x)=>string.IsNullOrWhiteSpace(x)?null:x.Trim();
}

public sealed class TemplateUsage : BaseEntity
{
    private TemplateUsage(){}
    public TemplateUsage(Guid templateId,Guid eventId,Guid createdBy){TemplateId=templateId;EventId=eventId;CreatedBy=createdBy;}
    public Guid TemplateId{get;private set;}public EventTemplate Template{get;private set;}=null!;
    public Guid EventId{get;private set;}public Event Event{get;private set;}=null!;
}

public sealed class EventMember : BaseEntity
{
    private EventMember() { }
    public EventMember(Guid eventId, Guid userId, EventMemberRole role, Guid createdBy)
    { EventId = eventId; UserId = userId; Role = role; CreatedBy = createdBy; }
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public EventMemberRole Role { get; private set; }
    public void ChangeRole(EventMemberRole role, Guid actorId) { Role = role; MarkUpdated(actorId); }
}
