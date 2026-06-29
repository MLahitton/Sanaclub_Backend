using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sanaclub.Api.Authorization.Permissions;
using Sanaclub.Application.Documents.Common;
using Sanaclub.Application.Documents.GenerateInformedConsentPdf;

namespace Sanaclub.Api.Controllers;

[ApiController]
[Route("api/v1/consents/{consentId:guid}/documents")]
[Authorize]
public sealed class ConsentDocumentsController : ControllerBase
{
    private readonly ISender _sender;

    public ConsentDocumentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("pdf")]
    [RequirePermission("documents.generate")]
    [ProducesResponseType(typeof(GeneratedDocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GeneratedDocumentResponse>> GeneratePdf(
        [FromRoute] Guid consentId,
        CancellationToken cancellationToken)
    {
        var generatedByUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(generatedByUserIdClaim, out var generatedByUserId))
        {
            return Unauthorized();
        }

        var command = new GenerateInformedConsentPdfCommand
        {
            ConsentId = consentId,
            GeneratedByUserId = generatedByUserId
        };

        var response = await _sender.Send(command, cancellationToken);

        return Ok(response);
    }
}
