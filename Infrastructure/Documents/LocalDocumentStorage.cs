using Microsoft.Extensions.Options;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Models;
using System;
using System.IO;
using System.Threading;

namespace Sanaclub.Infrastructure.Documents;

public sealed class LocalDocumentStorage : IDocumentStorage
{
    private readonly string _basePath;

    public LocalDocumentStorage(IOptions<DocumentStorageOptions> options)
    {
        var basePath = options.Value.Local.BasePath;
        if (string.IsNullOrWhiteSpace(basePath))
        {
            basePath = LocalDocumentStorageOptions.GetDefaultBasePath();
        }

        _basePath = Path.GetFullPath(basePath);
    }

    public async Task<StoredDocumentResult> SaveAsync(
        byte[] content,
        string storageObjectKey,
        CancellationToken cancellationToken = default)
    {
        ValidateStorageObjectKey(storageObjectKey);

        var fullPath = GetFullPath(storageObjectKey);
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllBytesAsync(fullPath, content, cancellationToken);

        return new StoredDocumentResult
        {
            StorageProvider = "LOCAL",
            StorageBucket = null,
            StorageObjectKey = storageObjectKey,
            StorageExternalId = null,
            StoragePath = storageObjectKey,
            FileSizeBytes = new FileInfo(fullPath).Length
        };
    }

    public Task<bool> ExistsAsync(
        string storageObjectKey,
        CancellationToken cancellationToken = default)
    {
        ValidateStorageObjectKey(storageObjectKey);
        var fullPath = GetFullPath(storageObjectKey);
        return Task.FromResult(File.Exists(fullPath));
    }

    public async Task<byte[]> ReadAsync(
        string storageObjectKey,
        CancellationToken cancellationToken = default)
    {
        ValidateStorageObjectKey(storageObjectKey);
        var fullPath = GetFullPath(storageObjectKey);

        cancellationToken.ThrowIfCancellationRequested();
        return await File.ReadAllBytesAsync(fullPath, cancellationToken);
    }

    private static void ValidateStorageObjectKey(string storageObjectKey)
    {
        if (string.IsNullOrWhiteSpace(storageObjectKey))
        {
            throw new ArgumentException("La clave de almacenamiento es obligatoria.");
        }

        if (storageObjectKey.Contains("..", StringComparison.Ordinal))
        {
            throw new ArgumentException("La clave de almacenamiento no puede contener navegación de directorios.");
        }

        if (Path.IsPathRooted(storageObjectKey))
        {
            throw new ArgumentException("La clave de almacenamiento no puede ser una ruta absoluta.");
        }

        if (storageObjectKey.StartsWith("/", StringComparison.Ordinal) ||
            storageObjectKey.StartsWith("\\", StringComparison.Ordinal))
        {
            throw new ArgumentException("La clave de almacenamiento no puede ser una ruta absoluta.");
        }

        if (storageObjectKey.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            storageObjectKey.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("La clave de almacenamiento no puede ser una URL.");
        }
    }

    private string GetFullPath(string storageObjectKey)
    {
        var normalizedStorageObjectKey = storageObjectKey.Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(_basePath, normalizedStorageObjectKey));

        if (!fullPath.StartsWith(_basePath, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("La ruta de almacenamiento no es válida.");
        }

        return fullPath;
    }
}

