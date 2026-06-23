using Sanaclub.Domain.Common;

namespace Sanaclub.Domain.Auth;

public sealed class Permission : AuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Module { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private Permission()
    {
    }

    public Permission(string code, string name, string module, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("El código del permiso es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El nombre del permiso es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(module))
        {
            throw new DomainException("El módulo es obligatorio.");
        }

        Code = code.Trim().ToLowerInvariant();
        Name = name.Trim();
        Module = module.Trim().ToLowerInvariant();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        IsActive = true;
    }

    public void Rename(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El nombre del permiso es obligatorio.");
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

