namespace EventLensAI.Application.DTOs.Auth;

public sealed record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? Phone,
    bool AcceptTerms);

public sealed record LoginRequest(string Email, string Password);
public sealed record RefreshTokenRequest(string? RefreshToken);
public sealed record UserDto(Guid Id, string FirstName, string LastName, string Email, IReadOnlyCollection<string> Roles);
public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User);
