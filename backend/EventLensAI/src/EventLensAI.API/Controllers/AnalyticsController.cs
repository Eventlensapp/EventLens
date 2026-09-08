using EventLensAI.Application.Common;
using EventLensAI.Application.Features.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EventLensAI.API.Controllers;
[ApiController,Authorize(Policy="ViewAnalytics"),Route("api/analytics"),Produces("application/json")]
public sealed class AnalyticsController(IAnalyticsService service):ControllerBase
{
 /// <summary>Returns tenant-scoped headline metrics for the selected filters.</summary>
 [HttpGet("dashboard")]public async Task<ActionResult<ApiResponse<AnalyticsDashboardDto>>> Dashboard([FromQuery]AnalyticsFilter filter,CancellationToken ct)=>Ok(ApiResponse<AnalyticsDashboardDto>.Ok(await service.DashboardAsync(filter,ct)));
 /// <summary>Returns overview, hourly activity, timeline, booth and template analytics for one event.</summary>
 [HttpGet("events/{eventId:guid}")]public async Task<ActionResult<ApiResponse<EventAnalyticsDto>>> Event(Guid eventId,[FromQuery]DateTime? from,[FromQuery]DateTime? to,CancellationToken ct)=>Ok(ApiResponse<EventAnalyticsDto>.Ok(await service.EventAsync(eventId,from,to,ct)));
 [HttpGet("charts")]public async Task<ActionResult<ApiResponse<IReadOnlyList<AnalyticsChartDto>>>> Charts([FromQuery]AnalyticsFilter filter,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<AnalyticsChartDto>>.Ok(await service.ChartsAsync(filter,ct)));
 [HttpGet("activity")]public async Task<ActionResult<ApiResponse<PagedResult<ActivityFeedItem>>>> Activity([FromQuery]AnalyticsFilter filter,[FromQuery]int page=1,[FromQuery]int pageSize=20,CancellationToken ct=default)=>Ok(ApiResponse<PagedResult<ActivityFeedItem>>.Ok(await service.ActivityAsync(filter,page,pageSize,ct)));
 /// <summary>Records authorized analytics telemetry and publishes a live SignalR update.</summary>
 [HttpPost("statistics")]public async Task<ActionResult<ApiResponse<object>>> Track(TrackAnalyticsRequest request,CancellationToken ct){await service.TrackAsync(request,ct);return Accepted(ApiResponse<object>.Ok(new{},"Metric recorded."));}
 /// <summary>Exports a tenant-scoped report as CSV, Excel XML workbook, or PDF.</summary>
 [HttpPost("reports/export")]public async Task<IActionResult> Export(ReportRequest request,CancellationToken ct){var file=await service.ExportAsync(request,ct);return File(file.Content,file.ContentType,file.FileName);}
}
