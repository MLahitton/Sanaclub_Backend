namespace Sanaclub.Application.EvolutionSheets.Common;

public sealed class EvolutionSheetResponse
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public Guid TreatmentSheetId { get; init; }
    public Guid EvolutionStatusId { get; init; }
    public string? TherapyNumber { get; init; }
    public DateOnly? EvolutionDate { get; init; }
    public TimeOnly? EntryTime { get; init; }
    public TimeOnly? ExitTime { get; init; }
    public string? AssignedStaffName { get; init; }
    public string? TherapyName { get; init; }
    public string EvolutionNotes { get; init; } = string.Empty;
    public string? NewIndications { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public Guid? CompletedByUserId { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
