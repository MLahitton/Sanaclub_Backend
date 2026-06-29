using Sanaclub.Domain.Patients;
using Sanaclub.Domain.TreatmentSheets;

namespace Sanaclub.Application.Common.Abstractions;

public interface ITreatmentSheetPdfGenerator
{
    Task<byte[]> GenerateAsync(
        Patient patient,
        TreatmentSheet treatmentSheet,
        CancellationToken cancellationToken = default);
}
