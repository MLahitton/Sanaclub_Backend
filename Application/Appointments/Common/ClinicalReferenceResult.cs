namespace Sanaclub.Application.Appointments.Common;

public sealed class ClinicalReferenceResult
{
    public Guid TreatmentSheetId { get; init; }
    public string ClinicalReferenceType { get; init; } = string.Empty;
    public Guid ClinicalReferenceId { get; init; }
}
