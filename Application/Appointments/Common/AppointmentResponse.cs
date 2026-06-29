namespace Sanaclub.Application.Appointments.Common;

public sealed class AppointmentResponse
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public string PatientFullName { get; init; } = string.Empty;
    public string PatientIdentificationNumber { get; init; } = string.Empty;
    public Guid TreatmentSheetId { get; init; }
    public string? TreatmentNumber { get; init; }
    public string ClinicalReferenceType { get; init; } = string.Empty;
    public Guid ClinicalReferenceId { get; init; }
    public string ClinicalReferenceLabel { get; init; } = string.Empty;
    public Guid TherapistUserId { get; init; }
    public string TherapistFullName { get; init; } = string.Empty;
    public DateOnly AppointmentDate { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public Guid AppointmentStatusId { get; init; }
    public string StatusCode { get; init; } = string.Empty;
    public string StatusName { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public Guid ScheduledByUserId { get; init; }
    public DateTime? ConfirmedAtUtc { get; init; }
    public Guid? ConfirmedByUserId { get; init; }
    public DateTime? CancelledAtUtc { get; init; }
    public Guid? CancelledByUserId { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
