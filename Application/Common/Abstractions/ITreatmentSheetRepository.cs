using Sanaclub.Domain.TreatmentSheets;

namespace Sanaclub.Application.Common.Abstractions;

public interface ITreatmentSheetRepository
{
    Task AddAsync(
        TreatmentSheet treatmentSheet,
        CancellationToken cancellationToken = default);

    Task<TreatmentSheet?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TreatmentSheet?> GetByIdForUpdateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TreatmentSheet>> ListByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

