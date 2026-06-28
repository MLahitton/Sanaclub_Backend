using System;
using Sanaclub.Domain.Common;

namespace Sanaclub.Domain.EvolutionSheets;

public sealed class EvolutionSheet : AuditableEntity
{
    public Guid PatientId { get; private set; }
    public Guid TreatmentSheetId { get; private set; }
    public Guid EvolutionStatusId { get; private set; }

    public string? TherapyNumber { get; private set; }
    public DateOnly? EvolutionDate { get; private set; }
    public TimeOnly? EntryTime { get; private set; }
    public TimeOnly? ExitTime { get; private set; }
    public string? AssignedStaffName { get; private set; }
    public string? TherapyName { get; private set; }
    public string EvolutionNotes { get; private set; } = string.Empty;

    public string? NewIndications { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }
    public Guid? CompletedByUserId { get; private set; }

    public bool IsActive { get; private set; }

    private EvolutionSheet()
    {
    }

    public EvolutionSheet(
        Guid patientId,
        Guid treatmentSheetId,
        Guid evolutionStatusId,
        string? therapyNumber,
        DateOnly? evolutionDate,
        TimeOnly? entryTime,
        TimeOnly? exitTime,
        string? assignedStaffName,
        string? therapyName,
        string evolutionNotes)
    {
        if (patientId == Guid.Empty)
        {
            throw new DomainException("El identificador del paciente es obligatorio.");
        }

        if (evolutionStatusId == Guid.Empty)
        {
            throw new DomainException("El estado de evoluciÃ³n es obligatorio.");
        }

        if (treatmentSheetId == Guid.Empty)
        {
            throw new DomainException("La hoja de tratamiento es obligatoria para crear una evoluciÃ³n.");
        }

        var trimmedEvolutionNotes = NormalizeOptionalText(evolutionNotes);
        if (trimmedEvolutionNotes is null)
        {
            throw new DomainException("La evoluciÃ³n es obligatoria.");
        }

        if (trimmedEvolutionNotes.Length > 4000)
        {
            throw new DomainException("La evoluciÃ³n no puede superar los 4000 caracteres.");
        }

        var trimmedTherapyNumber = NormalizeOptionalText(therapyNumber);
        if (trimmedTherapyNumber?.Length > 50)
        {
            throw new DomainException("El nÃºmero de terapia no puede superar los 50 caracteres.");
        }

        var trimmedAssignedStaffName = NormalizeOptionalText(assignedStaffName);
        if (trimmedAssignedStaffName?.Length > 200)
        {
            throw new DomainException("El nombre de la encargada no puede superar los 200 caracteres.");
        }

        var trimmedTherapyName = NormalizeOptionalText(therapyName);
        if (trimmedTherapyName?.Length > 200)
        {
            throw new DomainException("El nombre de la terapia no puede superar los 200 caracteres.");
        }

        if (entryTime.HasValue && exitTime.HasValue && exitTime.Value < entryTime.Value)
        {
            throw new DomainException("La hora de salida no puede ser menor a la hora de entrada.");
        }

        PatientId = patientId;
        TreatmentSheetId = treatmentSheetId;
        EvolutionStatusId = evolutionStatusId;
        TherapyNumber = trimmedTherapyNumber;
        EvolutionDate = evolutionDate;
        EntryTime = entryTime;
        ExitTime = exitTime;
        AssignedStaffName = trimmedAssignedStaffName;
        TherapyName = trimmedTherapyName;
        EvolutionNotes = trimmedEvolutionNotes;
        IsActive = true;
    }

    public void CompleteNewIndications(
        Guid draftStatusId,
        Guid completedStatusId,
        Guid completedByUserId,
        string newIndications)
    {
        if (!IsActive)
        {
            throw new DomainException("La evoluciÃ³n debe estar activa.");
        }

        if (draftStatusId == Guid.Empty)
        {
            throw new DomainException("El identificador del estado borrador es obligatorio.");
        }

        if (completedStatusId == Guid.Empty)
        {
            throw new DomainException("El identificador del estado completado es obligatorio.");
        }

        if (completedByUserId == Guid.Empty)
        {
            throw new DomainException("El identificador del usuario aprobador es obligatorio.");
        }

        if (EvolutionStatusId != draftStatusId)
        {
            throw new DomainException("Solo se puede completar una evoluciÃ³n en estado borrador.");
        }

        var trimmedNewIndications = NormalizeOptionalText(newIndications);
        if (trimmedNewIndications is null)
        {
            throw new DomainException("Las indicaciones nuevas son obligatorias.");
        }

        if (trimmedNewIndications.Length > 4000)
        {
            throw new DomainException("Las indicaciones nuevas no pueden superar los 4000 caracteres.");
        }

        NewIndications = trimmedNewIndications;
        EvolutionStatusId = completedStatusId;
        CompletedAtUtc = DateTime.UtcNow;
        CompletedByUserId = completedByUserId;

        MarkAsUpdated(completedByUserId);
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}

