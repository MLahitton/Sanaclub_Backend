using Sanaclub.Domain.Common;

namespace Sanaclub.Domain.Auth;

public sealed class Role : AuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsSystemRole { get; private set; }

    private Role()
    {
    }

    public Role(string code, string name, string? description = null, bool isSystemRole = false)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("El código del rol es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El nombre del rol es obligatorio.");
        }

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        IsActive = true;
        IsSystemRole = isSystemRole;
    }

    public void Rename(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El nombre del rol es obligatorio.");
        }

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}

