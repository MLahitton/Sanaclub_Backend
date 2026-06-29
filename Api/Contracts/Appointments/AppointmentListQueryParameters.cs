namespace Sanaclub.Api.Contracts.Appointments;

public sealed class AppointmentListQueryParameters
{
    public DateOnly? Date { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public Guid? TherapistUserId { get; init; }
    public Guid? PatientId { get; init; }
    public string? ClinicalReferenceType { get; init; }
    public Guid? ClinicalReferenceId { get; init; }
    public string? Status { get; init; }
    public bool IncludeCancelled { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
