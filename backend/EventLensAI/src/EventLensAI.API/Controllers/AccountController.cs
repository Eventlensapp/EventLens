using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Auth;
using EventLensAI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EventLensAI.API.Controllers;

[ApiController,Route("api/account")]
public sealed class AccountController(IAccountService accounts,IConfiguration configuration):ControllerBase
{
    string AppUrl=>configuration["WebAppBaseUrl"]??"http://localhost:3000";
    [AllowAnonymous,EnableRateLimiting("authentication"),HttpPost("verification/request")]
    public async Task<ActionResult<ApiResponse<object>>>RequestVerification(EmailRequest request,CancellationToken ct){await accounts.RequestVerificationAsync(request.Email,AppUrl,ct);return Ok(ApiResponse<object>.Ok(new{},"If the account exists, verification instructions have been sent."));}
    [AllowAnonymous,EnableRateLimiting("authentication"),HttpPost("verification/confirm")]
    public async Task<ActionResult<ApiResponse<object>>>Verify(TokenRequest request,CancellationToken ct){await accounts.VerifyEmailAsync(request.Token,ct);return Ok(ApiResponse<object>.Ok(new{},"Email verified."));}
    [AllowAnonymous,EnableRateLimiting("authentication"),HttpPost("password-reset/request")]
    public async Task<ActionResult<ApiResponse<object>>>RequestReset(EmailRequest request,CancellationToken ct){await accounts.RequestPasswordResetAsync(request.Email,AppUrl,ct);return Ok(ApiResponse<object>.Ok(new{},"If the account exists, password reset instructions have been sent."));}
    [AllowAnonymous,EnableRateLimiting("authentication"),HttpPost("password-reset/confirm")]
    public async Task<ActionResult<ApiResponse<object>>>Reset(ResetPasswordRequest request,CancellationToken ct){await accounts.ResetPasswordAsync(request,ct);return Ok(ApiResponse<object>.Ok(new{},"Password reset. Sign in again."));}
    [Authorize,HttpGet("profile")]public async Task<ActionResult<ApiResponse<ProfileDto>>>Profile(CancellationToken ct)=>Ok(ApiResponse<ProfileDto>.Ok(await accounts.GetProfileAsync(ct)));
    [Authorize,HttpPut("profile")]public async Task<ActionResult<ApiResponse<ProfileDto>>>Profile(UpdateProfileRequest request,CancellationToken ct)=>Ok(ApiResponse<ProfileDto>.Ok(await accounts.UpdateProfileAsync(request,ct),"Profile updated."));
    [Authorize,HttpGet("preferences")]public async Task<ActionResult<ApiResponse<PreferencesDto>>>Preferences(CancellationToken ct)=>Ok(ApiResponse<PreferencesDto>.Ok(await accounts.GetPreferencesAsync(ct)));
    [Authorize,HttpPut("preferences")]public async Task<ActionResult<ApiResponse<PreferencesDto>>>Preferences(UpdatePreferencesRequest request,CancellationToken ct)=>Ok(ApiResponse<PreferencesDto>.Ok(await accounts.UpdatePreferencesAsync(request,ct),"Preferences updated."));
    [Authorize,HttpGet("sessions")]public async Task<ActionResult<ApiResponse<IReadOnlyList<SessionDto>>>>Sessions(CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<SessionDto>>.Ok(await accounts.ListSessionsAsync(null,ct)));
    [Authorize,HttpDelete("sessions/{id:guid}")]public async Task<ActionResult<ApiResponse<object>>>RevokeSession(Guid id,CancellationToken ct){await accounts.RevokeSessionAsync(id,ct);return Ok(ApiResponse<object>.Ok(new{},"Session revoked."));}
    [Authorize,HttpDelete("sessions")]public async Task<ActionResult<ApiResponse<object>>>RevokeSessions(CancellationToken ct){await accounts.RevokeOtherSessionsAsync(null,ct);return Ok(ApiResponse<object>.Ok(new{},"All sessions revoked."));}
    [Authorize,HttpGet("activity")]public async Task<ActionResult<ApiResponse<IReadOnlyList<ActivityDto>>>>Activity(CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<ActivityDto>>.Ok(await accounts.ListActivityAsync(ct)));
    [Authorize,HttpGet("api-keys")]public async Task<ActionResult<ApiResponse<IReadOnlyList<ApiKeyDto>>>>ApiKeys(CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<ApiKeyDto>>.Ok(await accounts.ListApiKeysAsync(ct)));
    [Authorize,HttpPost("api-keys")]public async Task<ActionResult<ApiResponse<ApiKeyDto>>>CreateApiKey(CreateApiKeyRequest request,CancellationToken ct)=>StatusCode(201,ApiResponse<ApiKeyDto>.Ok(await accounts.CreateApiKeyAsync(request,ct),"Copy this key now; it will not be shown again."));
    [Authorize,HttpDelete("api-keys/{id:guid}")]public async Task<ActionResult<ApiResponse<object>>>RevokeApiKey(Guid id,CancellationToken ct){await accounts.RevokeApiKeyAsync(id,ct);return Ok(ApiResponse<object>.Ok(new{},"API key revoked."));}
}
