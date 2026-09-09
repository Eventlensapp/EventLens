using EventLensAI.Application.DTOs.Auth;
using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Application.Interfaces.Services;
using EventLensAI.Domain.Entities;
using FluentValidation;

namespace EventLensAI.Application.Services;

public sealed class AccountService(IUserRepository users,ICurrentUserService current,ITokenService tokens,
    IPasswordService passwords,IEmailService email,IUnitOfWork unit):IAccountService
{
    const string VerifyPurpose="email-verification",ResetPurpose="password-reset";
    Guid UserId=>current.UserId??throw new UnauthorizedException("Authentication is required.");
    public async Task RequestVerificationAsync(string emailAddress,string baseUrl,CancellationToken ct)
    {
        var user=await users.GetByNormalizedEmailAsync(emailAddress.Trim().ToUpperInvariant(),ct);
        if(user is null||user.EmailVerified)return;
        var raw=tokens.CreateOpaqueToken();await users.AddUserTokenAsync(new(user.Id,tokens.HashRefreshToken(raw),VerifyPurpose,DateTime.UtcNow.AddHours(24)),ct);
        await users.AddActivityAsync(new(user.Id,"verification.requested","Email verification requested.",null,null),ct);await unit.SaveChangesAsync(ct);
        await email.SendEmailVerificationAsync(user.Email,$"{baseUrl.TrimEnd('/')}/verify-email?token={Uri.EscapeDataString(raw)}",ct);
    }
    public async Task VerifyEmailAsync(string raw,CancellationToken ct)
    {var token=await users.GetUserTokenAsync(tokens.HashRefreshToken(raw),VerifyPurpose,ct);if(token?.IsValid!=true)throw new ValidationException("Verification token is invalid or expired.");token.Use();token.User.VerifyEmail();await users.AddActivityAsync(new(token.UserId,"email.verified","Email address verified.",null,null),ct);await unit.SaveChangesAsync(ct);}
    public async Task RequestPasswordResetAsync(string emailAddress,string baseUrl,CancellationToken ct)
    {var user=await users.GetByNormalizedEmailAsync(emailAddress.Trim().ToUpperInvariant(),ct);if(user is null)return;var raw=tokens.CreateOpaqueToken();await users.AddUserTokenAsync(new(user.Id,tokens.HashRefreshToken(raw),ResetPurpose,DateTime.UtcNow.AddMinutes(30)),ct);await users.AddActivityAsync(new(user.Id,"password-reset.requested","Password reset requested.",null,null),ct);await unit.SaveChangesAsync(ct);await email.SendPasswordResetAsync(user.Email,$"{baseUrl.TrimEnd('/')}/reset-password?token={Uri.EscapeDataString(raw)}",ct);}
    public async Task ResetPasswordAsync(ResetPasswordRequest request,CancellationToken ct)
    {if(request.Password!=request.ConfirmPassword)throw new ValidationException("Passwords do not match.");var token=await users.GetUserTokenAsync(tokens.HashRefreshToken(request.Token),ResetPurpose,ct);if(token?.IsValid!=true)throw new ValidationException("Reset token is invalid or expired.");token.Use();token.User.ChangePassword(passwords.Hash(request.Password));foreach(var session in await users.ListSessionsAsync(token.UserId,ct))session.Revoke();await users.AddActivityAsync(new(token.UserId,"password.changed","Password changed and active sessions revoked.",null,null),ct);await unit.SaveChangesAsync(ct);}
    public async Task ChangePasswordAsync(ChangePasswordRequest request,CancellationToken ct)
    {var user=await Me(ct);if(!passwords.Verify(user,request.CurrentPassword,user.PasswordHash))throw new ValidationException("Current password is incorrect.");if(request.Password!=request.ConfirmPassword)throw new ValidationException("Passwords do not match.");user.ChangePassword(passwords.Hash(request.Password));foreach(var session in await users.ListSessionsAsync(user.Id,ct))session.Revoke();await users.AddActivityAsync(new(user.Id,"password.changed","Temporary password replaced and active sessions revoked.",current.IPAddress,null),ct);await unit.SaveChangesAsync(ct);}
    async Task<User> Me(CancellationToken ct)=>await users.GetByIdWithRolesAsync(UserId,ct)??throw new NotFoundException("User not found.");
    static ProfileDto Profile(User x)=>new(x.Id,x.FirstName,x.LastName,x.Email,x.Phone,x.ProfileImage,x.TimeZone,x.Language,x.EmailVerified);
    public async Task<ProfileDto> GetProfileAsync(CancellationToken ct)=>Profile(await Me(ct));
    public async Task<ProfileDto> UpdateProfileAsync(UpdateProfileRequest r,CancellationToken ct){var x=await Me(ct);x.UpdateProfile(r.FirstName,r.LastName,r.Phone,r.ProfileImage,r.TimeZone,r.Language);await users.AddActivityAsync(new(x.Id,"profile.updated","Profile details updated.",null,null),ct);await unit.SaveChangesAsync(ct);return Profile(x);}
    static PreferencesDto Pref(UserPreference x)=>new(x.Theme,x.Language,x.EmailNotifications,x.SecurityNotifications);
    public async Task<PreferencesDto> GetPreferencesAsync(CancellationToken ct){var x=await users.GetPreferenceAsync(UserId,ct);if(x is null){x=new(UserId);await users.AddPreferenceAsync(x,ct);await unit.SaveChangesAsync(ct);}return Pref(x);}
    public async Task<PreferencesDto> UpdatePreferencesAsync(UpdatePreferencesRequest r,CancellationToken ct){var x=await users.GetPreferenceAsync(UserId,ct);if(x is null){x=new(UserId);await users.AddPreferenceAsync(x,ct);}x.Update(r.Theme,r.Language,r.EmailNotifications,r.SecurityNotifications);await unit.SaveChangesAsync(ct);return Pref(x);}
    public async Task<IReadOnlyList<SessionDto>> ListSessionsAsync(Guid? currentSession,CancellationToken ct)=>(await users.ListSessionsAsync(UserId,ct)).Select(x=>new SessionDto(x.Id,x.DeviceInfo,x.IpAddress,x.CreatedAt,x.LastActivityAt,x.ExpiresAt,x.Id==currentSession)).ToArray();
    public async Task RevokeSessionAsync(Guid id,CancellationToken ct){var x=await users.GetSessionAsync(UserId,id,ct)??throw new NotFoundException("Session not found.");x.Revoke();await users.AddActivityAsync(new(UserId,"session.revoked","A session was revoked.",null,null),ct);await unit.SaveChangesAsync(ct);}
    public async Task RevokeOtherSessionsAsync(Guid? currentSession,CancellationToken ct){foreach(var x in await users.ListSessionsAsync(UserId,ct))if(x.Id!=currentSession)x.Revoke();await users.AddActivityAsync(new(UserId,"sessions.revoked","Other sessions were revoked.",null,null),ct);await unit.SaveChangesAsync(ct);}
    public async Task<IReadOnlyList<ActivityDto>> ListActivityAsync(CancellationToken ct)=>(await users.ListActivityAsync(UserId,100,ct)).Select(x=>new ActivityDto(x.Id,x.Action,x.Description,x.IpAddress,x.Device,x.CreatedAt)).ToArray();
    public async Task<IReadOnlyList<ApiKeyDto>> ListApiKeysAsync(CancellationToken ct)=>(await users.ListApiKeysAsync(UserId,ct)).Select(x=>new ApiKeyDto(x.Id,x.Name,x.Prefix,x.ExpiresAt,x.LastUsedAt,x.IsActive)).ToArray();
    public async Task<ApiKeyDto> CreateApiKeyAsync(CreateApiKeyRequest r,CancellationToken ct){if(string.IsNullOrWhiteSpace(r.Name))throw new ValidationException("API key name is required.");var raw="el_"+tokens.CreateOpaqueToken().Replace("+","").Replace("/","").TrimEnd('=');var key=new ApiKey(UserId,r.Name.Trim(),raw[..Math.Min(12,raw.Length)],tokens.HashRefreshToken(raw),r.ExpiresAt);await users.AddApiKeyAsync(key,ct);await users.AddActivityAsync(new(UserId,"api-key.created",$"API key '{key.Name}' created.",null,null),ct);await unit.SaveChangesAsync(ct);return new(key.Id,key.Name,key.Prefix,key.ExpiresAt,null,true,raw);}
    public async Task RevokeApiKeyAsync(Guid id,CancellationToken ct){var x=await users.GetApiKeyAsync(UserId,id,ct)??throw new NotFoundException("API key not found.");x.Revoke();await users.AddActivityAsync(new(UserId,"api-key.revoked",$"API key '{x.Name}' revoked.",null,null),ct);await unit.SaveChangesAsync(ct);}
}
