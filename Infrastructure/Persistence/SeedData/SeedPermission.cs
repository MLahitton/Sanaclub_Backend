namespace Sanaclub.Infrastructure.Persistence.SeedData;

public sealed class SeedPermission
{
    public SeedPermission(
        Guid id,
        string code,
        string name,
        string module,
        string? description)
    {
        Id = id;
        Code = code;
        Name = name;
        Module = module;
        Description = description;
    }

    public Guid Id { get; }
    public string Code { get; }
    public string Name { get; }
    public string Module { get; }
    public string? Description { get; }
}
