namespace Sanaclub.Domain.Catalogs;

public sealed class ConsentStatus : CatalogEntity
{
    private ConsentStatus()
    {
    }

    public ConsentStatus(string code, string name, string? description = null, int sortOrder = 0)
        : base(code, name, description, sortOrder)
    {
    }
}
