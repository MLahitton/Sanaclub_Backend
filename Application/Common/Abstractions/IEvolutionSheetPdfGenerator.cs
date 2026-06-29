using Sanaclub.Domain.EvolutionSheets;
using Sanaclub.Domain.Patients;
using Sanaclub.Domain.TreatmentSheets;

namespace Sanaclub.Application.Common.Abstractions;

public interface IEvolutionSheetPdfGenerator
{
    Task<byte[]> GenerateAsync(
        Patient patient,
        EvolutionSheet evolutionSheet,
        TreatmentSheet treatmentSheet,
        CancellationToken cancellationToken = default);
}
