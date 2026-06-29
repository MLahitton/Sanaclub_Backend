namespace Sanaclub.Application.Common.Models;

public sealed class StoredDocumentResult
{
    public string StorageProvider { get; init; } = "LOCAL";
    public string? StorageBucket { get; init; }
    public string StorageObjectKey { get; init; } = string.Empty;
    public string? StorageExternalId { get; init; }
    public string StoragePath { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
}
