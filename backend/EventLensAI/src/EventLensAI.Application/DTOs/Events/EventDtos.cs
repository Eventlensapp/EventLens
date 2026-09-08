using EventLensAI.Domain.Enums;

namespace EventLensAI.Application.DTOs.Events;

public sealed record UpsertEventRequest(
    string Name, string? Description, EventType EventType, string? Venue, string? Address,
    decimal? Latitude, decimal? Longitude, DateTime StartDate, DateTime EndDate,
    string? CoverImage, string? Logo, string PrimaryColor, string SecondaryColor,
    int? GuestLimit, int? PhotoLimit, long? StorageLimit, bool PublicGalleryEnabled,
    bool RequireGuestRegistration, bool AllowDownloads, bool AllowSocialSharing, bool EnableAI, bool EnableQRCode);
public sealed record EventDto(
    Guid Id, Guid OrganizationId, string Name, string Slug, string? Description, EventType EventType,
    string? Venue, string? Address, decimal? Latitude, decimal? Longitude, DateTime StartDate,
    DateTime EndDate, string? CoverImage, string? Logo, string PrimaryColor, string SecondaryColor,
    EventStatus Status, int? GuestLimit, int? PhotoLimit, long? StorageLimit,
    bool PublicGalleryEnabled, bool RequireGuestRegistration, bool AllowDownloads,
    bool AllowSocialSharing, bool EnableAI, bool EnableQRCode, string? PublicUrl, string? QRCodeUrl,
    Guid? EventTypeId=null, Guid? BranchId=null, Guid? AssignedManagerId=null, Guid? BrandProfileId=null,
    string Timezone="UTC", string? City=null, string? Country=null, string? ContactPerson=null,
    string? ContactEmail=null, string? ContactPhone=null);
public sealed record EventSearchRequest(
    Guid? OrganizationId, string? Search, EventStatus? Status, EventType? EventType,
    DateTime? FromDate, DateTime? ToDate, int Page = 1, int PageSize = 20,
    string SortBy = "startDate", bool Descending = false);
public sealed record UpdateEventSettingsRequest(
    int Countdown, CaptureMode CaptureMode, Guid? TemplateId, string Language,
    string? Watermark, string? DefaultFilter, bool PrintEnabled, bool GIFEnabled,
    bool BoomerangEnabled, bool VideoEnabled, bool AIEnabled,
    bool BackgroundRemovalEnabled, bool FaceDetectionEnabled);
public sealed record EventSettingsDto(
    Guid EventId, int Countdown, CaptureMode CaptureMode, Guid? TemplateId,
    string Language, string? Watermark, string? DefaultFilter, bool PrintEnabled,
    bool GIFEnabled, bool BoomerangEnabled, bool VideoEnabled, bool AIEnabled,
    bool BackgroundRemovalEnabled, bool FaceDetectionEnabled);
public sealed record DuplicateEventRequest(string Name, DateTime StartDate, DateTime EndDate);
public sealed record QrCodeDto(string PublicUrl, string QRCodeUrl);

public sealed record CreateEventRequest(
    Guid OrganizationId, string Name, string? Description, Guid EventTypeId, EventStatus Status,
    DateTime StartDate, DateTime EndDate, string Timezone, Guid? BranchId, string? VenueName,
    string? Address, string? City, string? Country, string? ContactPerson, string? ContactEmail,
    string? ContactPhone, Guid? AssignedManagerId, Guid? BrandProfileId, Guid? TemplateId=null);
public sealed record UpdateEventCoreRequest(
    string Name, string? Description, Guid EventTypeId, EventStatus Status,
    DateTime StartDate, DateTime EndDate, string Timezone, Guid? BranchId, string? VenueName,
    string? Address, string? City, string? Country, string? ContactPerson, string? ContactEmail,
    string? ContactPhone, Guid? AssignedManagerId, Guid? BrandProfileId);
public sealed record CloneEventRequest(string Name, DateTime StartDate, DateTime EndDate);
public sealed record ChangeEventStatusRequest(EventStatus Status);
public sealed record EventTypeDto(Guid Id, string Name, string? Description, string? Icon, string Color, bool IsSystemType, bool IsActive, Guid? OrganizationId);
public sealed record CreateEventTypeRequest(Guid OrganizationId,string Name,string?Description,string?Icon,string Color);
public sealed record UpdateEventTypeRequest(string Name,string?Description,string?Icon,string Color,bool IsActive);
public sealed record EventTemplateConfiguration(bool BoothEnabled,bool GalleryEnabled,bool PrintingEnabled,bool AIEnabled,bool CRMEnabled);
public sealed record EventTemplateDto(Guid Id,Guid OrganizationId,string Name,string?Description,Guid EventTypeId,string EventTypeName,
    Guid?DefaultBrandProfileId,string?DefaultBrandProfileName,int DefaultDurationMinutes,EventTemplateConfiguration Configuration,
    bool IsSystemTemplate,bool IsActive,DateTime CreatedAt);
public sealed record CreateEventTemplateRequest(Guid OrganizationId,string Name,string?Description,Guid EventTypeId,
    Guid?DefaultBrandProfileId,int DefaultDurationMinutes,EventTemplateConfiguration Configuration);
public sealed record UpdateEventTemplateRequest(string Name,string?Description,Guid EventTypeId,
    Guid?DefaultBrandProfileId,int DefaultDurationMinutes,EventTemplateConfiguration Configuration,bool IsActive);
public sealed record CloneEventTemplateRequest(string Name);
public sealed record EventMemberDto(Guid UserId, string FirstName, string LastName, string Email, EventMemberRole Role);
public sealed record AssignEventMemberRequest(Guid UserId, EventMemberRole Role);
public sealed record ChangeEventMemberRoleRequest(EventMemberRole Role);
public sealed record EventDashboardDto(EventDto Event, string OrganizationName, string? BranchName,
    string? EventTypeName, EventMemberDto? AssignedManager, IReadOnlyList<EventMemberDto> AssignedTeam,
    int PhotoCount, int GuestCount, string GalleryStatus, string BoothStatus, int AIJobs);
