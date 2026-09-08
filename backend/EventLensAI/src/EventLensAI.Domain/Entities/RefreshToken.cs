using EventLensAI.Domain.Common;

namespace EventLensAI.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    private RefreshToken() { }
    public RefreshToken(Guid userId, string tokenHash, Guid familyId, DateTime expiresAt, string? deviceInfo = null, string? ipAddress = null)
    {
        UserId = userId;
        TokenHash = tokenHash;
        FamilyId = familyId;
        ExpiresAt = expiresAt;
        DeviceInfo = deviceInfo; IpAddress = ipAddress; LastActivityAt = DateTime.UtcNow;
    }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string TokenHash { get; private set; } = string.Empty;
    public Guid FamilyId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }
    public string? DeviceInfo { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime LastActivityAt { get; private set; }
    public bool IsActive => !IsDeleted && RevokedAt is null && ExpiresAt > DateTime.UtcNow;
    public void Revoke(Guid? replacementId = null)
    {
        if (RevokedAt is not null) return;
        RevokedAt = DateTime.UtcNow;
        ReplacedByTokenId = replacementId;
    }
    public void Touch() => LastActivityAt = DateTime.UtcNow;
}
