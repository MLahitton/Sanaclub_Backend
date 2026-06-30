namespace Sanaclub.Application.TreatmentSheets.PendingMedicalIndication;

public sealed class PendingTreatmentSheetResponse
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public string PatientFullName { get; init; } = string.Empty;
    public string PatientIdentificationNumber { get; init; } = string.Empty;
    public string? TreatmentNumber { get; init; }
    public DateOnly? ConsultationDate { get; init; }
    public string? EpsDiagnosis { get; init; }
    public string? ReferredClinicalHistory { get; init; }
    public Guid TreatmentStatusId { get; init; }
    public string StatusCode { get; init; } = string.Empty;
    public string StatusName { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
