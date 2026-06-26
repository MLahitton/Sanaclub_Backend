using Sanaclub.Domain.Consents;

namespace Sanaclub.Application.Common.Abstractions;

public interface IConsentRepository
{
    Task AddAsync(
        InformedConsent consent,
        CancellationToken cancellationToken = default);

    Task<InformedConsent?> GetByIdAsync(
        Guid consentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InformedConsent>> ListByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}

