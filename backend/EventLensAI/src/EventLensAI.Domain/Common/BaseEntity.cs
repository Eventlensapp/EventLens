namespace EventLensAI.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public Guid? CreatedBy { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }
    public bool IsDeleted { get; protected set; }

    public void MarkUpdated(Guid? actorId)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = actorId;
    }

    public void SoftDelete(Guid? actorId)
    {
        IsDeleted = true;
        MarkUpdated(actorId);
    }
    public void Restore(Guid? actorId)
    {
        IsDeleted = false;
        MarkUpdated(actorId);
    }
}
