using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.EvolutionSheets.Common;
using Sanaclub.Domain.EvolutionSheets;
using Sanaclub.Domain.Patients;

namespace Sanaclub.Application.EvolutionSheets.ListByPatient;

public sealed class ListEvolutionSheetsByPatientQuery : IRequest<IReadOnlyList<EvolutionSheetResponse>>
{
    public Guid PatientId { get; init; }
}

public sealed class ListEvolutionSheetsByPatientQueryHandler :
    IRequestHandler<ListEvolutionSheetsByPatientQuery, IReadOnlyList<EvolutionSheetResponse>>
{
    private readonly IEvolutionSheetRepository _evolutionSheetRepository;
    private readonly IPatientRepository _patientRepository;

    public ListEvolutionSheetsByPatientQueryHandler(
        IEvolutionSheetRepository evolutionSheetRepository,
        IPatientRepository patientRepository)
    {
        _evolutionSheetRepository = evolutionSheetRepository;
        _patientRepository = patientRepository;
    }

    public async Task<IReadOnlyList<EvolutionSheetResponse>> Handle(
        ListEvolutionSheetsByPatientQuery request,
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

        var evolutionSheets = await _evolutionSheetRepository.ListByPatientIdAsync(
            request.PatientId,
            cancellationToken);

        return evolutionSheets
            .Select(MapToResponse)
            .ToList()
            .AsReadOnly();
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
