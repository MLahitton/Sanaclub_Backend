using MediatR;
using System.Linq;
using Sanaclub.Domain.Common;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.TreatmentSheets.Common;
using Sanaclub.Domain.TreatmentSheets;

namespace Sanaclub.Application.TreatmentSheets.UpdateMedicalIndication;

public sealed class UpdateTreatmentSheetMedicalIndicationCommand : IRequest<TreatmentSheetResponse>
{
    public Guid TreatmentSheetId { get; init; }
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
    public Guid UpdatedByUserId { get; init; }
}

public sealed class UpdateTreatmentSheetMedicalIndicationCommandHandler :
    IRequestHandler<UpdateTreatmentSheetMedicalIndicationCommand, TreatmentSheetResponse>
{
    private readonly ITreatmentSheetRepository _treatmentSheetRepository;
    private readonly ICatalogRepository _catalogRepository;

    public UpdateTreatmentSheetMedicalIndicationCommandHandler(
        ITreatmentSheetRepository treatmentSheetRepository,
        ICatalogRepository catalogRepository)
    {
        _treatmentSheetRepository = treatmentSheetRepository;
        _catalogRepository = catalogRepository;
    }

    public async Task<TreatmentSheetResponse> Handle(
        UpdateTreatmentSheetMedicalIndicationCommand request,
        CancellationToken cancellationToken)
    {
        if (request.TreatmentSheetId == Guid.Empty)
        {
            throw new AppValidationException(
                "treatmentSheetId",
                "El identificador de la hoja de tratamiento es obligatorio.");
        }

        if (request.UpdatedByUserId == Guid.Empty)
        {
            throw new AppValidationException(
                "updatedByUserId",
                "El identificador del usuario actualizador es obligatorio.");
        }

        TreatmentSheet? treatmentSheet = await _treatmentSheetRepository.GetByIdForUpdateAsync(
            request.TreatmentSheetId,
            cancellationToken);
        if (treatmentSheet is null)
        {
            throw new NotFoundException("La hoja de tratamiento no fue encontrada.");
        }

        var draftStatusId = await _catalogRepository.GetTreatmentStatusIdByCodeAsync("DRAFT", cancellationToken);
        if (!draftStatusId.HasValue)
        {
            throw new ConflictException("El estado DRAFT de tratamiento no está configurado.");
        }

        var approvedStatusId = await _catalogRepository.GetTreatmentStatusIdByCodeAsync("APPROVED", cancellationToken);
        if (!approvedStatusId.HasValue)
        {
            throw new ConflictException("El estado APPROVED de tratamiento no está configurado.");
        }

        if (!treatmentSheet.IsActive)
        {
            throw new ConflictException("La hoja de tratamiento está inactiva.");
        }

        if (treatmentSheet.TreatmentStatusId != draftStatusId.Value)
        {
            throw new ConflictException("Solo se puede aprobar una hoja de tratamiento en estado borrador.");
        }

        try
        {
            treatmentSheet.UpdateMedicalIndicationAndApprove(
                request.IndicationDate,
                request.EntryTime,
                request.ExitTime,
                request.AssignedStaffName,
                request.TherapyName,
                request.NervousSystemIndications,
                request.DecompressSpine,
                request.DecompressNeck,
                request.DecompressBack,
                request.EndocrineNerves,
                request.EndocrineDefenses,
                request.EndocrineHormones,
                request.CardiovascularReflexologyWith,
                request.DigestiveColonReflexologyWith,
                request.RespiratoryReflexologyWith,
                request.UrinaryReflexologyWithAcidFruits,
                request.OtherIndications,
                request.Observations,
                draftStatusId.Value,
                approvedStatusId.Value,
                request.UpdatedByUserId);
        }
        catch (DomainException exception)
        {
            var invalidStateMessages = new[]
            {
                "La hoja de tratamiento está inactiva.",
                "Solo se puede aprobar una hoja de tratamiento en estado borrador."
            };

            if (invalidStateMessages.Contains(exception.Message))
            {
                throw new ConflictException(exception.Message);
            }

            throw new AppValidationException("medicalIndication", exception.Message);
        }

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
            ApprovedAtUtc = treatmentSheet.ApprovedAtUtc,
            ApprovedByUserId = treatmentSheet.ApprovedByUserId,
            IsActive = treatmentSheet.IsActive,
            CreatedAtUtc = treatmentSheet.CreatedAtUtc,
            UpdatedAtUtc = treatmentSheet.UpdatedAtUtc
        };
    }
}
