using EventLensAI.Application.Common;
using EventLensAI.Domain.Enums;
namespace EventLensAI.Application.Features.Analytics;
public sealed record AnalyticsFilter(Guid OrganizationId,Guid? EventId=null,DateTime? From=null,DateTime? To=null,string? EventType=null,Guid? TemplateId=null,Guid? PhotographerId=null);
public sealed record AnalyticsDashboardDto(int TotalEvents,int ActiveEvents,int TotalGuests,int TotalBoothSessions,int PhotosCaptured,int AIPhotosGenerated,int GalleryViews,int GalleryVisitors,int Downloads,int Shares,int QrScans,int LeadsCollected,decimal? Revenue,long StorageUsed);
public sealed record ChartPoint(string Label,double Value,double? SecondaryValue)
{
 public ChartPoint(string label,double value):this(label,value,null){}
}
public sealed record AnalyticsChartDto(string Type,string Title,IReadOnlyList<ChartPoint> Points);
public sealed record EventAnalyticsDto(Guid EventId,string EventName,AnalyticsDashboardDto Overview,double AverageSessionMinutes,double AveragePhotosPerGuest,IReadOnlyList<ChartPoint> HourlyActivity,IReadOnlyList<ChartPoint> Timeline,IReadOnlyList<ChartPoint> TemplatePopularity);
public sealed record ActivityFeedItem(Guid Id,AnalyticsMetric Metric,Guid? EventId,Guid? GuestId,Guid? PhotoId,DateTime OccurredAt,int Value);
public sealed record TrackAnalyticsRequest(Guid OrganizationId,AnalyticsMetric Metric,Guid? EventId=null,Guid? GuestId=null,Guid? PhotoId=null,Guid? TemplateId=null,Guid? PhotographerId=null,string? SessionKey=null,AnalyticsDeviceType DeviceType=AnalyticsDeviceType.Unknown,string? Browser=null,string? OperatingSystem=null,string? Country=null,string? City=null,string? DimensionsJson=null,int Value=1);
public sealed record ReportRequest(Guid OrganizationId,Guid? EventId,DateTime From,DateTime To,AnalyticsReportPeriod Period,AnalyticsExportFormat Format);
public sealed record ExportedReport(string FileName,string ContentType,byte[] Content);
public interface IAnalyticsService
{
 Task TrackAsync(TrackAnalyticsRequest request,CancellationToken ct);
 Task<AnalyticsDashboardDto> DashboardAsync(AnalyticsFilter filter,CancellationToken ct);
 Task<EventAnalyticsDto> EventAsync(Guid eventId,DateTime? from,DateTime? to,CancellationToken ct);
 Task<IReadOnlyList<AnalyticsChartDto>> ChartsAsync(AnalyticsFilter filter,CancellationToken ct);
 Task<PagedResult<ActivityFeedItem>> ActivityAsync(AnalyticsFilter filter,int page,int pageSize,CancellationToken ct);
 Task<ExportedReport> ExportAsync(ReportRequest request,CancellationToken ct);
}
public interface IAnalyticsNotifier{Task MetricRecordedAsync(Guid organizationId,Guid? eventId,AnalyticsMetric metric,int value,CancellationToken ct);}
