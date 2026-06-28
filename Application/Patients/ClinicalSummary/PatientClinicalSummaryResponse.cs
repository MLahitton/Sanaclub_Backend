using System;
using System.Collections.Generic;

namespace Sanaclub.Application.Patients.ClinicalSummary;

public sealed class PatientClinicalSummaryResponse
{
    public PatientClinicalSummaryPatientResponse Patient { get; init; } = default!;
    public PatientClinicalSummaryCountsResponse Counts { get; init; } = default!;
    public IReadOnlyList<PatientClinicalSummaryConsentResponse> Consents { get; init; } =
        Array.Empty<PatientClinicalSummaryConsentResponse>();

    public IReadOnlyList<PatientClinicalSummaryTreatmentSheetResponse> TreatmentSheets { get; init; } =
        Array.Empty<PatientClinicalSummaryTreatmentSheetResponse>();

    public IReadOnlyList<PatientClinicalSummaryEvolutionSheetResponse> EvolutionSheets { get; init; } =
        Array.Empty<PatientClinicalSummaryEvolutionSheetResponse>();

    public PatientClinicalSummaryTreatmentSheetResponse? LatestApprovedTreatmentSheet { get; init; }
    public PatientClinicalSummaryEvolutionSheetResponse? LatestCompletedEvolutionSheet { get; init; }
}

public sealed class PatientClinicalSummaryPatientResponse
{
    public Guid Id { get; init; }
    public Guid IdentificationTypeId { get; init; }
    public string IdentificationNumber { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public DateOnly? BirthDate { get; init; }
    public Guid? GenderId { get; init; }
    public Guid? CivilStatusId { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public string? CityOrMunicipality { get; init; }
    public string? Occupation { get; init; }
    public string? EmergencyContactName { get; init; }
    public string? EmergencyContactRelationship { get; init; }
    public string? EmergencyContactPhone { get; init; }
    public Guid PatientStatusId { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

public sealed class PatientClinicalSummaryCountsResponse
{
    public int TotalConsents { get; init; }
    public int TotalTreatmentSheets { get; init; }
    public int TotalEvolutionSheets { get; init; }
    public int TotalApprovedTreatmentSheets { get; init; }
    public int TotalCompletedEvolutionSheets { get; init; }
}

public sealed class PatientClinicalSummaryConsentResponse
{
    public Guid Id { get; init; }
    public Guid DocumentTypeId { get; init; }
    public Guid ConsentStatusId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime? SignedAtUtc { get; init; }
    public Guid? SignedByUserId { get; init; }
    public string? PatientSignerName { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

public sealed class PatientClinicalSummaryTreatmentSheetResponse
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public Guid TreatmentStatusId { get; init; }
    public string? TreatmentNumber { get; init; }
    public DateOnly? ConsultationDate { get; init; }
    public string? EpsTreatingDoctorDiagnosis { get; init; }
    public string? ReferredClinicalHistory { get; init; }
    public DateOnly? IndicationDate { get; init; }
    public TimeOnly? EntryTime { get; init; }
    public TimeOnly? ExitTime { get; init; }
    public string? AssignedStaffName { get; init; }
    public string? TherapyName { get; init; }
    public string? NervousSystemIndications { get; init; }
    public bool DecompressSpine { get; init; }
    public bool DecompressNeck { get; init; }
    public bool DecompressBack { get; init; }
    public bool EndocrineNerves { get; init; }
    public bool EndocrineDefenses { get; init; }
    public bool EndocrineHormones { get; init; }
    public string? CardiovascularReflexologyWith { get; init; }
    public string? DigestiveColonReflexologyWith { get; init; }
    public string? RespiratoryReflexologyWith { get; init; }
    public string? UrinaryReflexologyWithAcidFruits { get; init; }
    public string? OtherIndications { get; init; }
    public string? Observations { get; init; }
    public DateTime? ApprovedAtUtc { get; init; }
    public Guid? ApprovedByUserId { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed class PatientClinicalSummaryEvolutionSheetResponse
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public Guid TreatmentSheetId { get; init; }
    public Guid EvolutionStatusId { get; init; }
    public string? TherapyNumber { get; init; }
    public DateOnly? EvolutionDate { get; init; }
    public TimeOnly? EntryTime { get; init; }
    public TimeOnly? ExitTime { get; init; }
    public string? AssignedStaffName { get; init; }
    public string? TherapyName { get; init; }
    public string EvolutionNotes { get; init; } = string.Empty;
    public string? NewIndications { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public Guid? CompletedByUserId { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
