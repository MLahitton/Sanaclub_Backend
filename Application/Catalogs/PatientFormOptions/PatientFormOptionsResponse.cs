using Sanaclub.Application.Catalogs.Common;

namespace Sanaclub.Application.Catalogs.PatientFormOptions;

public sealed class PatientFormOptionsResponse
{
    public IReadOnlyCollection<CatalogItemResponse> IdentificationTypes { get; init; } = Array.Empty<CatalogItemResponse>();
    public IReadOnlyCollection<CatalogItemResponse> Genders { get; init; } = Array.Empty<CatalogItemResponse>();
    public IReadOnlyCollection<CatalogItemResponse> CivilStatuses { get; init; } = Array.Empty<CatalogItemResponse>();
    public IReadOnlyCollection<CatalogItemResponse> PatientStatuses { get; init; } = Array.Empty<CatalogItemResponse>();
}
