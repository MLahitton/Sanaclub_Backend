using System;

namespace Sanaclub.Api.Contracts.EvolutionSheets;

public sealed class CreateEvolutionSheetRequest
{
    public Guid TreatmentSheetId { get; init; }
    public string? TherapyNumber { get; init; }
    public DateOnly? EvolutionDate { get; init; }
    public TimeOnly? EntryTime { get; init; }
    public TimeOnly? ExitTime { get; init; }
    public string? AssignedStaffName { get; init; }
    public string? TherapyName { get; init; }
    public string EvolutionNotes { get; init; } = string.Empty;
}
