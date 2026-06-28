using Sanaclub.Domain.EvolutionSheets;

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

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
