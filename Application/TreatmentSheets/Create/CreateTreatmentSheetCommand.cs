using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.TreatmentSheets.Common;
using Sanaclub.Domain.Patients;
using Sanaclub.Domain.TreatmentSheets;

namespace Sanaclub.Application.TreatmentSheets.Create;

public sealed class CreateTreatmentSheetCommand : IRequest<TreatmentSheetResponse>
{
    public Guid PatientId { get; init; }
    public string? TreatmentNumber { get; init; }
    public DateOnly? ConsultationDate { get; init; }
    public string? EpsTreatingDoctorDiagnosis { get; init; }
    public string? ReferredClinicalHistory { get; init; }
    public Guid CreatedByUserId { get; init; }
}

public sealed class CreateTreatmentSheetCommandHandler :
    IRequestHandler<CreateTreatmentSheetCommand, TreatmentSheetResponse>
{
    private readonly ITreatmentSheetRepository _treatmentSheetRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly ICatalogRepository _catalogRepository;

    public CreateTreatmentSheetCommandHandler(
        ITreatmentSheetRepository treatmentSheetRepository,
        IPatientRepository patientRepository,
        ICatalogRepository catalogRepository)
    {
        _treatmentSheetRepository = treatmentSheetRepository;
        _patientRepository = patientRepository;
        _catalogRepository = catalogRepository;
    }

    public async Task<TreatmentSheetResponse> Handle(
        CreateTreatmentSheetCommand request,
        CancellationToken cancellationToken)
    {
        if (request.PatientId == Guid.Empty)
        {
            throw new AppValidationException(
                "patientId",
                "El identificador del paciente es obligatorio.");
        }

        if (request.CreatedByUserId == Guid.Empty)
        {
            throw new AppValidationException(
                "createdByUserId",
                "El identificador del usuario creador es obligatorio.");
        }

        Patient? patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("El paciente no fue encontrado.");
        }

        var treatmentStatusId = await _catalogRepository.GetTreatmentStatusIdByCodeAsync(
            "DRAFT",
            cancellationToken);
        if (!treatmentStatusId.HasValue)
        {
            throw new ConflictException("El estado DRAFT de tratamiento no está configurado.");
        }

        var treatmentSheet = new TreatmentSheet(
            request.PatientId,
            treatmentStatusId.Value,
            request.TreatmentNumber,
            request.ConsultationDate,
            request.EpsTreatingDoctorDiagnosis,
            request.ReferredClinicalHistory);

        treatmentSheet.MarkAsCreated(request.CreatedByUserId);

        await _treatmentSheetRepository.AddAsync(treatmentSheet, cancellationToken);
        await _treatmentSheetRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(treatmentSheet);
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
            IsActive = treatmentSheet.IsActive,
            CreatedAtUtc = treatmentSheet.CreatedAtUtc,
            UpdatedAtUtc = treatmentSheet.UpdatedAtUtc
        };
    }
}
