using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.EvolutionSheets.Common;
using Sanaclub.Domain.EvolutionSheets;

namespace Sanaclub.Application.EvolutionSheets.GetById;

public sealed class GetEvolutionSheetByIdQuery : IRequest<EvolutionSheetResponse>
{
    public Guid EvolutionSheetId { get; init; }
}

public sealed class GetEvolutionSheetByIdQueryHandler :
    IRequestHandler<GetEvolutionSheetByIdQuery, EvolutionSheetResponse>
{
    private readonly IEvolutionSheetRepository _evolutionSheetRepository;

    public GetEvolutionSheetByIdQueryHandler(IEvolutionSheetRepository evolutionSheetRepository)
    {
        _evolutionSheetRepository = evolutionSheetRepository;
    }

    public async Task<EvolutionSheetResponse> Handle(
        GetEvolutionSheetByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.EvolutionSheetId == Guid.Empty)
        {
            throw new AppValidationException(
                "evolutionSheetId",
                "El identificador de la hoja de evolución es obligatorio.");
        }

        EvolutionSheet? evolutionSheet = await _evolutionSheetRepository.GetByIdAsync(
            request.EvolutionSheetId,
            cancellationToken);
        if (evolutionSheet is null)
        {
            throw new NotFoundException("La hoja de evolución no fue encontrada.");
        }

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
