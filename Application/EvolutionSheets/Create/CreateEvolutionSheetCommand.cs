using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.EvolutionSheets.Common;
using Sanaclub.Domain.EvolutionSheets;
using Sanaclub.Domain.Patients;
using Sanaclub.Domain.TreatmentSheets;

namespace Sanaclub.Application.EvolutionSheets.Create;

public sealed class CreateEvolutionSheetCommand : IRequest<EvolutionSheetResponse>
{
    public Guid PatientId { get; init; }
    public Guid TreatmentSheetId { get; init; }
    public string? TherapyNumber { get; init; }
    public DateOnly? EvolutionDate { get; init; }
    public TimeOnly? EntryTime { get; init; }
    public TimeOnly? ExitTime { get; init; }
    public string? AssignedStaffName { get; init; }
    public string? TherapyName { get; init; }
    public string EvolutionNotes { get; init; } = string.Empty;
    public Guid CreatedByUserId { get; init; }
}

public sealed class CreateEvolutionSheetCommandHandler :
    IRequestHandler<CreateEvolutionSheetCommand, EvolutionSheetResponse>
{
    private readonly IEvolutionSheetRepository _evolutionSheetRepository;
    private readonly ITreatmentSheetRepository _treatmentSheetRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly ICatalogRepository _catalogRepository;

    public CreateEvolutionSheetCommandHandler(
        IEvolutionSheetRepository evolutionSheetRepository,
        ITreatmentSheetRepository treatmentSheetRepository,
        IPatientRepository patientRepository,
        ICatalogRepository catalogRepository)
    {
        _evolutionSheetRepository = evolutionSheetRepository;
        _treatmentSheetRepository = treatmentSheetRepository;
        _patientRepository = patientRepository;
        _catalogRepository = catalogRepository;
    }

    public async Task<EvolutionSheetResponse> Handle(
        CreateEvolutionSheetCommand request,
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
                "El identificador del usuario que crea la hoja es obligatorio.");
        }

        if (request.TreatmentSheetId == Guid.Empty)
        {
            throw new AppValidationException(
                "treatmentSheetId",
                "La hoja de tratamiento es obligatoria para crear una evoluciÃ³n.");
        }

        Patient? patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("El paciente no fue encontrado.");
        }

        TreatmentSheet? treatmentSheet = await _treatmentSheetRepository.GetByIdAsync(
            request.TreatmentSheetId,
            cancellationToken);
        if (treatmentSheet is null)
        {
            throw new NotFoundException("La hoja de tratamiento no fue encontrada.");
        }

        if (treatmentSheet.PatientId != request.PatientId)
        {
            throw new ConflictException("La hoja de tratamiento no pertenece al paciente indicado.");
        }

        var approvedTreatmentStatusId = await _catalogRepository.GetTreatmentStatusIdByCodeAsync(
            "APPROVED",
            cancellationToken);
        if (!approvedTreatmentStatusId.HasValue)
        {
            throw new ConflictException("El estado APPROVED de tratamiento no estÃ¡ configurado.");
        }

        if (treatmentSheet.TreatmentStatusId != approvedTreatmentStatusId.Value)
        {
            throw new ConflictException("Solo se pueden crear evoluciones sobre hojas de tratamiento aprobadas.");
        }

        var evolutionStatusId = await _catalogRepository.GetEvolutionStatusIdByCodeAsync(
            "DRAFT",
            cancellationToken);
        if (!evolutionStatusId.HasValue)
        {
            throw new ConflictException("El estado DRAFT de evolución no está configurado.");
        }

        var evolutionSheet = new EvolutionSheet(
            request.PatientId,
            request.TreatmentSheetId,
            evolutionStatusId.Value,
            request.TherapyNumber,
            request.EvolutionDate,
            request.EntryTime,
            request.ExitTime,
            request.AssignedStaffName,
            request.TherapyName,
            request.EvolutionNotes);

        evolutionSheet.MarkAsCreated(request.CreatedByUserId);

        await _evolutionSheetRepository.AddAsync(evolutionSheet, cancellationToken);
        await _evolutionSheetRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(evolutionSheet);
    }

    private static EvolutionSheetResponse MapToResponse(EvolutionSheet evolutionSheet)
    {
        return new EvolutionSheetResponse
        {
            Id = evolutionSheet.Id,
            PatientId = evolutionSheet.PatientId,
            TreatmentSheetId = evolutionSheet.TreatmentSheetId,
            EvolutionStatusId = evolutionSheet.EvolutionStatusId,
            TherapyNumber = evolutionSheet.TherapyNumber,
            EvolutionDate = evolutionSheet.EvolutionDate,
            EntryTime = evolutionSheet.EntryTime,
            ExitTime = evolutionSheet.ExitTime,
            AssignedStaffName = evolutionSheet.AssignedStaffName,
            TherapyName = evolutionSheet.TherapyName,
            EvolutionNotes = evolutionSheet.EvolutionNotes,
            NewIndications = evolutionSheet.NewIndications,
            CompletedAtUtc = evolutionSheet.CompletedAtUtc,
            CompletedByUserId = evolutionSheet.CompletedByUserId,
            IsActive = evolutionSheet.IsActive,
            CreatedAtUtc = evolutionSheet.CreatedAtUtc,
            UpdatedAtUtc = evolutionSheet.UpdatedAtUtc
        };
    }
}
