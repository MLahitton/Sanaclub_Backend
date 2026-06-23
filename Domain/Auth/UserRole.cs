using Sanaclub.Domain.Common;

namespace Sanaclub.Domain.Auth;

public sealed class UserRole : AuditableEntity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime AssignedAtUtc { get; private set; }
    public Guid? AssignedByUserId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public Guid? RevokedByUserId { get; private set; }
    public string? RevocationReason { get; private set; }

    private UserRole()
    {
    }

    public UserRole(Guid userId, Guid roleId, Guid? assignedByUserId = null)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException("El identificador del usuario es obligatorio.");
        }

        if (roleId == Guid.Empty)
        {
            throw new DomainException("El identificador del rol es obligatorio.");
        }

        UserId = userId;
        RoleId = roleId;
        AssignedAtUtc = DateTime.UtcNow;
        AssignedByUserId = assignedByUserId;
        IsActive = true;
    }

    public void Revoke(Guid? revokedByUserId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainException("La razón de revocación es obligatoria.");
        }

        IsActive = false;
        RevokedAtUtc = DateTime.UtcNow;
        RevokedByUserId = revokedByUserId;
        RevocationReason = reason.Trim();
    }

    public void Reactivate(Guid? assignedByUserId)
    {
        IsActive = true;
        AssignedAtUtc = DateTime.UtcNow;
        AssignedByUserId = assignedByUserId;
        RevokedAtUtc = null;
        RevokedByUserId = null;
        RevocationReason = null;
    }
}

