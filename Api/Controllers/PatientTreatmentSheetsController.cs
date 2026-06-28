using System.Collections.Generic;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sanaclub.Api.Authorization.Permissions;
using Sanaclub.Api.Contracts.TreatmentSheets;
using Sanaclub.Application.TreatmentSheets.Common;
using Sanaclub.Application.TreatmentSheets.Create;
using Sanaclub.Application.TreatmentSheets.ListByPatient;

namespace Sanaclub.Api.Controllers;

[ApiController]
[Route("api/v1/patients/{patientId:guid}/treatment-sheets")]
[Authorize]
public sealed class PatientTreatmentSheetsController : ControllerBase
{
    private readonly ISender _sender;

    public PatientTreatmentSheetsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [RequirePermission("treatments.create")]
    [ProducesResponseType(typeof(TreatmentSheetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TreatmentSheetResponse>> Create(
        [FromRoute] Guid patientId,
        [FromBody] CreateTreatmentSheetRequest request,
        CancellationToken cancellationToken)
    {
        var createdByUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(createdByUserIdClaim, out var createdByUserId))
        {
            return Unauthorized();
        }

        var command = new CreateTreatmentSheetCommand
        {
            PatientId = patientId,
            TreatmentNumber = request.TreatmentNumber,
            ConsultationDate = request.ConsultationDate,
            EpsTreatingDoctorDiagnosis = request.EpsTreatingDoctorDiagnosis,
            ReferredClinicalHistory = request.ReferredClinicalHistory,
            CreatedByUserId = createdByUserId
        };

        var response = await _sender.Send(command, cancellationToken);

        return CreatedAtAction("GetById", "TreatmentSheets", new { id = response.Id }, response);
    }

    [HttpGet]
    [RequirePermission("treatments.read")]
    [ProducesResponseType(typeof(IReadOnlyCollection<TreatmentSheetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyCollection<TreatmentSheetResponse>>> ListByPatient(
        [FromRoute] Guid patientId,
        CancellationToken cancellationToken)
    {
        var query = new ListTreatmentSheetsByPatientQuery
        {
            PatientId = patientId
        };

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }
}

