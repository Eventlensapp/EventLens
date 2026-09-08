using PhotoBooth.Domain.Common;

namespace PhotoBooth.Domain.Entities;

public sealed class RefreshToken : Entity
{
    private RefreshToken() { }

    public RefreshToken(Guid userId, string tokenHash, Guid familyId, DateTime expiresAtUtc)
    {
        UserId = userId;
        TokenHash = string.IsNullOrWhiteSpace(tokenHash)
            ? throw new ArgumentException("Token hash is required.")
            : tokenHash;
        FamilyId = familyId;
        ExpiresAtUtc = expiresAtUtc;
    }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string TokenHash { get; private set; } = string.Empty;
    public Guid FamilyId { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }
    public bool IsActive => RevokedAtUtc is null && ExpiresAtUtc > DateTime.UtcNow;

    public void Revoke(Guid? replacementId = null)
    {
        if (RevokedAtUtc is not null) return;
        RevokedAtUtc = DateTime.UtcNow;
        ReplacedByTokenId = replacementId;
    }
}
