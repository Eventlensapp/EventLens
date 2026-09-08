using EventLensAI.API.Extensions;
using EventLensAI.API.Middleware;
using EventLensAI.API.Swagger;
using EventLensAI.Application;
using EventLensAI.Infrastructure;
using Serilog;
using Microsoft.Extensions.FileProviders;
using EventLensAI.API.Hubs;
using EventLensAI.API.Services;
using EventLensAI.Application.Features.AI;
using EventLensAI.Application.Features.Analytics;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, services, logger) => logger
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());
    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration)
        .AddApiServices(builder.Configuration)
        .AddEventLensSwagger();
    builder.Services.AddSignalR();
    builder.Services.AddSingleton<IAIJobNotifier, SignalRAIJobNotifier>();
    builder.Services.AddSingleton<IAnalyticsNotifier, SignalRAnalyticsNotifier>();

    var app = builder.Build();
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseMiddleware<SecurityHeadersMiddleware>();
    app.UseSerilogRequestLogging();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseHttpsRedirection();
    var storagePath = Path.GetFullPath(builder.Configuration["Storage:LocalPath"] ?? "storage");
    Directory.CreateDirectory(storagePath);
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(storagePath),
        RequestPath = "/storage",
        ServeUnknownFileTypes = false
    });
    app.UseCors(EventLensAI.API.Extensions.ServiceCollectionExtensions.CorsPolicy);
    app.UseAuthentication();
    app.UseRateLimiter();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHub<AIJobsHub>("/hubs/ai-jobs");
    app.MapHub<AnalyticsHub>("/hubs/analytics");
    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "EventLens AI API terminated unexpectedly");
}
finally { Log.CloseAndFlush(); }

public partial class Program;
