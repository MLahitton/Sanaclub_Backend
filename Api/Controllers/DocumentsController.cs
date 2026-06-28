using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sanaclub.Api.Authorization.Permissions;
using Sanaclub.Application.Documents.Common;
using Sanaclub.Application.Documents.GetById;

namespace Sanaclub.Api.Controllers;

[ApiController]
[Route("api/v1/documents")]
[Authorize]
public sealed class DocumentsController : ControllerBase
{
    private readonly ISender _sender;

    public DocumentsController(ISender sender)
    {
        _sender = sender;
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
}
