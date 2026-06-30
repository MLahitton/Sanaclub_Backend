using Sanaclub.Domain.EvolutionSheets;
using Sanaclub.Application.EvolutionSheets.PendingNewIndications;

namespace Sanaclub.Application.Common.Abstractions;

public interface IEvolutionSheetRepository
{
    Task AddAsync(
        EvolutionSheet evolutionSheet,
        CancellationToken cancellationToken = default);

    Task<EvolutionSheet?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<EvolutionSheet?> GetByIdForUpdateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EvolutionSheet>> ListByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PendingEvolutionSheetResponse>> ListPendingNewIndicationsAsync(
        Guid draftStatusId,
        string? search,
        DateOnly? fromDate,
        DateOnly? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountPendingNewIndicationsAsync(
        Guid draftStatusId,
        string? search,
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
