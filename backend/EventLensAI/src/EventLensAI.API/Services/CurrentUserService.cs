using System.Security.Claims;
using EventLensAI.Application.Interfaces.Identity;

namespace EventLensAI.API.Services;

public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public Guid? UserId =>
        Guid.TryParse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId
            : null;
    public string? Email => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
    public string? IPAddress => accessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    public IReadOnlyCollection<string> Roles =>
        accessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray() ?? [];
}
