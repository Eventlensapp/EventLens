using System.Text;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Application.Interfaces.Services;
using EventLensAI.Application.Services;
using EventLensAI.Infrastructure.Identity;
using EventLensAI.Infrastructure.Persistence;
using EventLensAI.Infrastructure.Repositories;
using EventLensAI.Infrastructure.Services;
using EventLensAI.Infrastructure.Storage;
using EventLensAI.Infrastructure.ImageProcessing;
using EventLensAI.Application.Features.Photos.PhotoProcessing;
using EventLensAI.Application.Features.Photos.Templates;
using EventLensAI.Application.Features.AI;
using EventLensAI.Infrastructure.AI;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;
using EventLensAI.Application.Features.CRM;
using EventLensAI.Application.Features.Analytics;
using EventLensAI.Application.Features.Billing;
using EventLensAI.Application.Features.Entitlements;
using EventLensAI.Application.Features.Booth;
using EventLensAI.Infrastructure.Billing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EventLensAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("SQLServer")
            ?? throw new InvalidOperationException("SQL Server connection string is missing.");
        services.AddDbContext<EventLensDbContext>(options =>
            options.UseSqlServer(connection, sqlServer =>
            {
                sqlServer.MigrationsAssembly(typeof(EventLensDbContext).Assembly.FullName);
                sqlServer.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }));
        services.AddScoped<DevelopmentSuperAdminSeeder>();
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<EventLensDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IOrganizationSecurityRepository, OrganizationSecurityRepository>();
        services.AddScoped<IOrganizationalStructureRepository, OrganizationalStructureRepository>();
        services.AddScoped<IBrandingRepository, BrandingRepository>();
        services.AddScoped<IStorageRepository, StorageRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IBoothFoundationRepository,BoothFoundationRepository>();
        services.AddScoped<IBoothSessionEngineRepository,BoothSessionEngineRepository>();
        services.AddScoped<IGuestExperienceRepository,GuestExperienceRepository>();
        services.AddScoped<ICaptureEngineRepository,CaptureEngineRepository>();
        services.AddScoped<ICameraControlRepository,CameraControlRepository>();
        services.AddScoped<IMediaCaptureRepository,AdvancedMediaRepository>();
        services.AddScoped<IProfessionalCameraRepository,ProfessionalCameraRepository>();
        services.AddHostedService<SessionExpiryWorker>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        services.AddScoped<ITemplateRepository, TemplateRepository>();
        services.AddScoped<IPhotoTemplateRepository, PhotoTemplateRepository>();
        services.AddScoped<IAIJobRepository, AIJobRepository>();
        services.AddScoped<IAIPromptRepository, AIPromptRepository>();
        services.AddScoped<IAIBackgroundRepository, AIBackgroundRepository>();
        services.AddSingleton<IAIJobQueue, AIJobQueue>();
        services.AddScoped<IAIProviderResolver, AIProviderResolver>();
        services.AddSingleton<IAIProviderSelection, ConfiguredAIProviderSelection>();
        services.AddHostedService<AIJobWorker>();
        services.AddHttpClient("ai-provider");
        foreach (var providerSection in configuration.GetSection("AI:Providers").GetChildren()
            .Where(x => x.GetValue<bool>("Enabled")))
        {
            var name = providerSection.Key;
            var endpoint = providerSection["Endpoint"] ?? throw new InvalidOperationException($"AI provider {name} endpoint is missing.");
            var apiKey = providerSection["ApiKey"];
            var supported = providerSection.GetSection("SupportedJobTypes").Get<string[]>() ?? [];
            var kinds = supported.Select(x => Enum.Parse<AIJobType>(x, true)).ToHashSet();
            services.AddSingleton<IAIProvider>(sp => new HttpAIProvider(
                new AIProviderOptions(name, endpoint, apiKey, kinds, providerSection.GetValue("TimeoutSeconds", 120)),
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("ai-provider")));
        }
        services.AddSingleton<IStorageService, LocalStorageService>();
        services.AddSingleton<IFileScanner, BasicFileScanner>();
        services.AddSingleton<IBrandUploadPolicy>(new BrandUploadPolicy(configuration.GetValue<long?>("Branding:MaxUploadBytes") ?? 10 * 1024 * 1024));
        services.AddSingleton<IImageProcessingService, ImageSharpProcessingService>();
        services.AddSingleton<IPhotoComposerService, ImageSharpPhotoComposerService>();
        services.AddSingleton<IQrCodeService, QrCodeService>();
        services.AddSingleton<IPublicUrlService, PublicUrlService>();
        services.AddScoped<ICrmService, CrmService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddMemoryCache();
        services.AddHostedService<AnalyticsAggregationWorker>();
        services.AddScoped<IFeaturePermissionService, FeaturePermissionService>();
        services.AddScoped<IBillingService, BillingService>();
        services.AddScoped<EntitlementService>();
        services.AddScoped<ISubscriptionService>(x=>x.GetRequiredService<EntitlementService>());
        services.AddScoped<IEntitlementService>(x=>x.GetRequiredService<EntitlementService>());
        services.AddScoped<IUsageTrackingService>(x=>x.GetRequiredService<EntitlementService>());
        services.AddScoped<IPaymentProviderResolver, PaymentProviderResolver>();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<IEmailService, DevelopmentEmailService>();

        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT settings are missing.");
        if (jwt.Key.Length < 32) throw new InvalidOperationException("JWT key must be at least 32 characters.");
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true, ValidIssuer = jwt.Issuer,
                ValidateAudience = true, ValidAudience = jwt.Audience,
                ValidateLifetime = true, ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                ClockSkew = TimeSpan.FromSeconds(30)
            };
            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        success = false, message = "Authentication is required.", data = (object?)null
                    });
                },
                OnForbidden = async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        success = false, message = "You do not have permission to access this resource.", data = (object?)null
                    });
                }
            };
        });
        services.AddAuthorization(options =>
        {
            options.AddPolicy("OrganizationAdmin", policy =>
                policy.RequireRole(SystemRoles.SuperAdmin, SystemRoles.PlatformAdmin, SystemRoles.Owner, SystemRoles.Manager));
            options.AddPolicy("ManageEvents", policy =>
                policy.RequireRole(SystemRoles.SuperAdmin, SystemRoles.PlatformAdmin, SystemRoles.Owner, SystemRoles.Manager));
            options.AddPolicy("StartBooth", policy =>
                policy.RequireRole(SystemRoles.SuperAdmin, SystemRoles.Owner, SystemRoles.Manager, SystemRoles.Photographer, SystemRoles.BoothOperator));
            options.AddPolicy("ManageMembers", policy =>
                policy.RequireRole(SystemRoles.SuperAdmin, SystemRoles.PlatformAdmin, SystemRoles.Owner, SystemRoles.Manager));
            options.AddPolicy("EditTemplates", policy =>
                policy.RequireRole(SystemRoles.SuperAdmin, SystemRoles.Owner, SystemRoles.Manager, SystemRoles.Editor, SystemRoles.Designer));
            options.AddPolicy("OrganizationRead", policy =>
                policy.RequireRole(SystemRoles.SuperAdmin, SystemRoles.PlatformAdmin, SystemRoles.Owner, SystemRoles.Manager,
                    SystemRoles.Photographer, SystemRoles.BoothOperator, SystemRoles.Editor, SystemRoles.Designer,
                    SystemRoles.MarketingManager, SystemRoles.Viewer));
            options.AddPolicy("ManageCrm", policy =>
                policy.RequireRole(SystemRoles.SuperAdmin, SystemRoles.PlatformAdmin, SystemRoles.Owner, SystemRoles.Manager, SystemRoles.MarketingManager));
            options.AddPolicy("ViewAnalytics", policy =>
                policy.RequireRole(SystemRoles.SuperAdmin, SystemRoles.PlatformAdmin, SystemRoles.Owner, SystemRoles.Manager,
                    SystemRoles.Photographer, SystemRoles.MarketingManager, SystemRoles.Viewer));
            options.AddPolicy("BillingOwner", policy =>
                policy.RequireRole(SystemRoles.SuperAdmin, SystemRoles.Owner));
            options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole(SystemRoles.SuperAdmin));
        });
        return services;
    }
}
