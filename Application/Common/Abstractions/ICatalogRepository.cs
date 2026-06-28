using Sanaclub.Application.Catalogs.Common;

namespace Sanaclub.Application.Common.Abstractions;

public interface ICatalogRepository
{
    Task<IReadOnlyCollection<CatalogItemResponse>> ListIdentificationTypesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CatalogItemResponse>> ListGendersAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CatalogItemResponse>> ListCivilStatusesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CatalogItemResponse>> ListPatientStatusesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CatalogItemResponse>> ListDocumentTypesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CatalogItemResponse>> ListConsentStatusesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CatalogItemResponse>> ListTreatmentStatusesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CatalogItemResponse>> ListEvolutionStatusesAsync(
        CancellationToken cancellationToken = default);

    Task<Guid?> GetConsentStatusIdByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<Guid?> GetTreatmentStatusIdByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<Guid?> GetEvolutionStatusIdByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);
}
