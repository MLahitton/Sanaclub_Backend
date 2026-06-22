namespace Sanaclub.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public Guid? CreatedByUserId { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }

    protected AuditableEntity()
    {
    }

    protected AuditableEntity(Guid id) : base(id)
    {
    }

    public void MarkAsCreated(Guid? userId)
    {
        CreatedAtUtc = DateTime.UtcNow;
        CreatedByUserId = userId;
    }

    public void MarkAsUpdated(Guid? userId)
    {
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedByUserId = userId;
    }
}