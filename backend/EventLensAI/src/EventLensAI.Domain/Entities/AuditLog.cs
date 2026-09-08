using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;

public sealed class AuditLog : BaseEntity
{
    private AuditLog() { }
    public AuditLog(Guid userId, string resourceType, Guid resourceId, AuditAction action, string? ipAddress)
    {
        UserId = userId; ResourceType = resourceType; ResourceId = resourceId; Action = action; IPAddress = ipAddress;
    }
    public Guid UserId { get; private set; }
    public string ResourceType { get; private set; } = string.Empty;
    public Guid ResourceId { get; private set; }
    public AuditAction Action { get; private set; }
    public string? IPAddress { get; private set; }
}
