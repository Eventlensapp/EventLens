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
public sealed record EmailRequest(string Email);
public sealed record TokenRequest(string Token);
public sealed record ResetPasswordRequest(string Token,string Password,string ConfirmPassword);
public sealed record UpdateProfileRequest(string FirstName,string LastName,string? Phone,string? ProfileImage,string TimeZone,string Language);
public sealed record ProfileDto(Guid Id,string FirstName,string LastName,string Email,string? Phone,string? ProfileImage,string TimeZone,string Language,bool EmailVerified);
public sealed record UpdatePreferencesRequest(string Theme,string Language,bool EmailNotifications,bool SecurityNotifications);
public sealed record PreferencesDto(string Theme,string Language,bool EmailNotifications,bool SecurityNotifications);
public sealed record SessionDto(Guid Id,string? Device,string? IpAddress,DateTime CreatedAt,DateTime LastActivityAt,DateTime ExpiresAt,bool Current);
public sealed record ActivityDto(Guid Id,string Action,string Description,string? IpAddress,string? Device,DateTime CreatedAt);
public sealed record CreateApiKeyRequest(string Name,DateTime? ExpiresAt);
public sealed record ApiKeyDto(Guid Id,string Name,string Prefix,DateTime? ExpiresAt,DateTime? LastUsedAt,bool IsActive,string? Key=null);
