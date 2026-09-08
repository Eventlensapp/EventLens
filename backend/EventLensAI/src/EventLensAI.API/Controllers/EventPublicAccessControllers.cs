using EventLensAI.Application.Common;using EventLensAI.Application.DTOs.Events;using EventLensAI.Application.Services;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.AspNetCore.RateLimiting;
namespace EventLensAI.API.Controllers;
[ApiController,Authorize,Route("api/events/{eventId:guid}/qr")]public sealed class EventQRCodesController(IEventQRCodeService service):ControllerBase
{
 [HttpGet]public async Task<ActionResult<ApiResponse<IReadOnlyList<QRCodeDto>>>>List(Guid eventId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<QRCodeDto>>.Ok(await service.ListAsync(eventId,ct)));
 [HttpPost]public async Task<ActionResult<ApiResponse<QRCodeDto>>>Create(Guid eventId,CreateQRCodeRequest r,CancellationToken ct)=>Ok(ApiResponse<QRCodeDto>.Ok(await service.CreateAsync(eventId,r,ct),"QR code generated."));
 [HttpPut("{id:guid}")]public async Task<ActionResult<ApiResponse<QRCodeDto>>>Update(Guid eventId,Guid id,UpdateQRCodeRequest r,CancellationToken ct)=>Ok(ApiResponse<QRCodeDto>.Ok(await service.UpdateAsync(eventId,id,r,ct),"QR code updated."));
 [HttpDelete("{id:guid}")]public async Task<ActionResult<ApiResponse<object>>>Delete(Guid eventId,Guid id,CancellationToken ct){await service.ArchiveAsync(eventId,id,ct);return Ok(ApiResponse<object>.Ok(new{},"QR code archived."));}
}
[ApiController,Authorize,Route("api/events/{eventId:guid}/access")]public sealed class EventAccessController(IEventAccessSettingsService service):ControllerBase
{
 [HttpGet]public async Task<ActionResult<ApiResponse<EventAccessSettingDto>>>Get(Guid eventId,CancellationToken ct)=>Ok(ApiResponse<EventAccessSettingDto>.Ok(await service.GetAsync(eventId,ct)));
 [HttpPut]public async Task<ActionResult<ApiResponse<EventAccessSettingDto>>>Put(Guid eventId,UpdateEventAccessSettingRequest r,CancellationToken ct)=>Ok(ApiResponse<EventAccessSettingDto>.Ok(await service.UpdateAsync(eventId,r,ct),"Public access settings updated."));
}
[ApiController,Authorize,Route("api/events/{eventId:guid}/access-analytics")]public sealed class EventAccessAnalyticsController(IEventAccessAnalyticsService service):ControllerBase
{[HttpGet]public async Task<ActionResult<ApiResponse<AccessAnalyticsDto>>>Get(Guid eventId,CancellationToken ct)=>Ok(ApiResponse<AccessAnalyticsDto>.Ok(await service.GetAsync(eventId,ct)));}
[ApiController,AllowAnonymous,EnableRateLimiting("public-event"),Route("api/public/events/{token}")]public sealed class PublicEventsController(IPublicEventService events,IGuestSessionService sessions):ControllerBase
{
 [HttpGet]public async Task<ActionResult<ApiResponse<PublicEventDto>>>Get(string token,[FromHeader(Name="X-Event-Password")]string?password,CancellationToken ct)=>Ok(ApiResponse<PublicEventDto>.Ok(await events.GetAsync(token,password,Request.Headers.UserAgent.ToString(),ct)));
 [HttpPost("session")]public async Task<ActionResult<ApiResponse<GuestSessionDto>>>Session(string token,[FromHeader(Name="X-Event-Password")]string?password,CancellationToken ct)=>Ok(ApiResponse<GuestSessionDto>.Ok(await sessions.CreateAsync(token,password,HttpContext.Connection.RemoteIpAddress?.ToString(),Request.Headers.UserAgent.ToString(),Request.Headers.UserAgent.ToString(),ct),"Guest session started."));
}
