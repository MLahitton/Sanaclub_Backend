namespace Sanaclub.Domain.Catalogs;

public sealed class CivilStatus : CatalogEntity
{
    private CivilStatus()
    {
    }

    public CivilStatus(string code, string name, string? description = null, int sortOrder = 0)
        : base(code, name, description, sortOrder)
    {
    }
}
