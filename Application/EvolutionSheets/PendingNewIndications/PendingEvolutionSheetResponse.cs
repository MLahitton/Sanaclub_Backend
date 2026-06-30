namespace Sanaclub.Application.EvolutionSheets.PendingNewIndications;

public sealed class PendingEvolutionSheetResponse
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public string PatientFullName { get; init; } = string.Empty;
    public string PatientIdentificationNumber { get; init; } = string.Empty;
    public Guid TreatmentSheetId { get; init; }
    public string? TreatmentNumber { get; init; }
    public string? TherapyNumber { get; init; }
    public DateOnly? EvolutionDate { get; init; }
    public TimeOnly? EntryTime { get; init; }
    public TimeOnly? ExitTime { get; init; }
    public string? AssignedStaffName { get; init; }
    public string? TherapyName { get; init; }
    public string EvolutionNotes { get; init; } = string.Empty;
    public string? NewIndications { get; init; }
    public Guid EvolutionStatusId { get; init; }
    public string StatusCode { get; init; } = string.Empty;
    public string StatusName { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
