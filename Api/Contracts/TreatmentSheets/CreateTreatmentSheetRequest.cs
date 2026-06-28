using System;

namespace Sanaclub.Api.Contracts.TreatmentSheets;

public sealed class CreateTreatmentSheetRequest
{
    public string? TreatmentNumber { get; init; }
    public DateOnly? ConsultationDate { get; init; }
    public string? EpsTreatingDoctorDiagnosis { get; init; }
    public string? ReferredClinicalHistory { get; init; }
}

