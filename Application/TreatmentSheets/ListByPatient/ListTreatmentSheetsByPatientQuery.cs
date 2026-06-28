using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.TreatmentSheets.Common;
using Sanaclub.Domain.Patients;
using Sanaclub.Domain.TreatmentSheets;

namespace Sanaclub.Application.TreatmentSheets.ListByPatient;

public sealed class ListTreatmentSheetsByPatientQuery : IRequest<IReadOnlyList<TreatmentSheetResponse>>
{
    public Guid PatientId { get; init; }
}

public sealed class ListTreatmentSheetsByPatientQueryHandler :
    IRequestHandler<ListTreatmentSheetsByPatientQuery, IReadOnlyList<TreatmentSheetResponse>>
{
    private readonly ITreatmentSheetRepository _treatmentSheetRepository;
    private readonly IPatientRepository _patientRepository;

    public ListTreatmentSheetsByPatientQueryHandler(
        ITreatmentSheetRepository treatmentSheetRepository,
        IPatientRepository patientRepository)
    {
        _treatmentSheetRepository = treatmentSheetRepository;
        _patientRepository = patientRepository;
    }

    public async Task<IReadOnlyList<TreatmentSheetResponse>> Handle(
        ListTreatmentSheetsByPatientQuery request,
        CancellationToken cancellationToken)
    {
        if (request.PatientId == Guid.Empty)
        {
            throw new AppValidationException(
                "patientId",
                "El identificador del paciente es obligatorio.");
        }

        Patient? patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("El paciente no fue encontrado.");
        }

        var treatmentSheets = await _treatmentSheetRepository.ListByPatientIdAsync(
            request.PatientId,
            cancellationToken);

        return treatmentSheets
            .Select(MapToResponse)
            .ToList()
            .AsReadOnly();
    }

    private static TreatmentSheetResponse MapToResponse(TreatmentSheet treatmentSheet)
    {
        return new TreatmentSheetResponse
        {
            Id = treatmentSheet.Id,
            PatientId = treatmentSheet.PatientId,
            TreatmentStatusId = treatmentSheet.TreatmentStatusId,
            TreatmentNumber = treatmentSheet.TreatmentNumber,
            ConsultationDate = treatmentSheet.ConsultationDate,
            EpsTreatingDoctorDiagnosis = treatmentSheet.EpsTreatingDoctorDiagnosis,
            ReferredClinicalHistory = treatmentSheet.ReferredClinicalHistory,
            IndicationDate = treatmentSheet.IndicationDate,
            EntryTime = treatmentSheet.EntryTime,
            ExitTime = treatmentSheet.ExitTime,
            AssignedStaffName = treatmentSheet.AssignedStaffName,
            TherapyName = treatmentSheet.TherapyName,
            NervousSystemIndications = treatmentSheet.NervousSystemIndications,
            DecompressSpine = treatmentSheet.DecompressSpine,
            DecompressNeck = treatmentSheet.DecompressNeck,
            DecompressBack = treatmentSheet.DecompressBack,
            EndocrineNerves = treatmentSheet.EndocrineNerves,
            EndocrineDefenses = treatmentSheet.EndocrineDefenses,
            EndocrineHormones = treatmentSheet.EndocrineHormones,
            CardiovascularReflexologyWith = treatmentSheet.CardiovascularReflexologyWith,
            DigestiveColonReflexologyWith = treatmentSheet.DigestiveColonReflexologyWith,
            RespiratoryReflexologyWith = treatmentSheet.RespiratoryReflexologyWith,
            UrinaryReflexologyWithAcidFruits = treatmentSheet.UrinaryReflexologyWithAcidFruits,
            OtherIndications = treatmentSheet.OtherIndications,
            Observations = treatmentSheet.Observations,
            ApprovedAtUtc = treatmentSheet.ApprovedAtUtc,
            ApprovedByUserId = treatmentSheet.ApprovedByUserId,
            IsActive = treatmentSheet.IsActive,
            CreatedAtUtc = treatmentSheet.CreatedAtUtc,
            UpdatedAtUtc = treatmentSheet.UpdatedAtUtc
        };
    }
}
