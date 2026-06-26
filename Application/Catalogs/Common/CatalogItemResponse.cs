using Sanaclub.Application.Catalogs.Common;

namespace Sanaclub.Application.Catalogs.Common;

public sealed class CatalogItemResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}
