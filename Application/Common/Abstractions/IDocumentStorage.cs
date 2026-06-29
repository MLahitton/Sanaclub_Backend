using Sanaclub.Application.Common.Models;

namespace Sanaclub.Application.Common.Abstractions;

public interface IDocumentStorage
{
    Task<StoredDocumentResult> SaveAsync(
        byte[] content,
        string storageObjectKey,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string storageObjectKey,
        CancellationToken cancellationToken = default);

    Task<byte[]> ReadAsync(
        string storageObjectKey,
        CancellationToken cancellationToken = default);
}
