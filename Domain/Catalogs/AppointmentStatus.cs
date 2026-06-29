namespace Sanaclub.Domain.Catalogs;

public sealed class AppointmentStatus : CatalogEntity
{
    private AppointmentStatus()
    {
    }

    public AppointmentStatus(string code, string name, string? description = null, int sortOrder = 0)
        : base(code, name, description, sortOrder)
    {
    }
}
