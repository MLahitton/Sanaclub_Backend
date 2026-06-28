using System;

namespace Sanaclub.Application.Patients.ClinicalSummary;

public sealed class PatientClinicalSummarySearchResultResponse
{
    public Guid Id { get; init; }
    public Guid IdentificationTypeId { get; init; }
    public string? IdentificationTypeCode { get; init; }
    public string? IdentificationTypeName { get; init; }
    public string IdentificationNumber { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public DateOnly? BirthDate { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? CityOrMunicipality { get; init; }
    public Guid PatientStatusId { get; init; }
    public string? PatientStatusCode { get; init; }
    public string? PatientStatusName { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
