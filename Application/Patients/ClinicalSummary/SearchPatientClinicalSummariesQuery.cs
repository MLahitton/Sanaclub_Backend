using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Models;
using Sanaclub.Domain.Patients;
using System.Collections.Generic;
using System.Linq;

namespace Sanaclub.Application.Patients.ClinicalSummary;

public sealed class SearchPatientClinicalSummariesQuery : IRequest<PaginatedResult<PatientClinicalSummarySearchResultResponse>>
{
    public string? Search { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public sealed class SearchPatientClinicalSummariesQueryHandler :
    IRequestHandler<SearchPatientClinicalSummariesQuery, PaginatedResult<PatientClinicalSummarySearchResultResponse>>
{
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 50;
    private const int MinPageSize = 1;
    private const int MinPageNumber = 1;

    private readonly IPatientRepository _patientRepository;

    public SearchPatientClinicalSummariesQueryHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PaginatedResult<PatientClinicalSummarySearchResultResponse>> Handle(
        SearchPatientClinicalSummariesQuery request,
        CancellationToken cancellationToken)
    {
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

        var totalCount = await _patientRepository.CountAsync(
            normalizedSearch,
            isActive: null,
            patientStatusId: null,
            cancellationToken);

        var patients = await _patientRepository.SearchForClinicalSummaryAsync(
            normalizedSearch,
            pageNumber,
            pageSize,
            cancellationToken);

        var responseItems = patients
            .Select(MapToResponse)
            .ToList();

        return new PaginatedResult<PatientClinicalSummarySearchResultResponse>(
            responseItems,
            pageNumber,
            pageSize,
            totalCount);
    }

    private static PatientClinicalSummarySearchResultResponse MapToResponse(Patient patient)
    {
        return new PatientClinicalSummarySearchResultResponse
        {
            Id = patient.Id,
            IdentificationTypeId = patient.IdentificationTypeId,
            IdentificationNumber = patient.IdentificationNumber,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            FullName = patient.FullName,
            BirthDate = patient.BirthDate,
            PhoneNumber = patient.PhoneNumber,
            Email = patient.Email,
            CityOrMunicipality = patient.CityOrMunicipality,
            PatientStatusId = patient.PatientStatusId,
            IsActive = patient.IsActive,
            CreatedAtUtc = patient.CreatedAtUtc
        };
    }
}
