using System.Collections.Generic;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sanaclub.Api.Authorization.Permissions;
using Sanaclub.Api.Contracts.Consents;
using Sanaclub.Application.Consents.Common;
using Sanaclub.Application.Consents.Create;
using Sanaclub.Application.Consents.GetById;
using Sanaclub.Application.Consents.ListByPatient;

namespace Sanaclub.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Authorize]
public sealed class ConsentsController : ControllerBase
{
    private readonly ISender _sender;

    public ConsentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("patients/{patientId:guid}/consents")]
    [RequirePermission("consents.create")]
    [ProducesResponseType(typeof(ConsentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ConsentResponse>> Create(
        [FromRoute] Guid patientId,
        [FromBody] CreateConsentRequest request,
        CancellationToken cancellationToken)
    {
        var createdByUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(createdByUserIdClaim, out var createdByUserId))
        {
            return Unauthorized();
        }

        var command = new CreateConsentCommand
        {
            PatientId = patientId,
            DocumentTypeId = request.DocumentTypeId,
            ConsentStatusId = request.ConsentStatusId,
            Title = request.Title,
            Description = request.Description,
            PatientSignerName = request.PatientSignerName,
            Notes = request.Notes,
            CreatedByUserId = createdByUserId
        };

        var response = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("patients/{patientId:guid}/consents")]
    [RequirePermission("consents.read")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ConsentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyCollection<ConsentResponse>>> ListByPatient(
        [FromRoute] Guid patientId,
        CancellationToken cancellationToken)
    {
        var query = new ListPatientConsentsQuery
        {
            PatientId = patientId
        };

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }

    [HttpGet("consents/{id:guid}")]
    [RequirePermission("consents.read")]
    [ProducesResponseType(typeof(ConsentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ConsentResponse>> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetConsentByIdQuery
        {
            ConsentId = id
        };

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }
}
