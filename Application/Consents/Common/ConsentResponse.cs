namespace Sanaclub.Application.Consents.Common;

public sealed class ConsentResponse
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public Guid DocumentTypeId { get; init; }
    public Guid ConsentStatusId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime? SignedAtUtc { get; init; }
    public Guid? SignedByUserId { get; init; }
    public string? PatientSignerName { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

