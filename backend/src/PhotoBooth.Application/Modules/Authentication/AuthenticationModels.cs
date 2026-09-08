namespace PhotoBooth.Application.Modules.Authentication;

public sealed record RegisterCommand(string Email, string Password, string DisplayName);
public sealed record LoginCommand(string Email, string Password);
public sealed record RefreshCommand(string RefreshToken);

public sealed record AuthenticationResult(
    bool Succeeded,
    string? ErrorCode,
    string? AccessToken,
    string? RefreshToken,
    DateTime? AccessTokenExpiresAtUtc,
    UserResponse? User)
{
    public static AuthenticationResult Failure(string errorCode) =>
        new(false, errorCode, null, null, null, null);
}

public sealed record UserResponse(Guid Id, string Email, string DisplayName, string Role);

public interface IAuthenticationService
{
    Task<AuthenticationResult> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken);
    Task<AuthenticationResult> LoginAsync(LoginCommand command, CancellationToken cancellationToken);
    Task<AuthenticationResult> RefreshAsync(RefreshCommand command, CancellationToken cancellationToken);
    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken);
}
