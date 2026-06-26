using System;

namespace Sanaclub.Application.Patients.Common;

public sealed class PatientResponse
{
    public Guid Id { get; init; }
    public Guid IdentificationTypeId { get; init; }
    public string IdentificationNumber { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public DateOnly? BirthDate { get; init; }
    public Guid? GenderId { get; init; }
    public Guid? CivilStatusId { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public string? CityOrMunicipality { get; init; }
    public string? Occupation { get; init; }
    public string? EmergencyContactName { get; init; }
    public string? EmergencyContactRelationship { get; init; }
    public string? EmergencyContactPhone { get; init; }
    public Guid PatientStatusId { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
