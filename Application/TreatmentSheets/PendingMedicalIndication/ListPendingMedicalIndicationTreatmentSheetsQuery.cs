using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Common.Models;

namespace Sanaclub.Application.TreatmentSheets.PendingMedicalIndication;

public sealed class ListPendingMedicalIndicationTreatmentSheetsQuery
    : IRequest<PaginatedResult<PendingTreatmentSheetResponse>>
{
    public string? Search { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed class ListPendingMedicalIndicationTreatmentSheetsQueryHandler
    : IRequestHandler<ListPendingMedicalIndicationTreatmentSheetsQuery, PaginatedResult<PendingTreatmentSheetResponse>>
{
    private const int MaxPageSize = 100;
    private const int MinPageSize = 1;
    private const int MinPageNumber = 1;
    private const string DraftStatusCode = "DRAFT";

    private readonly ITreatmentSheetRepository _treatmentSheetRepository;
    private readonly ICatalogRepository _catalogRepository;

    public ListPendingMedicalIndicationTreatmentSheetsQueryHandler(
        ITreatmentSheetRepository treatmentSheetRepository,
        ICatalogRepository catalogRepository)
    {
        _treatmentSheetRepository = treatmentSheetRepository;
        _catalogRepository = catalogRepository;
    }

    public async Task<PaginatedResult<PendingTreatmentSheetResponse>> Handle(
        ListPendingMedicalIndicationTreatmentSheetsQuery request,
        CancellationToken cancellationToken)
    {
        var draftStatusId = await _catalogRepository.GetTreatmentStatusIdByCodeAsync(
            DraftStatusCode,
            cancellationToken);

        if (draftStatusId is null)
        {
            throw new NotFoundException("El estado DRAFT de hojas de tratamiento no fue encontrado.");
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

        var totalCount = await _treatmentSheetRepository.CountPendingMedicalIndicationAsync(
            draftStatusId.Value,
            normalizedSearch,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        var items = await _treatmentSheetRepository.ListPendingMedicalIndicationAsync(
            draftStatusId.Value,
            normalizedSearch,
            request.FromDate,
            request.ToDate,
            pageNumber,
            pageSize,
            cancellationToken);

        return new PaginatedResult<PendingTreatmentSheetResponse>(
            items,
            pageNumber,
            pageSize,
            totalCount);
    }
}
