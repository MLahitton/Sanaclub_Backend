using System;
using Sanaclub.Domain.Common;

namespace Sanaclub.Domain.TreatmentSheets;

public sealed class TreatmentSheet : AuditableEntity
{
    public Guid PatientId { get; private set; }
    public Guid TreatmentStatusId { get; private set; }
    public string? TreatmentNumber { get; private set; }
    public DateOnly? ConsultationDate { get; private set; }
    public string? EpsTreatingDoctorDiagnosis { get; private set; }
    public string? ReferredClinicalHistory { get; private set; }

    public DateOnly? IndicationDate { get; private set; }
    public TimeOnly? EntryTime { get; private set; }
    public TimeOnly? ExitTime { get; private set; }
    public string? AssignedStaffName { get; private set; }
    public string? TherapyName { get; private set; }
    public string? NervousSystemIndications { get; private set; }

    public bool DecompressSpine { get; private set; }
    public bool DecompressNeck { get; private set; }
    public bool DecompressBack { get; private set; }

    public bool EndocrineNerves { get; private set; }
    public bool EndocrineDefenses { get; private set; }
    public bool EndocrineHormones { get; private set; }

    public string? CardiovascularReflexologyWith { get; private set; }
    public string? DigestiveColonReflexologyWith { get; private set; }
    public string? RespiratoryReflexologyWith { get; private set; }
    public string? UrinaryReflexologyWithAcidFruits { get; private set; }

    public string? OtherIndications { get; private set; }
    public string? Observations { get; private set; }

    public bool IsActive { get; private set; }

    private TreatmentSheet()
    {
    }

    public TreatmentSheet(
        Guid patientId,
        Guid treatmentStatusId,
        string? treatmentNumber,
        DateOnly? consultationDate,
        string? epsTreatingDoctorDiagnosis,
        string? referredClinicalHistory)
    {
        if (patientId == Guid.Empty)
        {
            throw new DomainException("El identificador del paciente es obligatorio.");
        }

        if (treatmentStatusId == Guid.Empty)
        {
            throw new DomainException("El estado de tratamiento es obligatorio.");
        }

        var trimmedTreatmentNumber = NormalizeOptionalText(treatmentNumber);
        if (trimmedTreatmentNumber?.Length > 50)
        {
            throw new DomainException("El número de tratamiento no puede superar los 50 caracteres.");
        }

        var trimmedEpsTreatingDoctorDiagnosis = NormalizeOptionalText(epsTreatingDoctorDiagnosis);
        if (trimmedEpsTreatingDoctorDiagnosis?.Length > 1000)
        {
            throw new DomainException("El diagnóstico del médico tratante EPS no puede superar los 1000 caracteres.");
        }

        var trimmedReferredClinicalHistory = NormalizeOptionalText(referredClinicalHistory);
        if (trimmedReferredClinicalHistory?.Length > 4000)
        {
            throw new DomainException("La historia clínica referida no puede superar los 4000 caracteres.");
        }

        PatientId = patientId;
        TreatmentStatusId = treatmentStatusId;
        TreatmentNumber = trimmedTreatmentNumber;
        ConsultationDate = consultationDate;
        EpsTreatingDoctorDiagnosis = trimmedEpsTreatingDoctorDiagnosis;
        ReferredClinicalHistory = trimmedReferredClinicalHistory;
        IsActive = true;
    }

    public void UpdateMedicalIndication(
        DateOnly? indicationDate,
        TimeOnly? entryTime,
        TimeOnly? exitTime,
        string? assignedStaffName,
        string? therapyName,
        string? nervousSystemIndications,
        bool decompressSpine,
        bool decompressNeck,
        bool decompressBack,
        bool endocrineNerves,
        bool endocrineDefenses,
        bool endocrineHormones,
        string? cardiovascularReflexologyWith,
        string? digestiveColonReflexologyWith,
        string? respiratoryReflexologyWith,
        string? urinaryReflexologyWithAcidFruits,
        string? otherIndications,
        string? observations,
        Guid? updatedByUserId)
    {
        if (!IsActive)
        {
            throw new DomainException("La hoja de tratamiento está inactiva.");
        }

        var trimmedAssignedStaffName = NormalizeOptionalText(assignedStaffName);
        if (trimmedAssignedStaffName?.Length > 200)
        {
            throw new DomainException("El nombre del personal asignado no puede superar los 200 caracteres.");
        }

        var trimmedTherapyName = NormalizeOptionalText(therapyName);
        if (trimmedTherapyName?.Length > 200)
        {
            throw new DomainException("El nombre de terapia no puede superar los 200 caracteres.");
        }

        var trimmedNervousSystemIndications = NormalizeOptionalText(nervousSystemIndications);
        if (trimmedNervousSystemIndications?.Length > 1000)
        {
            throw new DomainException("Las indicaciones del sistema nervioso no pueden superar los 1000 caracteres.");
        }

        var trimmedCardiovascularReflexologyWith = NormalizeOptionalText(cardiovascularReflexologyWith);
        if (trimmedCardiovascularReflexologyWith?.Length > 500)
        {
            throw new DomainException(
                "La indicación de reflexología cardiovascular no puede superar los 500 caracteres.");
        }

        var trimmedDigestiveColonReflexologyWith = NormalizeOptionalText(digestiveColonReflexologyWith);
        if (trimmedDigestiveColonReflexologyWith?.Length > 500)
        {
            throw new DomainException(
                "La indicación de reflexología de colon digestivo no puede superar los 500 caracteres.");
        }

        var trimmedRespiratoryReflexologyWith = NormalizeOptionalText(respiratoryReflexologyWith);
        if (trimmedRespiratoryReflexologyWith?.Length > 500)
        {
            throw new DomainException("La indicación de reflexología respiratoria no puede superar los 500 caracteres.");
        }

        var trimmedUrinaryReflexologyWithAcidFruits = NormalizeOptionalText(urinaryReflexologyWithAcidFruits);
        if (trimmedUrinaryReflexologyWithAcidFruits?.Length > 500)
        {
            throw new DomainException("La indicación de reflexología urinaria no puede superar los 500 caracteres.");
        }

        var trimmedOtherIndications = NormalizeOptionalText(otherIndications);
        if (trimmedOtherIndications?.Length > 2000)
        {
            throw new DomainException("Otras indicaciones no puede superar los 2000 caracteres.");
        }

        var trimmedObservations = NormalizeOptionalText(observations);
        if (trimmedObservations?.Length > 2000)
        {
            throw new DomainException("Las observaciones no pueden superar los 2000 caracteres.");
        }

        if (entryTime.HasValue && exitTime.HasValue && exitTime.Value < entryTime.Value)
        {
            throw new DomainException("La hora de salida no puede ser menor a la hora de entrada.");
        }

        IndicationDate = indicationDate;
        EntryTime = entryTime;
        ExitTime = exitTime;
        AssignedStaffName = trimmedAssignedStaffName;
        TherapyName = trimmedTherapyName;
        NervousSystemIndications = trimmedNervousSystemIndications;
        DecompressSpine = decompressSpine;
        DecompressNeck = decompressNeck;
        DecompressBack = decompressBack;
        EndocrineNerves = endocrineNerves;
        EndocrineDefenses = endocrineDefenses;
        EndocrineHormones = endocrineHormones;
        CardiovascularReflexologyWith = trimmedCardiovascularReflexologyWith;
        DigestiveColonReflexologyWith = trimmedDigestiveColonReflexologyWith;
        RespiratoryReflexologyWith = trimmedRespiratoryReflexologyWith;
        UrinaryReflexologyWithAcidFruits = trimmedUrinaryReflexologyWithAcidFruits;
        OtherIndications = trimmedOtherIndications;
        Observations = trimmedObservations;

        MarkAsUpdated(updatedByUserId);
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
