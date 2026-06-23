namespace Sanaclub.Domain.Catalogs;

public sealed class TreatmentStatus : CatalogEntity
{
    private TreatmentStatus()
    {
    }

    public TreatmentStatus(string code, string name, string? description = null, int sortOrder = 0)
        : base(code, name, description, sortOrder)
    {
    }
}
