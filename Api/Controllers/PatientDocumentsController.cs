using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sanaclub.Api.Authorization.Permissions;
using Sanaclub.Application.Documents.Common;
using Sanaclub.Application.Documents.ListByPatient;

namespace Sanaclub.Api.Controllers;

[ApiController]
[Route("api/v1/patients/{patientId:guid}/documents")]
[Authorize]
public sealed class PatientDocumentsController : ControllerBase
{
    private readonly ISender _sender;

    public PatientDocumentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [RequirePermission("documents.read")]
    [ProducesResponseType(typeof(IReadOnlyList<GeneratedDocumentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<GeneratedDocumentResponse>>> ListByPatient(
        [FromRoute] Guid patientId,
        CancellationToken cancellationToken)
    {
        var query = new ListGeneratedDocumentsByPatientQuery
        {
            PatientId = patientId
        };

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }
}
