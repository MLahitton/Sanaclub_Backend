using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Domain.EvolutionSheets;
using Sanaclub.Domain.TreatmentSheets;

namespace Sanaclub.Application.Appointments.Common;

public static class AppointmentClinicalReferenceResolver
{
    public static async Task<ClinicalReferenceResult> ResolveAsync(
        Guid patientId,
        ICatalogRepository catalogRepository,
        ITreatmentSheetRepository treatmentSheetRepository,
        IEvolutionSheetRepository evolutionSheetRepository,
        CancellationToken cancellationToken = default)
    {
        var approvedTreatmentStatusId = await catalogRepository.GetTreatmentStatusIdByCodeAsync(
            "APPROVED",
            cancellationToken);

        var completedEvolutionStatusId = await catalogRepository.GetEvolutionStatusIdByCodeAsync(
            "COMPLETED",
            cancellationToken);

        if (approvedTreatmentStatusId is null || completedEvolutionStatusId is null)
        {
            throw new ConflictException("No se pudo determinar el documento clínico vigente del paciente.");
        }

        var treatmentSheets = await treatmentSheetRepository.ListByPatientIdAsync(patientId, cancellationToken);
        var latestApprovedTreatmentSheet = treatmentSheets
            .Where(x => x.TreatmentStatusId == approvedTreatmentStatusId.Value)
            .OrderByDescending(GetTreatmentSheetReferenceDate)
            .FirstOrDefault();

        if (latestApprovedTreatmentSheet is null)
        {
            throw new ConflictException("El paciente no tiene una hoja de tratamiento aprobada para agendar una cita.");
        }

        var treatmentSheetIds = treatmentSheets
            .Where(x => x.PatientId == patientId)
            .Select(x => x.Id)
            .ToHashSet();

        var evolutionSheets = await evolutionSheetRepository.ListByPatientIdAsync(patientId, cancellationToken);
        var latestCompletedEvolutionSheet = evolutionSheets
            .Where(x => x.EvolutionStatusId == completedEvolutionStatusId.Value
                        && treatmentSheetIds.Contains(x.TreatmentSheetId))
            .OrderByDescending(GetEvolutionSheetReferenceDate)
            .FirstOrDefault();

        if (latestCompletedEvolutionSheet is null)
        {
            return new ClinicalReferenceResult
            {
                TreatmentSheetId = latestApprovedTreatmentSheet.Id,
                ClinicalReferenceType = AppointmentConstants.TreatmentSheetClinicalReferenceType,
                ClinicalReferenceId = latestApprovedTreatmentSheet.Id
            };
        }

        var treatmentSheetDate = GetTreatmentSheetReferenceDate(latestApprovedTreatmentSheet);
        var evolutionSheetDate = GetEvolutionSheetReferenceDate(latestCompletedEvolutionSheet);

        if (evolutionSheetDate >= treatmentSheetDate)
        {
            return new ClinicalReferenceResult
            {
                TreatmentSheetId = latestCompletedEvolutionSheet.TreatmentSheetId,
                ClinicalReferenceType = AppointmentConstants.EvolutionSheetClinicalReferenceType,
                ClinicalReferenceId = latestCompletedEvolutionSheet.Id
            };
        }

        return new ClinicalReferenceResult
        {
            TreatmentSheetId = latestApprovedTreatmentSheet.Id,
            ClinicalReferenceType = AppointmentConstants.TreatmentSheetClinicalReferenceType,
            ClinicalReferenceId = latestApprovedTreatmentSheet.Id
        };
    }

    private static DateTime GetTreatmentSheetReferenceDate(TreatmentSheet treatmentSheet)
    {
        return treatmentSheet.ApprovedAtUtc
            ?? treatmentSheet.UpdatedAtUtc
            ?? treatmentSheet.CreatedAtUtc;
    }

    private static DateTime GetEvolutionSheetReferenceDate(EvolutionSheet evolutionSheet)
    {
        return evolutionSheet.CompletedAtUtc
            ?? evolutionSheet.UpdatedAtUtc
            ?? evolutionSheet.CreatedAtUtc;
    }
}
