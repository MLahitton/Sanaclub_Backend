namespace Sanaclub.Infrastructure.Persistence.SeedData;

public sealed class SeedRole
{
    public SeedRole(
        Guid id,
        string code,
        string name,
        string? description,
        bool isSystemRole)
    {
        Id = id;
        Code = code;
        Name = name;
        Description = description;
        IsSystemRole = isSystemRole;
    }

    public Guid Id { get; }
    public string Code { get; }
    public string Name { get; }
    public string? Description { get; }
    public bool IsSystemRole { get; }
}
