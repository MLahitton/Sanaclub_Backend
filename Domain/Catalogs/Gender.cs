namespace Sanaclub.Domain.Catalogs;

public sealed class Gender : CatalogEntity
{
    private Gender()
    {
    }

    public Gender(string code, string name, string? description = null, int sortOrder = 0)
        : base(code, name, description, sortOrder)
    {
    }
}
