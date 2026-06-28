using System;

namespace Sanaclub.Application.Documents.Common;

public sealed class GeneratedDocumentResponse
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public string DocumentKind { get; init; } = string.Empty;
    public Guid SourceEntityId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public string StoragePath { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime GeneratedAtUtc { get; init; }
    public Guid GeneratedByUserId { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
