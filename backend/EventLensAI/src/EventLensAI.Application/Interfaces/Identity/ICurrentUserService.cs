namespace EventLensAI.Application.Interfaces.Identity;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    string? IPAddress { get; }
    IReadOnlyCollection<string> Roles { get; }
}
