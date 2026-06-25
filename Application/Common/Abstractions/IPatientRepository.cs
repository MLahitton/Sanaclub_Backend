using Sanaclub.Domain.Patients;

namespace Sanaclub.Application.Common.Abstractions;

public interface IPatientRepository
{
    Task<bool> ExistsByIdentificationAsync(
        Guid identificationTypeId,
        string identificationNumber,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Patient patient,
        CancellationToken cancellationToken = default);

    Task<Patient?> GetByIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Patient>> ListAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        string? search,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
