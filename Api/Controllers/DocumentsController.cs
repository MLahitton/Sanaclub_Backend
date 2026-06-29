using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sanaclub.Api.Authorization.Permissions;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Documents.Common;
using Sanaclub.Application.Documents.GetById;
using Sanaclub.Domain.Documents;

namespace Sanaclub.Api.Controllers;

[ApiController]
[Route("api/v1/documents")]
[Authorize]
public sealed class DocumentsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IGeneratedDocumentRepository _generatedDocumentRepository;
    private readonly IDocumentStorage _documentStorage;

    public DocumentsController(
        ISender sender,
        IGeneratedDocumentRepository generatedDocumentRepository,
        IDocumentStorage documentStorage)
    {
        _sender = sender;
        _generatedDocumentRepository = generatedDocumentRepository;
        _documentStorage = documentStorage;
    }

    [HttpGet("{id:guid}")]
    [RequirePermission("documents.read")]
    [ProducesResponseType(typeof(GeneratedDocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GeneratedDocumentResponse>> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetGeneratedDocumentByIdQuery
        {
            DocumentId = id
        };

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id:guid}/view")]
    [RequirePermission("documents.download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> View([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var document = await GetActiveDocumentOrThrowAsync(id, cancellationToken);

        var contentType = ResolveContentType(document.ContentType);
        var storageObjectKey = GetStorageObjectKey(document);
        if (!await _documentStorage.ExistsAsync(storageObjectKey, cancellationToken))
        {
            throw new NotFoundException("El archivo del documento no fue encontrado.");
        }

        var fileContent = await _documentStorage.ReadAsync(storageObjectKey, cancellationToken);

        return File(fileContent, contentType);
    }

    [HttpGet("{id:guid}/download")]
    [RequirePermission("documents.download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Download([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var document = await GetActiveDocumentOrThrowAsync(id, cancellationToken);

        var contentType = ResolveContentType(document.ContentType);
        var storageObjectKey = GetStorageObjectKey(document);
        if (!await _documentStorage.ExistsAsync(storageObjectKey, cancellationToken))
        {
            throw new NotFoundException("El archivo del documento no fue encontrado.");
        }

        var fileContent = await _documentStorage.ReadAsync(storageObjectKey, cancellationToken);

        return File(fileContent, contentType, document.FileName);
    }

    private async Task<GeneratedDocument> GetActiveDocumentOrThrowAsync(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        if (documentId == Guid.Empty)
        {
            throw new AppValidationException("id", "El identificador del documento es obligatorio.");
        }

        var document = await _generatedDocumentRepository.GetByIdAsync(documentId, cancellationToken);
        if (document is null)
        {
            throw new NotFoundException("El documento no fue encontrado.");
        }

        if (!document.IsActive)
        {
            throw new NotFoundException("El documento no está disponible.");
        }

        return document;
    }

    private static string ResolveContentType(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return "application/pdf";
        }

        if (!contentType.Contains("/", StringComparison.OrdinalIgnoreCase))
        {
            return "application/pdf";
        }

        return contentType;
    }

    private static string GetStorageObjectKey(GeneratedDocument document)
    {
        var storageObjectKey = string.IsNullOrWhiteSpace(document.StorageObjectKey)
            ? document.StoragePath
            : document.StorageObjectKey;

        if (string.IsNullOrWhiteSpace(storageObjectKey))
        {
            throw new NotFoundException("La ruta de almacenamiento del documento no está configurada.");
        }

        return storageObjectKey;
    }
}
