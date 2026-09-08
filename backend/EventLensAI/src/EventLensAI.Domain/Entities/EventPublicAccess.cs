using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;

public sealed class EventQRCode : BaseEntity
{
    private EventQRCode() { }
    public EventQRCode(Guid eventId, EventQRCodeType type, string token, string publicUrl, string imageStoragePath, string customizationJson, DateTime? expiresAt)
    {
        EventId = eventId; QRType = type; Token = Required(token); PublicUrl = Required(publicUrl);
        ImageStoragePath = Required(imageStoragePath); Update(customizationJson, expiresAt, true);
    }
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    public EventQRCodeType QRType { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public string PublicUrl { get; private set; } = string.Empty;
    public string ImageStoragePath { get; private set; } = string.Empty;
    public string CustomizationJson { get; private set; } = "{}";
    public DateTime? ExpiresAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsValid(DateTime utcNow) => IsActive && !IsDeleted && (!ExpiresAt.HasValue || ExpiresAt > utcNow);
    public void Update(string customizationJson, DateTime? expiresAt, bool active)
    {
        CustomizationJson = string.IsNullOrWhiteSpace(customizationJson) ? "{}" : customizationJson;
        ExpiresAt = expiresAt?.ToUniversalTime(); IsActive = active;
    }
    public void SetImage(string storagePath) => ImageStoragePath = Required(storagePath);
    public void Archive(Guid actor) { IsActive = false; SoftDelete(actor); }
    private static string Required(string value) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("A value is required.") : value.Trim();
}

public sealed class EventAccessConfiguration : BaseEntity
{
    private EventAccessConfiguration() { }
    public EventAccessConfiguration(Guid eventId) => EventId = eventId;
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    public bool IsPublicEnabled { get; private set; }
    public bool RequirePassword { get; private set; }
    public string? PasswordHash { get; private set; }
    public bool AllowGuestAccess { get; private set; } = true;
    public bool AllowGalleryAccess { get; private set; }
    public bool AllowDownloads { get; private set; }
    public bool AllowSharing { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public void Update(bool enabled, bool requirePassword, string? passwordHash, bool guest, bool gallery, bool downloads, bool sharing, DateTime? expiry)
    {
        if (requirePassword && string.IsNullOrWhiteSpace(passwordHash) && string.IsNullOrWhiteSpace(PasswordHash))
            throw new ArgumentException("A public access password is required.");
        IsPublicEnabled = enabled; RequirePassword = requirePassword;
        if (!string.IsNullOrWhiteSpace(passwordHash)) PasswordHash = passwordHash;
        if (!requirePassword) PasswordHash = null;
        AllowGuestAccess = guest; AllowGalleryAccess = gallery; AllowDownloads = downloads; AllowSharing = sharing;
        ExpiryDate = expiry?.ToUniversalTime();
    }
    public bool IsAvailable(DateTime utcNow) => IsPublicEnabled && (!ExpiryDate.HasValue || ExpiryDate > utcNow);
}

public sealed class GuestSession : BaseEntity
{
    private GuestSession() { }
    public GuestSession(Guid eventId, string token, string? ipAddress, string? deviceInfo)
    { EventId = eventId; SessionToken = token; IPAddress = Clean(ipAddress); DeviceInfo = Clean(deviceInfo); StartedAt = LastActivityAt = DateTime.UtcNow; }
    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    public string SessionToken { get; private set; } = string.Empty;
    public string? IPAddress { get; private set; }
    public string? DeviceInfo { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime LastActivityAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public void Touch() => LastActivityAt = DateTime.UtcNow;
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class EventAccessLog : BaseEntity
{
    private EventAccessLog() { }
    public EventAccessLog(Guid eventId, Guid? qrCodeId, Guid? sessionId, EventAccessAction action, string? deviceType)
    { EventId = eventId; QRCodeId = qrCodeId; GuestSessionId = sessionId; Action = action; Timestamp = DateTime.UtcNow; DeviceType = string.IsNullOrWhiteSpace(deviceType) ? "Unknown" : deviceType.Trim(); }
    public Guid EventId { get; private set; }
    public Guid? QRCodeId { get; private set; }
    public Guid? GuestSessionId { get; private set; }
    public EventAccessAction Action { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string DeviceType { get; private set; } = "Unknown";
}
