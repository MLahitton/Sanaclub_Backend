using Sanaclub.Domain.Common;

namespace Sanaclub.Domain.Catalogs;

public abstract class CatalogEntity : AuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public int SortOrder { get; private set; }

    protected CatalogEntity()
    {
    }

    protected CatalogEntity(string code, string name, string? description = null, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("El código de catálogo es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El nombre de catálogo es obligatorio.");
        }

        if (sortOrder < 0)
        {
            throw new DomainException("El orden de ordenamiento no puede ser negativo.");
        }

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        IsActive = true;
        SortOrder = sortOrder;
    }

    public void Update(string name, string? description, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El nombre de catálogo es obligatorio.");
        }

        if (sortOrder < 0)
        {
            throw new DomainException("El orden de ordenamiento no puede ser negativo.");
        }

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        SortOrder = sortOrder;
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

