using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using EventLensAI.API.Configurations;
using EventLensAI.API.Filters;
using EventLensAI.API.Services;
using EventLensAI.Application.Interfaces.Identity;
using Microsoft.AspNetCore.RateLimiting;

namespace EventLensAI.API.Extensions;

public static class ServiceCollectionExtensions
{
    public const string CorsPolicy = "EventLensWeb";

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ValidationFilter>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services
            .AddControllers(options => options.Filters.AddService<ValidationFilter>())
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddEndpointsApiExplorer();
        services.Configure<CorsSettings>(configuration.GetSection(CorsSettings.SectionName));
        var origins = configuration.GetSection(CorsSettings.SectionName)
            .Get<CorsSettings>()?.AllowedOrigins ?? [];
        services.AddCors(options => options.AddPolicy(CorsPolicy, policy =>
        {
            if (origins.Length == 0) return;
            policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        }));
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    success = false, message = "Too many requests. Try again later.", data = (object?)null
                }, cancellationToken);
            };
            options.AddFixedWindowLimiter("authentication", limiter =>
            {
                limiter.PermitLimit = 10;
                limiter.Window = TimeSpan.FromMinutes(1);
                limiter.QueueLimit = 0;
                limiter.AutoReplenishment = true;
            });
            options.AddFixedWindowLimiter("public-event",limiter=>{limiter.PermitLimit=30;limiter.Window=TimeSpan.FromMinutes(1);limiter.QueueLimit=0;limiter.AutoReplenishment=true;});
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 120,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));
        });
        return services;
    }
}
