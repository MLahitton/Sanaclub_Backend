using Sanaclub.Domain.Common;

namespace Sanaclub.Domain.Auth;

public sealed class RolePermission : AuditableEntity
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
    public DateTime AssignedAtUtc { get; private set; }
    public Guid? AssignedByUserId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public Guid? RevokedByUserId { get; private set; }
    public string? RevocationReason { get; private set; }

    private RolePermission()
    {
    }

    public RolePermission(Guid roleId, Guid permissionId, Guid? assignedByUserId = null)
    {
        if (roleId == Guid.Empty)
        {
            throw new DomainException("El identificador del rol es obligatorio.");
        }

        if (permissionId == Guid.Empty)
        {
            throw new DomainException("El identificador del permiso es obligatorio.");
        }

        RoleId = roleId;
        PermissionId = permissionId;
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

