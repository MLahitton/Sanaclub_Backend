using Sanaclub.Domain.TreatmentSheets;
using Sanaclub.Application.TreatmentSheets.PendingMedicalIndication;

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

    Task<IReadOnlyCollection<PendingTreatmentSheetResponse>> ListPendingMedicalIndicationAsync(
        Guid draftStatusId,
        string? search,
        DateOnly? fromDate,
        DateOnly? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountPendingMedicalIndicationAsync(
        Guid draftStatusId,
        string? search,
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
