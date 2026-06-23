namespace Sanaclub.Domain.Catalogs;

public sealed class EvolutionStatus : CatalogEntity
{
    private EvolutionStatus()
    {
    }

    public EvolutionStatus(string code, string name, string? description = null, int sortOrder = 0)
        : base(code, name, description, sortOrder)
    {
    }
}
