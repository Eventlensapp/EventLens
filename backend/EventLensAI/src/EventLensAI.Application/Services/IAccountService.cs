using EventLensAI.Application.DTOs.Auth;

namespace EventLensAI.Application.Services;

public interface IAccountService
{
    Task RequestVerificationAsync(string email,string baseUrl,CancellationToken ct);
    Task VerifyEmailAsync(string token,CancellationToken ct);
    Task RequestPasswordResetAsync(string email,string baseUrl,CancellationToken ct);
    Task ResetPasswordAsync(ResetPasswordRequest request,CancellationToken ct);
    Task<ProfileDto> GetProfileAsync(CancellationToken ct);
    Task<ProfileDto> UpdateProfileAsync(UpdateProfileRequest request,CancellationToken ct);
    Task<PreferencesDto> GetPreferencesAsync(CancellationToken ct);
    Task<PreferencesDto> UpdatePreferencesAsync(UpdatePreferencesRequest request,CancellationToken ct);
    Task<IReadOnlyList<SessionDto>> ListSessionsAsync(Guid? currentSession,CancellationToken ct);
    Task RevokeSessionAsync(Guid id,CancellationToken ct);
    Task RevokeOtherSessionsAsync(Guid? currentSession,CancellationToken ct);
    Task<IReadOnlyList<ActivityDto>> ListActivityAsync(CancellationToken ct);
    Task<IReadOnlyList<ApiKeyDto>> ListApiKeysAsync(CancellationToken ct);
    Task<ApiKeyDto> CreateApiKeyAsync(CreateApiKeyRequest request,CancellationToken ct);
    Task RevokeApiKeyAsync(Guid id,CancellationToken ct);
}
