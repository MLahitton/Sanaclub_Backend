using Sanaclub.Domain.Common;

namespace Sanaclub.Domain.Documents;

public sealed class GeneratedDocument : AuditableEntity
{
    public Guid PatientId { get; private set; }
    public string DocumentKind { get; private set; } = string.Empty;
    public Guid SourceEntityId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public string StoragePath { get; private set; } = string.Empty;
    public long FileSizeBytes { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public DateTime GeneratedAtUtc { get; private set; }
    public Guid GeneratedByUserId { get; private set; }
    public bool IsActive { get; private set; }

    public const string StatusGenerated = "GENERATED";

    private const int MaxDocumentKindLength = 80;
    private const int MaxTitleLength = 200;
    private const int MaxFileNameLength = 255;
    private const int MaxContentTypeLength = 100;
    private const int MaxStoragePathLength = 1000;
    private const int MaxStatusLength = 50;

    private GeneratedDocument()
    {
    }

    public GeneratedDocument(
        Guid patientId,
        string documentKind,
        Guid sourceEntityId,
        string title,
        string fileName,
        string contentType,
        string storagePath,
        long fileSizeBytes,
        Guid generatedByUserId)
    {
        if (patientId == Guid.Empty)
        {
            throw new DomainException("El paciente es obligatorio.");
        }

        var trimmedDocumentKind = string.IsNullOrWhiteSpace(documentKind)
            ? string.Empty
            : documentKind.Trim();
        if (string.IsNullOrWhiteSpace(trimmedDocumentKind))
        {
            throw new DomainException("El tipo de documento es obligatorio.");
        }

        if (trimmedDocumentKind.Length > MaxDocumentKindLength)
        {
            throw new DomainException("El tipo de documento no puede superar los 80 caracteres.");
        }

        if (sourceEntityId == Guid.Empty)
        {
            throw new DomainException("El origen del documento es obligatorio.");
        }

        var trimmedTitle = string.IsNullOrWhiteSpace(title)
            ? string.Empty
            : title.Trim();
        if (string.IsNullOrWhiteSpace(trimmedTitle))
        {
            throw new DomainException("El titulo del documento es obligatorio.");
        }

        if (trimmedTitle.Length > MaxTitleLength)
        {
            throw new DomainException("El titulo del documento no puede superar los 200 caracteres.");
        }

        var trimmedFileName = string.IsNullOrWhiteSpace(fileName)
            ? string.Empty
            : fileName.Trim();
        if (string.IsNullOrWhiteSpace(trimmedFileName))
        {
            throw new DomainException("El nombre del archivo es obligatorio.");
        }

        if (trimmedFileName.Length > MaxFileNameLength)
        {
            throw new DomainException("El nombre del archivo no puede superar los 255 caracteres.");
        }

        var trimmedContentType = string.IsNullOrWhiteSpace(contentType)
            ? string.Empty
            : contentType.Trim();
        if (string.IsNullOrWhiteSpace(trimmedContentType))
        {
            throw new DomainException("El tipo de contenido es obligatorio.");
        }

        if (trimmedContentType.Length > MaxContentTypeLength)
        {
            throw new DomainException("El tipo de contenido no puede superar los 100 caracteres.");
        }

        var trimmedStoragePath = string.IsNullOrWhiteSpace(storagePath)
            ? string.Empty
            : storagePath.Trim();
        if (string.IsNullOrWhiteSpace(trimmedStoragePath))
        {
            throw new DomainException("La ruta de almacenamiento es obligatoria.");
        }

        if (trimmedStoragePath.Length > MaxStoragePathLength)
        {
            throw new DomainException("La ruta de almacenamiento no puede superar los 1000 caracteres.");
        }

        if (fileSizeBytes < 0)
        {
            throw new DomainException("El tamanio de archivo no puede ser negativo.");
        }

        if (generatedByUserId == Guid.Empty)
        {
            throw new DomainException("El usuario generador es obligatorio.");
        }

        PatientId = patientId;
        DocumentKind = trimmedDocumentKind;
        SourceEntityId = sourceEntityId;
        Title = trimmedTitle;
        FileName = trimmedFileName;
        ContentType = trimmedContentType;
        StoragePath = trimmedStoragePath;
        FileSizeBytes = fileSizeBytes;
        Status = StatusGenerated;
        GeneratedAtUtc = DateTime.UtcNow;
        GeneratedByUserId = generatedByUserId;
        IsActive = true;
    }
}
