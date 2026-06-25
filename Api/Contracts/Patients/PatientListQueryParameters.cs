namespace Sanaclub.Api.Contracts.Patients;

public sealed class PatientListQueryParameters
{
    public string? Search { get; init; }
    public bool? IsActive { get; init; }
    public Guid? PatientStatusId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
