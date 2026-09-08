using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Domain.Common;
using EventLensAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventLensAI.Infrastructure.Persistence;

public sealed class EventLensDbContext(
    DbContextOptions<EventLensDbContext> options,
    ICurrentUserService? currentUser = null)
    : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<OrganizationMember> OrganizationMembers => Set<OrganizationMember>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventTypeDefinition> EventTypes => Set<EventTypeDefinition>();
    public DbSet<EventMember> EventMembers => Set<EventMember>();
    public DbSet<EventTemplate> EventTemplates => Set<EventTemplate>();
    public DbSet<TemplateUsage> EventTemplateUsages => Set<TemplateUsage>();
    public DbSet<EventBrandConfiguration> EventBrandConfigurations=>Set<EventBrandConfiguration>();
    public DbSet<EventExperienceSettings> EventExperienceSettings=>Set<EventExperienceSettings>();
    public DbSet<EventAsset> EventAssets=>Set<EventAsset>();
    public DbSet<EventSponsor> EventSponsors=>Set<EventSponsor>();
    public DbSet<VenueType> VenueTypes=>Set<VenueType>();public DbSet<Venue> Venues=>Set<Venue>();
    public DbSet<ScheduleType> ScheduleTypes=>Set<ScheduleType>();public DbSet<EventScheduleItem> EventScheduleItems=>Set<EventScheduleItem>();
    public DbSet<EventChecklist> EventChecklists=>Set<EventChecklist>();public DbSet<EventChecklistItem> EventChecklistItems=>Set<EventChecklistItem>();
    public DbSet<EventStaffAssignment> EventStaffAssignments=>Set<EventStaffAssignment>();public DbSet<EventZone> EventZones=>Set<EventZone>();
    public DbSet<BoothPlacement> BoothPlacements=>Set<BoothPlacement>();public DbSet<EventNotification> EventNotifications=>Set<EventNotification>();
    public DbSet<EventQRCode> EventQRCodes=>Set<EventQRCode>();public DbSet<EventAccessConfiguration> EventAccessConfigurations=>Set<EventAccessConfiguration>();
    public DbSet<GuestSession> GuestSessions=>Set<GuestSession>();public DbSet<EventAccessLog> EventAccessLogs=>Set<EventAccessLog>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<PhotoTemplate> PhotoTemplates => Set<PhotoTemplate>();
    public DbSet<TemplateLayout> TemplateLayouts => Set<TemplateLayout>();
    public DbSet<TemplateElement> TemplateElements => Set<TemplateElement>();
    public DbSet<Sticker> Stickers => Set<Sticker>();
    public DbSet<TemplateAssignment> TemplateAssignments => Set<TemplateAssignment>();
    public DbSet<Gallery> Galleries => Set<Gallery>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<OrganizationInvitation> OrganizationInvitations => Set<OrganizationInvitation>();
    public DbSet<EventSettings> EventSettings => Set<EventSettings>();
    public DbSet<EventBranding> EventBranding => Set<EventBranding>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<BoothSession> BoothSessions => Set<BoothSession>();
    public DbSet<BoothSessionActivity> BoothSessionActivities => Set<BoothSessionActivity>();
    public DbSet<BoothExperienceConfiguration> BoothExperienceConfigurations => Set<BoothExperienceConfiguration>();
    public DbSet<GuestBoothSession> GuestBoothSessions => Set<GuestBoothSession>();
    public DbSet<BoothExperienceLog> BoothExperienceLogs => Set<BoothExperienceLog>();
    public DbSet<BoothConfiguration> BoothConfigurations => Set<BoothConfiguration>();
    public DbSet<BoothCapability> BoothCapabilities => Set<BoothCapability>();
    public DbSet<BoothDevice> BoothDevices => Set<BoothDevice>();
    public DbSet<BoothHealthCheck> BoothHealthChecks => Set<BoothHealthCheck>();
    public DbSet<CameraPreference> CameraPreferences => Set<CameraPreference>();
    public DbSet<CaptureConfiguration> CaptureConfigurations=>Set<CaptureConfiguration>();
    public DbSet<CapturedPhoto> CapturedPhotos=>Set<CapturedPhoto>();
    public DbSet<CameraProfile> CameraProfiles=>Set<CameraProfile>();
    public DbSet<CameraSettingsHistory> CameraSettingsHistory=>Set<CameraSettingsHistory>();
    public DbSet<CapturedMedia> CapturedMedia=>Set<CapturedMedia>();
    public DbSet<MediaCaptureSettings> MediaCaptureSettings=>Set<MediaCaptureSettings>();
    public DbSet<MediaProcessingJob> MediaProcessingJobs=>Set<MediaProcessingJob>();
    public DbSet<ProfessionalCamera> ProfessionalCameras=>Set<ProfessionalCamera>();
    public DbSet<CameraAdapter> CameraAdapters=>Set<CameraAdapter>();
    public DbSet<CameraConnectionLog> CameraConnectionLogs=>Set<CameraConnectionLog>();
    public DbSet<CameraCapabilityProfile> CameraCapabilityProfiles=>Set<CameraCapabilityProfile>();
    public DbSet<ProfessionalCameraEvent> ProfessionalCameraEvents=>Set<ProfessionalCameraEvent>();
    public DbSet<AIJob> AIJobs => Set<AIJob>();
    public DbSet<AIPromptDefinition> AIPromptDefinitions => Set<AIPromptDefinition>();
    public DbSet<AIBackground> AIBackgrounds => Set<AIBackground>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<LeadForm> LeadForms => Set<LeadForm>();
    public DbSet<LeadResponse> LeadResponses => Set<LeadResponse>();
    public DbSet<GuestCheckIn> GuestCheckIns => Set<GuestCheckIn>();
    public DbSet<GuestActivity> GuestActivities => Set<GuestActivity>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<GuestTag> GuestTags => Set<GuestTag>();
    public DbSet<Segment> Segments => Set<Segment>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<CampaignLog> CampaignLogs => Set<CampaignLog>();
    public DbSet<EmailMessage> Emails => Set<EmailMessage>();
    public DbSet<SmsMessage> Sms => Set<SmsMessage>();
    public DbSet<WhatsAppMessage> WhatsAppMessages => Set<WhatsAppMessage>();
    public DbSet<Automation> Automations => Set<Automation>();
    public DbSet<WorkflowStep> WorkflowSteps => Set<WorkflowStep>();
    public DbSet<Survey> Surveys => Set<Survey>();
    public DbSet<SurveyResponse> SurveyResponses => Set<SurveyResponse>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<Referral> Referrals => Set<Referral>();
    public DbSet<AnalyticsRecord> AnalyticsRecords => Set<AnalyticsRecord>();
    public DbSet<AnalyticsDailyAggregate> AnalyticsDailyAggregates => Set<AnalyticsDailyAggregate>();
    public DbSet<PlanDefinition> PlanDefinitions => Set<PlanDefinition>();
    public DbSet<UsageCounter> UsageCounters => Set<UsageCounter>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<BillingCoupon> BillingCoupons => Set<BillingCoupon>();
    public DbSet<BillingWebhook> BillingWebhooks => Set<BillingWebhook>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<OrganizationMemberPermission> OrganizationMemberPermissions => Set<OrganizationMemberPermission>();
    public DbSet<OrganizationOwnershipTransfer> OrganizationOwnershipTransfers => Set<OrganizationOwnershipTransfer>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<DepartmentMember> DepartmentMembers => Set<DepartmentMember>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<BranchMember> BranchMembers => Set<BranchMember>();
    public DbSet<BrandKit> BrandKits => Set<BrandKit>();
    public DbSet<BrandAsset> BrandAssets => Set<BrandAsset>();
    public DbSet<BrandTheme> BrandThemes => Set<BrandTheme>();
    public DbSet<StorageFile> StorageFiles => Set<StorageFile>(); public DbSet<StorageFolder> StorageFolders => Set<StorageFolder>();
    public DbSet<StorageCategory> StorageCategories => Set<StorageCategory>(); public DbSet<StorageUsage> StorageUsages => Set<StorageUsage>();
    public DbSet<FileTag> FileTags => Set<FileTag>(); public DbSet<FolderPermission> FolderPermissions => Set<FolderPermission>();
    public DbSet<EntitlementPlan> EntitlementPlans => Set<EntitlementPlan>(); public DbSet<FeatureDefinition> FeatureDefinitions => Set<FeatureDefinition>();
    public DbSet<PlanFeature> EntitlementPlanFeatures => Set<PlanFeature>(); public DbSet<OrganizationSubscription> OrganizationSubscriptions => Set<OrganizationSubscription>();
    public DbSet<UsageRecord> EntitlementUsageRecords => Set<UsageRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventLensDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;
                if (entry.Property(x => x.CreatedBy).CurrentValue is null)
                    entry.Property(x => x.CreatedBy).CurrentValue = currentUser?.UserId;
            }
            if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.UpdatedAt).CurrentValue = DateTime.UtcNow;
                entry.Property(x => x.UpdatedBy).CurrentValue = currentUser?.UserId;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
