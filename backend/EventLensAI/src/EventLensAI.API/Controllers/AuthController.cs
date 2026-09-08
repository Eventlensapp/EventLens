using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Auth;
using EventLensAI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EventLensAI.API.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("authentication")]
public sealed class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    private const string RefreshCookie = "eventlensai_refresh";

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(
        RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);
        SetRefreshCookie(result.RefreshToken);
        logger.LogInformation("User registered {UserId}", result.User.Id);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AuthResponse>.Ok(result, "Registration successful."));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(
        LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        SetRefreshCookie(result.RefreshToken);
        logger.LogInformation("User authenticated {UserId}", result.User.Id);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "Login successful."));
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Refresh(
        RefreshTokenRequest? request, CancellationToken cancellationToken)
    {
        var rawToken = ResolveRefreshToken(request);
        if (string.IsNullOrWhiteSpace(rawToken)) return Unauthorized(ApiResponse<object>.Fail("Refresh token is required."));
        var result = await authService.RefreshAsync(rawToken, cancellationToken);
        SetRefreshCookie(result.RefreshToken);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "Token refreshed."));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<object>>> Logout(
        RefreshTokenRequest? request, CancellationToken cancellationToken)
    {
        var rawToken = ResolveRefreshToken(request);
        if (!string.IsNullOrWhiteSpace(rawToken)) await authService.LogoutAsync(rawToken, cancellationToken);
        Response.Cookies.Delete(RefreshCookie, RefreshCookieOptions());
        logger.LogInformation("User logged out {UserId}", User.FindFirst("sub")?.Value);
        return Ok(ApiResponse<object>.Ok(new { }, "Logout successful."));
    }

    private string? ResolveRefreshToken(RefreshTokenRequest? request)
    {
        if (!string.IsNullOrWhiteSpace(request?.RefreshToken)) return request.RefreshToken;
        return Request.Cookies.TryGetValue(RefreshCookie, out var value) ? value : null;
    }
    private void SetRefreshCookie(string token) =>
        Response.Cookies.Append(RefreshCookie, token, RefreshCookieOptions());
    private static CookieOptions RefreshCookieOptions() => new()
    {
        HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict,
        Expires = DateTimeOffset.UtcNow.AddDays(30), Path = "/api/auth"
    };
}
