using System.Net.Mail;

namespace EventLensAI.Domain.ValueObjects;

public readonly record struct EmailAddress
{
    private EmailAddress(string value) => Value = value;
    public string Value { get; }

    public static EmailAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Email is required.");
        try
        {
            var parsed = new MailAddress(value.Trim());
            if (!string.Equals(parsed.Address, value.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Email is invalid.");
            return new EmailAddress(parsed.Address.ToLowerInvariant());
        }
        catch (FormatException)
        {
            throw new ArgumentException("Email is invalid.");
        }
    }

    public override string ToString() => Value;
}
