using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;

public sealed class Event : BaseEntity
{
    private Event() { }
    public Event(Guid organizationId, string name, string slug, EventType eventType,
        DateTime startDate, DateTime endDate, Guid creatorId)
    {
        OrganizationId = organizationId; Name = name.Trim(); Slug = slug.Trim().ToLowerInvariant();
        EventType = eventType; SetDates(startDate, endDate); CreatedBy = creatorId;
    }
    public Guid OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public Guid? BranchId { get; private set; }
    public Branch? Branch { get; private set; }
    public Guid? EventTypeId { get; private set; }
    public EventTypeDefinition? EventTypeDefinition { get; private set; }
    public Guid? AssignedManagerId { get; private set; }
    public User? AssignedManager { get; private set; }
    public Guid? BrandProfileId { get; private set; }
    public BrandTheme? BrandProfile { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public EventType EventType { get; private set; }
    public string? Venue { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? Country { get; private set; }
    public string Timezone { get; private set; } = "UTC";
    public string? ContactPerson { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? ContactPhone { get; private set; }
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string? CoverImage { get; private set; }
    public string? Logo { get; private set; }
    public string PrimaryColor { get; private set; } = "#111111";
    public string SecondaryColor { get; private set; } = "#FFFFFF";
    public EventStatus Status { get; private set; } = EventStatus.Draft;
    public int? GuestLimit { get; private set; }
    public int? PhotoLimit { get; private set; }
    public long? StorageLimit { get; private set; }
    public bool PublicGalleryEnabled { get; private set; } = true;
    public bool RequireGuestRegistration { get; private set; }
    public bool AllowDownloads { get; private set; } = true;
    public bool AllowSocialSharing { get; private set; } = true;
    public bool EnableAI { get; private set; }
    public bool EnableQRCode { get; private set; } = true;
    public string? PublicUrl { get; private set; }
    public string? QRCodeUrl { get; private set; }
    public string? QRCodeSvg { get; private set; }
    public EventSettings Settings { get; private set; } = null!;
    public EventBranding Branding { get; private set; } = null!;
    public ICollection<Photo> Photos { get; private set; } = [];
    public ICollection<Gallery> Galleries { get; private set; } = [];
    public ICollection<EventMember> Members { get; private set; } = [];

    public void Update(string name, string? description, EventType type, string? venue, string? address,
        decimal? latitude, decimal? longitude, DateTime start, DateTime end, string? coverImage, string? logo,
        string primary, string secondary, int? guestLimit, int? photoLimit, long? storageLimit,
        bool publicGallery, bool requireRegistration, bool downloads, bool sharing, bool ai, bool qr)
    {
        Name = name.Trim(); Description = description?.Trim(); EventType = type; Venue = venue?.Trim();
        Address = address?.Trim(); Latitude = latitude; Longitude = longitude; SetDates(start, end);
        CoverImage = coverImage; Logo = logo; PrimaryColor = primary; SecondaryColor = secondary;
        GuestLimit = guestLimit; PhotoLimit = photoLimit; StorageLimit = storageLimit;
        PublicGalleryEnabled = publicGallery; RequireGuestRegistration = requireRegistration;
        AllowDownloads = downloads; AllowSocialSharing = sharing; EnableAI = ai; EnableQRCode = qr;
    }
    public void Publish() => Status = EventStatus.Published;
    public void ChangeStatus(EventStatus next)
    {
        var valid = (Status, next) switch
        {
            (EventStatus.Draft, EventStatus.Upcoming or EventStatus.Cancelled or EventStatus.Archived) => true,
            (EventStatus.Upcoming, EventStatus.Active or EventStatus.Cancelled or EventStatus.Archived) => true,
            (EventStatus.Active, EventStatus.Completed or EventStatus.Cancelled or EventStatus.Archived) => true,
            (EventStatus.Completed or EventStatus.Cancelled, EventStatus.Archived) => true,
            _ when Status == next => true,
            _ => false
        };
        if (!valid) throw new InvalidOperationException($"Cannot change event status from {Status} to {next}.");
        Status = next;
    }
    public void Archive() { ChangeStatus(EventStatus.Archived); }
    public void RestoreFromArchive(Guid actorId)
    {
        if (Status != EventStatus.Archived) throw new InvalidOperationException("Only archived events can be restored.");
        Restore(actorId); Status = StartDate > DateTime.UtcNow ? EventStatus.Upcoming : EventStatus.Draft;
    }
    public void ConfigureCore(Guid eventTypeId, Guid? branchId, Guid? managerId, Guid? brandProfileId,
        string timezone, string? venueName, string? address, string? city, string? country,
        string? contactPerson, string? contactEmail, string? contactPhone)
    {
        EventTypeId = eventTypeId; BranchId = branchId; AssignedManagerId = managerId; BrandProfileId = brandProfileId;
        Timezone = string.IsNullOrWhiteSpace(timezone) ? "UTC" : timezone.Trim();
        Venue = Clean(venueName); Address = Clean(address); City = Clean(city); Country = Clean(country);
        ContactPerson = Clean(contactPerson); ContactEmail = Clean(contactEmail); ContactPhone = Clean(contactPhone);
    }
    public void InitializeComponents()
    {
        Settings ??= new EventSettings(Id);
        Branding ??= new EventBranding(Id);
    }
    public void SetPublicAccess(string publicUrl, string qrCodeUrl, string qrCodeSvg)
    { PublicUrl = publicUrl; QRCodeUrl = qrCodeUrl; QRCodeSvg = qrCodeSvg; }
    private void SetDates(DateTime start, DateTime end)
    {
        if (end <= start || end - start > TimeSpan.FromDays(31)) throw new ArgumentException("Event dates are invalid.");
        StartDate = start.ToUniversalTime(); EndDate = end.ToUniversalTime();
    }
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
