using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Documents.Common;
using Sanaclub.Domain.Documents;

namespace Sanaclub.Application.Documents.GetById;

public sealed class GetGeneratedDocumentByIdQuery : IRequest<GeneratedDocumentResponse>
{
    public Guid DocumentId { get; init; }
}

public sealed class GetGeneratedDocumentByIdQueryHandler :
    IRequestHandler<GetGeneratedDocumentByIdQuery, GeneratedDocumentResponse>
{
    private readonly IGeneratedDocumentRepository _generatedDocumentRepository;

    public GetGeneratedDocumentByIdQueryHandler(IGeneratedDocumentRepository generatedDocumentRepository)
    {
        _generatedDocumentRepository = generatedDocumentRepository;
    }

    public async Task<GeneratedDocumentResponse> Handle(
        GetGeneratedDocumentByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.DocumentId == Guid.Empty)
        {
            throw new AppValidationException(
                "documentId",
                "El identificador del documento es obligatorio.");
        }

        GeneratedDocument? document = await _generatedDocumentRepository.GetByIdAsync(
            request.DocumentId,
            cancellationToken);

        if (document is null)
        {
            throw new NotFoundException("El documento no fue encontrado.");
        }

        return MapToResponse(document);
    }

    private static GeneratedDocumentResponse MapToResponse(GeneratedDocument document)
    {
        return new GeneratedDocumentResponse
        {
            Id = document.Id,
            PatientId = document.PatientId,
            DocumentKind = document.DocumentKind,
            SourceEntityId = document.SourceEntityId,
            Title = document.Title,
            FileName = document.FileName,
            ContentType = document.ContentType,
            StorageProvider = document.StorageProvider,
            StorageBucket = document.StorageBucket,
            StorageObjectKey = document.StorageObjectKey,
            StorageExternalId = document.StorageExternalId,
            StoragePath = document.StoragePath,
            FileSizeBytes = document.FileSizeBytes,
            Status = document.Status,
            GeneratedAtUtc = document.GeneratedAtUtc,
            GeneratedByUserId = document.GeneratedByUserId,
            IsActive = document.IsActive,
            CreatedAtUtc = document.CreatedAtUtc,
            UpdatedAtUtc = document.UpdatedAtUtc
        };
    }
}
