using Sanaclub.Domain.Consents;
using Sanaclub.Domain.Patients;

namespace Sanaclub.Application.Common.Abstractions;

public interface IInformedConsentPdfGenerator
{
    Task<byte[]> GenerateAsync(
        InformedConsent consent,
        Patient patient,
        string? documentTypeName,
        string? identificationType,
        CancellationToken cancellationToken = default);
}
