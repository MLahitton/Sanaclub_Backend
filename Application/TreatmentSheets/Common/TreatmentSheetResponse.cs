namespace Sanaclub.Application.TreatmentSheets.Common;

public sealed class TreatmentSheetResponse
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
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

