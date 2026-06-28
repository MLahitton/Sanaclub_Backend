using Sanaclub.Domain.Documents;

namespace Sanaclub.Application.Common.Abstractions;

public interface IGeneratedDocumentRepository
{
    Task AddAsync(
        GeneratedDocument document,
        CancellationToken cancellationToken = default);

    Task<GeneratedDocument?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GeneratedDocument>> ListByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
