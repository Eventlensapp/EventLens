namespace EventLensAI.Domain.Enums;

public enum SubscriptionPlan { Free=0, Creator=1, Professional=2, Business=3, Enterprise=4, CustomEnterprise=5 }
public enum OrganizationType { PhotographyStudio, EventAgency, WeddingPlanner, Corporate, Education, BrandActivation, Venue, Other }
public enum OrganizationStatus { Active, Archived }
public enum OrganizationMemberStatus { Active, Suspended }
public enum OrganizationInvitationStatus { Pending, Accepted, Expired, Cancelled }
public enum OwnershipTransferStatus { Pending, Accepted, Cancelled, Expired }
public enum OrganizationalUnitStatus { Active, Inactive, Archived }
public enum BrandAssetType { Logo, Watermark, Background, Frame, Icon, Pattern, Decoration }
public enum BrandLogoType { Primary, Dark, Light, Icon, Favicon }
public enum BrandThemeKind { Theme, Preset }
public enum StorageFileCategory { BrandAsset, Template, BoothPhoto, GalleryPhoto, AIAsset, Report, CRMFile, QRImage, Video, Document, Other }
public enum StorageVisibility { Private, Organization, Public }
public enum StorageScanStatus { Pending, Clean, Rejected, Failed }
public enum EntitlementSubscriptionStatus { Trial, Active, Expired, Cancelled, Suspended, Pending }
public enum SubscriptionStatus { Trialing, Active, PastDue, Paused, GracePeriod, Canceled, Expired }
public enum EventType { Wedding, Birthday, Corporate, Graduation, Festival, BrandActivation, Conference, Private, School, Exhibition, ProductLaunch }
public enum EventStatus
{
    Draft = 0,
    Upcoming = 1,
    Published = Upcoming,
    Active = 2,
    Running = Active,
    Completed = 3,
    Cancelled = 4,
    Archived = 5
}
public enum EventTypeStatus { Active, Inactive }
public enum EventMemberRole { EventOwner, EventManager, Photographer, BoothOperator, Designer, MarketingManager, Viewer }
public enum EventAssetType { Logo, SponsorLogo, Background, Watermark, Frame, Sticker, Other }
public enum EventThemeMode { Inherit, Light, Dark, Custom }
public enum EventScheduleStatus { Planned, Confirmed, InProgress, Completed, Cancelled }
public enum ChecklistPriority { Low, Normal, High, Critical }
public enum ChecklistItemStatus { Pending, InProgress, Completed, Blocked, Cancelled }
public enum EventStaffRole { EventManager, BoothOperator, Photographer, Designer, MarketingManager, TechnicalSupport }
public enum StaffAssignmentStatus { Scheduled, Confirmed, CheckedIn, Completed, Cancelled }
public enum BoothPlacementStatus { Planned, SetupRequired, Ready, Active, Removed }
public enum EventNotificationType { TaskAssigned, TaskCompleted, ScheduleReminder, StaffAssigned, EventReminder, System }
public enum EventQRCodeType { EventLanding, Booth, Gallery }
public enum EventAccessAction { QRScan, PageView, SessionStart }
public enum CaptureMode { SinglePhoto, Photo=SinglePhoto, TwoPhotoStrip, ThreePhotoStrip, FourPhotoStrip, Gif, GIF=Gif, Boomerang, ShortVideo, Video=ShortVideo, Burst, SlowMotion, TimeLapse, LivePhoto }
public enum MediaCaptureStatus { Recording, Processing, Completed, Failed, Cancelled }
public enum MediaProcessingJobStatus { Queued, Processing, Completed, Failed, Cancelled }
public enum CameraProviderType { Browser, Canon, Nikon, Sony, GenericDSLR, GenericMirrorless }
public enum ProfessionalCameraStatus { Available, Connected, Disconnected, Error, Maintenance }
public enum ProfessionalCameraEventType { CameraConnected, CameraDisconnected, CameraError, CaptureStarted, CaptureCompleted, SettingsChanged }
public enum PhotoTemplateCategory { Wedding, Birthday, Corporate, Graduation, Festival, Brand, Minimal, Luxury }
public enum TemplateLayoutType { PhotoStrip, SinglePhoto, Grid, Collage }
public enum TemplateElementType { Photo, Text, Logo, Shape, Sticker, Date, QRCode }
public enum StickerCategory { Party, Wedding, Birthday, Festival }
public enum GuestBoothState { Attract, Welcome, ModeSelection, Preparation, Countdown, Capturing, Preview, TemplateSelection, Rendering, Completed, Error }
public enum BoothAnimationType { Gradient, Particles, Spotlight, None }
public enum BoothSessionStatus { Pending, Active, Capturing, Reviewing, Completed, Abandoned, Created, Initializing, Paused, Expired, Cancelled, Error }
public enum BoothSessionType { Guest, Operator, Test }
public enum CaptureProcessingStatus{Captured,Queued,Processing,Completed,Failed}
public enum CaptureWorkflowState{Idle,Preparing,Countdown,Capturing,Completed,Failed,Cancelled}
public enum CaptureQueueState{Queued,Capturing,Saved,Failed}
public enum PhotoProcessingStatus { Captured, Processing, Completed, Failed }
public enum PhotoProcessingType { Original, Strip, Template, Export }
public enum PhotoFormat { Jpeg, Png, Pdf }
public enum TemplateCategory { Wedding, Birthday, Corporate, Graduation, Festival, Minimal, Retro, Luxury }
public enum AIJobType { BackgroundRemoval, BackgroundReplacement, StyleTransfer, FaceEnhancement, ImageUpscale, NoiseReduction, AIProps, MagicErase, GenerativeFill, CollageGeneration, HighlightGeneration, MemoryBook, TemplateGeneration, SmartPhotoSelection }
public enum AIJobStatus { Queued, Running, Completed, Failed, RetryScheduled, Cancelled }
public enum AIProviderKind { OpenAI, StabilityAI, GoogleGemini, AzureOpenAI, ComfyUI, LocalStableDiffusion, Custom }
public enum PhotoQualityRating { BestShot, Good, Discard }
public enum BackgroundCategory { Wedding, Birthday, Corporate, Nature, Beach, Mountains, Festival, Luxury, Minimal, Space, Christmas, Halloween, NewYear }
public enum AuditAction { Create, Update, Delete, Publish, Archive, Invite, RemoveMember, ChangeRole, AIRequest, SubscriptionChange, Refund, GrantCredits, Suspend }
