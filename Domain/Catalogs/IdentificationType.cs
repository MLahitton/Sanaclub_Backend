namespace Sanaclub.Domain.Catalogs;

public sealed class IdentificationType : CatalogEntity
{
    private IdentificationType()
    {
    }

    public IdentificationType(string code, string name, string? description = null, int sortOrder = 0)
        : base(code, name, description, sortOrder)
    {
    }
}
