namespace Sanaclub.Domain.Catalogs;

public sealed class PatientStatus : CatalogEntity
{
    private PatientStatus()
    {
    }

    public PatientStatus(string code, string name, string? description = null, int sortOrder = 0)
        : base(code, name, description, sortOrder)
    {
    }
}
