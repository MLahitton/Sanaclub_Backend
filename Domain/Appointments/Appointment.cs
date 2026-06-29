using Sanaclub.Domain.Common;

namespace Sanaclub.Domain.Appointments;

public sealed class Appointment : AuditableEntity
{
    public const int FixedDurationMinutes = 135;
    public const int ClinicalReferenceTypeMaxLength = 40;
    public const int PatientNameSnapshotMaxLength = 220;
    public const int NotesMaxLength = 2000;

    public Guid PatientId { get; private set; }
    public Guid TreatmentSheetId { get; private set; }
    public string ClinicalReferenceType { get; private set; } = string.Empty;
    public Guid ClinicalReferenceId { get; private set; }
    public Guid TherapistUserId { get; private set; }
    public DateOnly AppointmentDate { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public Guid AppointmentStatusId { get; private set; }
    public string? PatientNameSnapshot { get; private set; }
    public Guid ScheduledByUserId { get; private set; }
    public DateTime? ConfirmedAtUtc { get; private set; }
    public Guid? ConfirmedByUserId { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public Guid? CancelledByUserId { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }

    private Appointment()
    {
    }

    public Appointment(
        Guid patientId,
        Guid treatmentSheetId,
        string clinicalReferenceType,
        Guid clinicalReferenceId,
        Guid therapistUserId,
        DateOnly appointmentDate,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid appointmentStatusId,
        string? patientNameSnapshot,
        Guid scheduledByUserId,
        string? notes)
    {
        if (patientId == Guid.Empty)
        {
            throw new DomainException("El paciente de la cita es obligatorio.");
        }

        if (treatmentSheetId == Guid.Empty)
        {
            throw new DomainException("La hoja de tratamiento base de la cita es obligatoria.");
        }

        if (string.IsNullOrWhiteSpace(clinicalReferenceType))
        {
            throw new DomainException("El tipo de referencia clinica de la cita es obligatorio.");
        }

        var trimmedClinicalReferenceType = clinicalReferenceType.Trim().ToUpperInvariant();
        if (trimmedClinicalReferenceType.Length > ClinicalReferenceTypeMaxLength)
        {
            throw new DomainException("El tipo de referencia clinica de la cita no puede superar 40 caracteres.");
        }

        if (clinicalReferenceId == Guid.Empty)
        {
            throw new DomainException("La referencia clinica de la cita es obligatoria.");
        }

        if (therapistUserId == Guid.Empty)
        {
            throw new DomainException("La terapeuta de la cita es obligatoria.");
        }

        if (appointmentStatusId == Guid.Empty)
        {
            throw new DomainException("El estado de la cita es obligatorio.");
        }

        if (scheduledByUserId == Guid.Empty)
        {
            throw new DomainException("El usuario que agenda la cita es obligatorio.");
        }

        var trimmedPatientNameSnapshot = NormalizeOptionalText(patientNameSnapshot);
        if (trimmedPatientNameSnapshot?.Length > PatientNameSnapshotMaxLength)
        {
            throw new DomainException("El nombre del paciente de la cita no puede superar 220 caracteres.");
        }

        var trimmedNotes = NormalizeOptionalText(notes);
        if (trimmedNotes?.Length > NotesMaxLength)
        {
            throw new DomainException("Las notas de la cita no pueden superar 2000 caracteres.");
        }

        PatientId = patientId;
        TreatmentSheetId = treatmentSheetId;
        ClinicalReferenceType = trimmedClinicalReferenceType;
        ClinicalReferenceId = clinicalReferenceId;
        TherapistUserId = therapistUserId;
        AppointmentDate = appointmentDate;
        StartTime = startTime;
        EndTime = endTime;
        AppointmentStatusId = appointmentStatusId;
        PatientNameSnapshot = trimmedPatientNameSnapshot;
        ScheduledByUserId = scheduledByUserId;
        Notes = trimmedNotes;
        IsActive = true;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
