using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;

public sealed class Organization : BaseEntity
{
    private Organization() { }
    public Organization(string name, string slug, Guid ownerId)
    {
        Update(name, slug, null, null, null, null, null, null, "UTC", "#111111", "#FFFFFF");
        CreatedBy = ownerId;
    }

    public string Name { get; private set; } = string.Empty;
    public OrganizationType OrganizationType { get; private set; } = OrganizationType.Other;
    public OrganizationStatus Status { get; private set; } = OrganizationStatus.Active;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Logo { get; private set; }
    public string? Website { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Address { get; private set; }
    public string? Country { get; private set; }
    public string TimeZone { get; private set; } = "UTC";
    public string PrimaryColor { get; private set; } = "#111111";
    public string SecondaryColor { get; private set; } = "#FFFFFF";
    public SubscriptionPlan Plan { get; private set; } = SubscriptionPlan.Free;
    public long StorageUsed { get; private set; }
    public long StorageLimit { get; private set; } = 1_073_741_824;
    public bool IsActive { get; private set; } = true;
    public ICollection<OrganizationMember> Members { get; private set; } = [];
    public ICollection<Event> Events { get; private set; } = [];
    public ICollection<Subscription> Subscriptions { get; private set; } = [];
    public ICollection<OrganizationInvitation> Invitations { get; private set; } = [];

    public void Update(string name, string slug, string? description, string? logo, string? website,
        string? email, string? phone, string? address, string? timeZone, string primaryColor, string secondaryColor)
    {
        Name = Required(name, 200); Slug = Required(slug, 100).ToLowerInvariant();
        Description = description?.Trim(); Logo = logo; Website = website; Email = email?.Trim().ToLowerInvariant();
        Phone = phone?.Trim(); Address = address?.Trim(); TimeZone = string.IsNullOrWhiteSpace(timeZone) ? "UTC" : timeZone;
        PrimaryColor = primaryColor; SecondaryColor = secondaryColor;
    }
    public void SetCountry(string? country) => Country = country?.Trim();
    public void SetType(OrganizationType type) => OrganizationType = type;
    public void Archive() { Status = OrganizationStatus.Archived; IsActive = false; }
    public void ApplyPlan(SubscriptionPlan plan,long storageLimit){Plan=plan;StorageLimit=storageLimit;}
    public void SetActive(bool active)=>IsActive=active;
    public void AddStorage(long bytes)=>StorageUsed=Math.Max(0,checked(StorageUsed+bytes));
    private static string Required(string value, int max) =>
        string.IsNullOrWhiteSpace(value) || value.Trim().Length > max ? throw new ArgumentException("Required value is invalid.") : value.Trim();
}
