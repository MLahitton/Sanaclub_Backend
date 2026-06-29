using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Models;
using Sanaclub.Application.Patients.Common;
using Sanaclub.Domain.Patients;

namespace Sanaclub.Application.Patients.List;

public sealed class ListPatientsQuery : IRequest<PaginatedResult<PatientResponse>>
{
    public string? Search { get; init; }
    public bool? IsActive { get; init; }
    public Guid? PatientStatusId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed class ListPatientsQueryHandler : IRequestHandler<ListPatientsQuery, PaginatedResult<PatientResponse>>
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;
    private const int MinPageSize = 1;
    private const int MinPageNumber = 1;

    private readonly IPatientRepository _patientRepository;

    public ListPatientsQueryHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PaginatedResult<PatientResponse>> Handle(ListPatientsQuery request, CancellationToken cancellationToken)
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
            request.IsActive,
            request.PatientStatusId,
            cancellationToken);
        var patients = await _patientRepository.ListAsync(
            normalizedSearch,
            request.IsActive,
            request.PatientStatusId,
            pageNumber,
            pageSize,
            cancellationToken);

        var responseItems = patients
            .Select(p => MapToResponse(p))
            .ToList();

        return new PaginatedResult<PatientResponse>(
            responseItems,
            pageNumber,
            pageSize,
            totalCount);
    }

    private static PatientResponse MapToResponse(Patient patient)
    {
        return new PatientResponse
        {
            Id = patient.Id,
            IdentificationTypeId = patient.IdentificationTypeId,
            IdentificationNumber = patient.IdentificationNumber,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            FullName = patient.FullName,
            BirthDate = patient.BirthDate,
            GenderId = patient.GenderId,
            CivilStatusId = patient.CivilStatusId,
            PhoneNumber = patient.PhoneNumber,
            Email = patient.Email,
            Address = patient.Address,
            CityOrMunicipality = patient.CityOrMunicipality,
            Occupation = patient.Occupation,
            EmergencyContactName = patient.EmergencyContactName,
            EmergencyContactRelationship = patient.EmergencyContactRelationship,
            EmergencyContactPhone = patient.EmergencyContactPhone,
            PatientStatusId = patient.PatientStatusId,
            IsActive = patient.IsActive,
            CreatedAtUtc = patient.CreatedAtUtc
        };
    }
}
