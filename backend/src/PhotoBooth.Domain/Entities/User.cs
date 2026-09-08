using PhotoBooth.Domain.Common;

namespace PhotoBooth.Domain.Entities;

public sealed class User : Entity
{
    private User() { }

    public User(string email, string passwordHash, string displayName)
    {
        Email = NormalizeEmail(email);
        NormalizedEmail = Email.ToUpperInvariant();
        PasswordHash = string.IsNullOrWhiteSpace(passwordHash)
            ? throw new ArgumentException("Password hash is required.")
            : passwordHash;
        DisplayName = string.IsNullOrWhiteSpace(displayName)
            ? throw new ArgumentException("Display name is required.")
            : displayName.Trim();
    }

    public string Email { get; private set; } = string.Empty;
    public string NormalizedEmail { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string Role { get; private set; } = UserRoles.OrganizationOwner;
    public bool IsEmailVerified { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? LastLoginAtUtc { get; private set; }
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];

    public void RecordLogin() => LastLoginAtUtc = DateTime.UtcNow;
    public void VerifyEmail() => IsEmailVerified = true;
    public void Deactivate() => IsActive = false;

    private static string NormalizeEmail(string email) =>
        string.IsNullOrWhiteSpace(email) || !email.Contains('@')
            ? throw new ArgumentException("A valid email address is required.")
            : email.Trim().ToLowerInvariant();
}

public static class UserRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string OrganizationOwner = "OrganizationOwner";
    public const string EventManager = "EventManager";
    public const string Photographer = "Photographer";
    public const string Guest = "Guest";
}
