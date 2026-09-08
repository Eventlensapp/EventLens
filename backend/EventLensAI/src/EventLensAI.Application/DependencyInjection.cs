using EventLensAI.Application.Mappings;
using EventLensAI.Application.Services;
using FluentValidation;
using EventLensAI.Application.Features.Photos.PhotoProcessing;
using EventLensAI.Application.Features.Photos.Templates;
using EventLensAI.Application.Features.AI;
using Microsoft.Extensions.DependencyInjection;
using EventLensAI.Application.Features.Booth;

namespace EventLensAI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddAutoMapper(_ => { }, typeof(MappingProfile).Assembly);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IOrganizationSecurityService, OrganizationSecurityService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IBranchService, BranchService>();
        services.AddScoped<IBrandKitService, BrandKitService>();
        services.AddScoped<IBrandAssetService, BrandAssetService>();
        services.AddScoped<IBrandThemeService, BrandThemeService>();
        services.AddScoped<IFileService, FileService>(); services.AddScoped<IFolderService, FolderService>(); services.AddScoped<IStorageQuotaService, StorageQuotaService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IEventTypeService, EventTypeService>();
        services.AddScoped<IEventMemberService, EventMemberService>();
        services.AddScoped<IEventTemplateService, EventTemplateService>();
        services.AddScoped<IEventBrandService,EventBrandService>();services.AddScoped<IEventExperienceService,EventExperienceService>();
        services.AddScoped<IEventAssetService,EventAssetService>();services.AddScoped<IEventSponsorService,EventSponsorService>();
        services.AddScoped<IVenueService,VenueService>();services.AddScoped<IScheduleService,ScheduleService>();
        services.AddScoped<IEventChecklistService,EventChecklistService>();
        services.AddScoped<IEventReadinessService,EventReadinessService>();
        services.AddScoped<IEventStaffService,EventStaffService>();
        services.AddScoped<IBoothPlacementService,BoothPlacementService>();
        services.AddScoped<INotificationService,NotificationService>();
        services.AddScoped<IEventQRCodeService,EventQRCodeService>();services.AddScoped<IEventAccessSettingsService,EventAccessSettingsService>();
        services.AddScoped<IPublicEventService,PublicEventService>();services.AddScoped<IGuestSessionService,GuestSessionService>();services.AddScoped<IEventAccessAnalyticsService,EventAccessAnalyticsService>();
        services.AddScoped<IBoothSessionService, BoothSessionService>();
        services.AddScoped<IBoothConfigurationService,BoothConfigurationService>();
        services.AddScoped<IBrowserCapabilityService,BrowserCapabilityService>();
        services.AddScoped<IHardwareDetectionService,HardwareDetectionService>();
        services.AddScoped<IPermissionService,PermissionService>();
        services.AddSingleton<IBoothStateService,BoothStateService>();
        services.AddScoped<IBoothDiagnosticService,BoothDiagnosticService>();
        services.AddScoped<IOfflineFoundationService,OfflineFoundationService>();
        services.AddSingleton<IBoothRuntimeService,BoothRuntimeService>();
        services.AddScoped<ISessionRecoveryService,SessionRecoveryService>();
        services.AddScoped<ISessionActivityService,SessionActivityService>();
        services.AddSingleton<ICameraManager,CameraManager>();
        services.AddScoped<ICameraDiscoveryService,CameraDiscoveryService>();
        services.AddScoped<ICameraCapabilityService,CameraCapabilityService>();
        services.AddScoped<ICameraPreferenceService,CameraPreferenceService>();
        services.AddScoped<ICaptureService,CaptureService>();
        services.AddSingleton<ICountdownService,CountdownService>();
        services.AddSingleton<ICaptureQueueService,CaptureQueueService>();
        services.AddSingleton<ILocalCaptureStorageService,LocalCaptureStorageService>();
        services.AddScoped<ICameraControlService,CameraControlService>();
        services.AddScoped<ICameraProfileService,CameraProfileService>();
        services.AddScoped<ICameraSettingsAuditService,CameraSettingsAuditService>();
        services.AddScoped<IMediaCaptureService,MediaCaptureService>();
        services.AddSingleton<IMediaProcessingQueue,MediaProcessingQueue>();
        services.AddSingleton<IGifCaptureService,GifCaptureService>();services.AddSingleton<IVideoCaptureService,VideoCaptureService>();
        services.AddSingleton<IBoomerangService,BoomerangService>();services.AddSingleton<ITimeLapseService,TimeLapseService>();
        services.AddSingleton<BrowserCameraAdapter>();services.AddSingleton<ICameraAdapterFactory,CameraAdapterFactory>();
        services.AddScoped<IProfessionalCameraService,ProfessionalCameraService>();services.AddScoped<ICameraHealthService,CameraHealthService>();
        services.AddScoped<IPhotoProcessingService, PhotoProcessingService>();
        services.AddScoped<ITemplateService, TemplateService>();
        services.AddScoped<IPhotoTemplateService, PhotoTemplateService>();
        services.AddScoped<ITemplateRendererService, TemplateRendererService>();
        services.AddScoped<IStickerService, StickerService>();
        services.AddScoped<IBoothExperienceService,BoothExperienceService>();
        services.AddScoped<IGuestJourneyService,GuestJourneyService>();
        services.AddScoped<IBoothRecoveryService>(x=>x.GetRequiredService<IGuestJourneyService>() as IBoothRecoveryService??throw new InvalidOperationException());
        services.AddScoped<IAIProcessingService, AIProcessingService>();
        services.AddScoped<IAIAssetService, AIAssetService>();
        return services;
    }
}
