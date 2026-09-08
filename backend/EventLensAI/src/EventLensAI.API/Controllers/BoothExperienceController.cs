using EventLensAI.Application.Common;using EventLensAI.Application.Features.Booth;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.AspNetCore.RateLimiting;
namespace EventLensAI.API.Controllers;
[ApiController,Route("api/booth/experience"),Produces("application/json"),EnableRateLimiting("public-event")]
public sealed class BoothExperienceController(IBoothExperienceService experience,IGuestJourneyService journey,IBoothRecoveryService recovery):ControllerBase
{
 [AllowAnonymous,HttpGet("configuration/{eventId:guid}")]public async Task<ActionResult<ApiResponse<BoothExperienceDto>>>Configuration(Guid eventId,CancellationToken ct)=>Ok(ApiResponse<BoothExperienceDto>.Ok(await experience.GetAsync(eventId,ct)));
 [Authorize(Policy="StartBooth"),HttpPut("configuration/{eventId:guid}")]public async Task<ActionResult<ApiResponse<GuestBoothConfigurationDto>>>Update(Guid eventId,SaveBoothExperienceConfigurationRequest request,CancellationToken ct)=>Ok(ApiResponse<GuestBoothConfigurationDto>.Ok(await experience.UpdateAsync(eventId,request,ct),"Booth experience updated."));
 [AllowAnonymous,HttpPost("start")]public async Task<ActionResult<ApiResponse<GuestJourneyStateDto>>>Start(StartGuestJourneyRequest request,CancellationToken ct)=>Ok(ApiResponse<GuestJourneyStateDto>.Ok(await journey.StartAsync(request,ct)));
 [AllowAnonymous,HttpPost("transition")]public async Task<ActionResult<ApiResponse<GuestJourneyStateDto>>>Transition(TransitionGuestJourneyRequest request,CancellationToken ct)=>Ok(ApiResponse<GuestJourneyStateDto>.Ok(await journey.TransitionAsync(request,ct)));
 [AllowAnonymous,HttpGet("state/{sessionId:guid}")]public async Task<ActionResult<ApiResponse<GuestJourneyStateDto>>>State(Guid sessionId,[FromQuery]string token,CancellationToken ct)=>Ok(ApiResponse<GuestJourneyStateDto>.Ok(await journey.StateAsync(sessionId,token,ct)));
 [AllowAnonymous,HttpPost("recover")]public async Task<ActionResult<ApiResponse<GuestJourneyStateDto?>>>Recover(RecoverGuestJourneyRequest request,CancellationToken ct)=>Ok(ApiResponse<GuestJourneyStateDto?>.Ok(await recovery.RecoverAsync(request,ct)));
}
