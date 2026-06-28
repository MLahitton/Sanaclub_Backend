using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.TreatmentSheets.Common;
using Sanaclub.Domain.TreatmentSheets;

namespace Sanaclub.Application.TreatmentSheets.GetById;

public sealed class GetTreatmentSheetByIdQuery : IRequest<TreatmentSheetResponse>
{
    public Guid TreatmentSheetId { get; init; }
}

public sealed class GetTreatmentSheetByIdQueryHandler :
    IRequestHandler<GetTreatmentSheetByIdQuery, TreatmentSheetResponse>
{
    private readonly ITreatmentSheetRepository _treatmentSheetRepository;

    public GetTreatmentSheetByIdQueryHandler(ITreatmentSheetRepository treatmentSheetRepository)
    {
        _treatmentSheetRepository = treatmentSheetRepository;
    }

    public async Task<TreatmentSheetResponse> Handle(
        GetTreatmentSheetByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.TreatmentSheetId == Guid.Empty)
        {
            throw new AppValidationException(
                "treatmentSheetId",
                "El identificador de la hoja de tratamiento es obligatorio.");
        }

        TreatmentSheet? treatmentSheet = await _treatmentSheetRepository.GetByIdAsync(
            request.TreatmentSheetId,
            cancellationToken);
        if (treatmentSheet is null)
        {
            throw new NotFoundException("La hoja de tratamiento no fue encontrada.");
        }

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
            ApprovedAtUtc = treatmentSheet.ApprovedAtUtc,
            ApprovedByUserId = treatmentSheet.ApprovedByUserId,
            IsActive = treatmentSheet.IsActive,
            CreatedAtUtc = treatmentSheet.CreatedAtUtc,
            UpdatedAtUtc = treatmentSheet.UpdatedAtUtc
        };
    }
}
