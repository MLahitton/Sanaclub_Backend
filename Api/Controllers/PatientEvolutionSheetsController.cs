using System.Collections.Generic;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sanaclub.Api.Authorization.Permissions;
using Sanaclub.Api.Contracts.EvolutionSheets;
using Sanaclub.Application.EvolutionSheets.Common;
using Sanaclub.Application.EvolutionSheets.Create;
using Sanaclub.Application.EvolutionSheets.ListByPatient;

namespace Sanaclub.Api.Controllers;

[ApiController]
[Route("api/v1/patients/{patientId:guid}/evolution-sheets")]
[Authorize]
public sealed class PatientEvolutionSheetsController : ControllerBase
{
    private readonly ISender _sender;

    public PatientEvolutionSheetsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [RequirePermission("evolutions.create")]
    [ProducesResponseType(typeof(EvolutionSheetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EvolutionSheetResponse>> Create(
        [FromRoute] Guid patientId,
        [FromBody] CreateEvolutionSheetRequest request,
        CancellationToken cancellationToken)
    {
        var createdByUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(createdByUserIdClaim, out var createdByUserId))
        {
            return Unauthorized();
        }

        var command = new CreateEvolutionSheetCommand
        {
            PatientId = patientId,
            TreatmentSheetId = request.TreatmentSheetId,
            TherapyNumber = request.TherapyNumber,
            EvolutionDate = request.EvolutionDate,
            EntryTime = request.EntryTime,
            ExitTime = request.ExitTime,
            AssignedStaffName = request.AssignedStaffName,
            TherapyName = request.TherapyName,
            EvolutionNotes = request.EvolutionNotes,
            CreatedByUserId = createdByUserId
        };

        var response = await _sender.Send(command, cancellationToken);

        return CreatedAtAction("GetById", "EvolutionSheets", new { id = response.Id }, response);
    }

    [HttpGet]
    [RequirePermission("evolutions.read")]
    [ProducesResponseType(typeof(IReadOnlyCollection<EvolutionSheetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyCollection<EvolutionSheetResponse>>> ListByPatient(
        [FromRoute] Guid patientId,
        CancellationToken cancellationToken)
    {
        var query = new ListEvolutionSheetsByPatientQuery
        {
            PatientId = patientId
        };

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }
}
