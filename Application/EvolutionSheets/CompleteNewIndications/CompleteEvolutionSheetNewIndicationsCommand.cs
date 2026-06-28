using System.Linq;
using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.EvolutionSheets.Common;
using Sanaclub.Domain.Common;
using Sanaclub.Domain.EvolutionSheets;

namespace Sanaclub.Application.EvolutionSheets.CompleteNewIndications;

public sealed class CompleteEvolutionSheetNewIndicationsCommand : IRequest<EvolutionSheetResponse>
{
    public Guid EvolutionSheetId { get; init; }
    public string NewIndications { get; init; } = string.Empty;
    public Guid CompletedByUserId { get; init; }
}

public sealed class CompleteEvolutionSheetNewIndicationsCommandHandler :
    IRequestHandler<CompleteEvolutionSheetNewIndicationsCommand, EvolutionSheetResponse>
{
    private readonly IEvolutionSheetRepository _evolutionSheetRepository;
    private readonly ICatalogRepository _catalogRepository;

    public CompleteEvolutionSheetNewIndicationsCommandHandler(
        IEvolutionSheetRepository evolutionSheetRepository,
        ICatalogRepository catalogRepository)
    {
        _evolutionSheetRepository = evolutionSheetRepository;
        _catalogRepository = catalogRepository;
    }

    public async Task<EvolutionSheetResponse> Handle(
        CompleteEvolutionSheetNewIndicationsCommand request,
        CancellationToken cancellationToken)
    {
        if (request.EvolutionSheetId == Guid.Empty)
        {
            throw new AppValidationException(
                "evolutionSheetId",
                "El identificador de la hoja de evolución es obligatorio.");
        }

        if (request.CompletedByUserId == Guid.Empty)
        {
            throw new AppValidationException(
                "completedByUserId",
                "El identificador del usuario aprobador es obligatorio.");
        }

        var evolutionSheet = await _evolutionSheetRepository.GetByIdForUpdateAsync(
            request.EvolutionSheetId,
            cancellationToken);
        if (evolutionSheet is null)
        {
            throw new NotFoundException("La hoja de evolución no fue encontrada.");
        }

        var draftStatusId = await _catalogRepository.GetEvolutionStatusIdByCodeAsync(
            "DRAFT",
            cancellationToken);
        if (!draftStatusId.HasValue)
        {
            throw new ConflictException("El estado DRAFT de evolución no está configurado.");
        }

        var completedStatusId = await _catalogRepository.GetEvolutionStatusIdByCodeAsync(
            "COMPLETED",
            cancellationToken);
        if (!completedStatusId.HasValue)
        {
            throw new ConflictException("El estado COMPLETED de evolución no está configurado.");
        }

        try
        {
            evolutionSheet.CompleteNewIndications(
                draftStatusId.Value,
                completedStatusId.Value,
                request.CompletedByUserId,
                request.NewIndications);
        }
        catch (DomainException exception)
        {
            var invalidStateMessages = new[]
            {
                "La evolución debe estar activa.",
                "Solo se puede completar una evolución en estado borrador."
            };

            if (invalidStateMessages.Contains(exception.Message))
            {
                throw new ConflictException(exception.Message);
            }

            throw new AppValidationException("newIndications", exception.Message);
        }

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
