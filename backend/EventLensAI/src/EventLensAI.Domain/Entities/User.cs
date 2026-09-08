using EventLensAI.Domain.Common;
using EventLensAI.Domain.ValueObjects;

namespace EventLensAI.Domain.Entities;

public sealed class User : BaseEntity
{
    private User() { }
    public User(string firstName, string lastName, string email, string passwordHash, string? phone)
    {
        FirstName = Required(firstName, nameof(firstName), 100);
        LastName = Required(lastName, nameof(lastName), 100);
        Email = EmailAddress.Create(email).Value;
        NormalizedEmail = Email.ToUpperInvariant();
        PasswordHash = Required(passwordHash, nameof(passwordHash), 512);
        Phone = phone?.Trim();
    }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string NormalizedEmail { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? ProfileImage { get; private set; }
    public string TimeZone { get; private set; } = "UTC";
    public string Language { get; private set; } = "en";
    public bool EmailVerified { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int FailedLoginCount { get; private set; }
    public DateTime? LockoutEnd { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public ICollection<OrganizationMember> Memberships { get; private set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];

    public void RecordLogin() { LastLoginAt = DateTime.UtcNow; FailedLoginCount = 0; LockoutEnd = null; }
    public void RecordFailedLogin() { FailedLoginCount++; if (FailedLoginCount >= 5) LockoutEnd = DateTime.UtcNow.AddMinutes(15); }
    public bool IsLockedOut => LockoutEnd > DateTime.UtcNow;
    public void VerifyEmail() => EmailVerified = true;
    public void ChangePassword(string hash) => PasswordHash = Required(hash, nameof(hash), 512);
    public void UpdateProfile(string firstName, string lastName, string? phone, string? image, string timeZone, string language)
    {
        FirstName = Required(firstName, nameof(firstName), 100); LastName = Required(lastName, nameof(lastName), 100);
        Phone = phone?.Trim(); ProfileImage = image?.Trim(); TimeZone = Required(timeZone, nameof(timeZone), 100);
        Language = Required(language, nameof(language), 10);
    }

    private static string Required(string value, string name, int maxLength) =>
        string.IsNullOrWhiteSpace(value) || value.Trim().Length > maxLength
            ? throw new ArgumentException($"{name} is required and must not exceed {maxLength} characters.")
            : value.Trim();
}
