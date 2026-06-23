namespace Sanaclub.Infrastructure.Persistence.SeedData;

public sealed class SeedCatalogItem
{
    public SeedCatalogItem(
        Guid id,
        string code,
        string name,
        string? description,
        int sortOrder)
    {
        Id = id;
        Code = code;
        Name = name;
        Description = description;
        SortOrder = sortOrder;
    }

    public Guid Id { get; }
    public string Code { get; }
    public string Name { get; }
    public string? Description { get; }
    public int SortOrder { get; }
}
