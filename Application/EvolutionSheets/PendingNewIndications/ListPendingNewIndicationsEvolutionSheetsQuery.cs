using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Common.Models;

namespace Sanaclub.Application.EvolutionSheets.PendingNewIndications;

public sealed class ListPendingNewIndicationsEvolutionSheetsQuery
    : IRequest<PaginatedResult<PendingEvolutionSheetResponse>>
{
    public string? Search { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed class ListPendingNewIndicationsEvolutionSheetsQueryHandler
    : IRequestHandler<ListPendingNewIndicationsEvolutionSheetsQuery, PaginatedResult<PendingEvolutionSheetResponse>>
{
    private const int MaxPageSize = 100;
    private const int MinPageSize = 1;
    private const int MinPageNumber = 1;
    private const string DraftStatusCode = "DRAFT";

    private readonly IEvolutionSheetRepository _evolutionSheetRepository;
    private readonly ICatalogRepository _catalogRepository;

    public ListPendingNewIndicationsEvolutionSheetsQueryHandler(
        IEvolutionSheetRepository evolutionSheetRepository,
        ICatalogRepository catalogRepository)
    {
        _evolutionSheetRepository = evolutionSheetRepository;
        _catalogRepository = catalogRepository;
    }

    public async Task<PaginatedResult<PendingEvolutionSheetResponse>> Handle(
        ListPendingNewIndicationsEvolutionSheetsQuery request,
        CancellationToken cancellationToken)
    {
        var draftStatusId = await _catalogRepository.GetEvolutionStatusIdByCodeAsync(
            DraftStatusCode,
            cancellationToken);

        if (draftStatusId is null)
        {
            throw new NotFoundException("El estado DRAFT de hojas de evolución no fue encontrado.");
        }

        var pageNumber = request.PageNumber < MinPageNumber
            ? MinPageNumber
            : request.PageNumber;

        var pageSize = request.PageSize < MinPageSize
            ? MinPageSize
            : request.PageSize;

        if (pageSize > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }

        var normalizedSearch = string.IsNullOrWhiteSpace(request.Search)
            ? null
            : request.Search.Trim();

        var totalCount = await _evolutionSheetRepository.CountPendingNewIndicationsAsync(
            draftStatusId.Value,
            normalizedSearch,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        var items = await _evolutionSheetRepository.ListPendingNewIndicationsAsync(
            draftStatusId.Value,
            normalizedSearch,
            request.FromDate,
            request.ToDate,
            pageNumber,
            pageSize,
            cancellationToken);

        return new PaginatedResult<PendingEvolutionSheetResponse>(
            items,
            pageNumber,
            pageSize,
            totalCount);
    }
}
