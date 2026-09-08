using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoBooth.Application.Modules.Authentication;

namespace PhotoBooth.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(IAuthenticationService authenticationService) : ControllerBase
{
    private const string RefreshCookie = "eventlens_refresh";

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<AuthenticationResponse>> Register(
        RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await authenticationService.RegisterAsync(
            new RegisterCommand(request.Email, request.Password, request.DisplayName), cancellationToken);
        if (!result.Succeeded) return ToProblem(result.ErrorCode!);
        SetRefreshCookie(result.RefreshToken!);
        return StatusCode(StatusCodes.Status201Created, ToResponse(result));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthenticationResponse>> Login(
        LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Problem(statusCode: 400, title: "Email and password are required.");
        var result = await authenticationService.LoginAsync(
            new LoginCommand(request.Email, request.Password), cancellationToken);
        if (!result.Succeeded) return ToProblem(result.ErrorCode!);
        SetRefreshCookie(result.RefreshToken!);
        return Ok(ToResponse(result));
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthenticationResponse>> Refresh(
        RefreshRequest? request, CancellationToken cancellationToken)
    {
        var rawToken = request?.RefreshToken;
        if (string.IsNullOrWhiteSpace(rawToken))
            Request.Cookies.TryGetValue(RefreshCookie, out rawToken);
        if (string.IsNullOrWhiteSpace(rawToken))
            return Problem(statusCode: 401, title: "Refresh token is required.");

        var result = await authenticationService.RefreshAsync(
            new RefreshCommand(rawToken), cancellationToken);
        if (!result.Succeeded)
        {
            DeleteRefreshCookie();
            return ToProblem(result.ErrorCode!);
        }
        SetRefreshCookie(result.RefreshToken!);
        return Ok(ToResponse(result));
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(
        RefreshRequest? request, CancellationToken cancellationToken)
    {
        var rawToken = request?.RefreshToken;
        if (string.IsNullOrWhiteSpace(rawToken))
            Request.Cookies.TryGetValue(RefreshCookie, out rawToken);
        if (!string.IsNullOrWhiteSpace(rawToken))
            await authenticationService.RevokeAsync(rawToken, cancellationToken);
        DeleteRefreshCookie();
        return NoContent();
    }

    [HttpGet("me")]
    public ActionResult<object> Me() => Ok(new
    {
        id = User.FindFirstValue(ClaimTypes.NameIdentifier),
        email = User.FindFirstValue(ClaimTypes.Email),
        name = User.FindFirstValue(ClaimTypes.Name),
        role = User.FindFirstValue(ClaimTypes.Role)
    });

    private void SetRefreshCookie(string refreshToken) =>
        Response.Cookies.Append(RefreshCookie, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            Path = "/api/v1/auth"
        });

    private void DeleteRefreshCookie() =>
        Response.Cookies.Delete(RefreshCookie, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/v1/auth"
        });

    private ObjectResult ToProblem(string errorCode) => errorCode switch
    {
        "email_already_registered" => Problem(
            statusCode: 409, title: "Email is already registered.", type: "urn:eventlens:auth:email-exists"),
        "weak_password" => Problem(
            statusCode: 400, title: "Password does not meet the security policy.",
            detail: "Use at least 12 characters with upper-case, lower-case, and numeric characters."),
        "invalid_email" or "invalid_display_name" => Problem(
            statusCode: 400, title: "Registration details are invalid.", type: $"urn:eventlens:auth:{errorCode}"),
        _ => Problem(
            statusCode: 401, title: "Authentication failed.", type: $"urn:eventlens:auth:{errorCode}")
    };

    private static AuthenticationResponse ToResponse(AuthenticationResult result) =>
        new(result.AccessToken!, result.RefreshToken!, result.AccessTokenExpiresAtUtc!.Value, result.User!);

    public sealed record RegisterRequest(string Email, string Password, string DisplayName);
    public sealed record LoginRequest(string Email, string Password);
    public sealed record RefreshRequest(string? RefreshToken);
    public sealed record AuthenticationResponse(
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpiresAtUtc,
        UserResponse User);
}
