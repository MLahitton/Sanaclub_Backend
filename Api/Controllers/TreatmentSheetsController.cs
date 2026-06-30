using System.Collections.Generic;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sanaclub.Api.Authorization.Permissions;
using Sanaclub.Api.Contracts.TreatmentSheets;
using Sanaclub.Application.Common.Models;
using Sanaclub.Application.TreatmentSheets.Common;
using Sanaclub.Application.TreatmentSheets.GetById;
using Sanaclub.Application.TreatmentSheets.PendingMedicalIndication;
using Sanaclub.Application.TreatmentSheets.UpdateMedicalIndication;

namespace Sanaclub.Api.Controllers;

[ApiController]
[Route("api/v1/treatment-sheets")]
[Authorize]
public sealed class TreatmentSheetsController : ControllerBase
{
    private readonly ISender _sender;

    public TreatmentSheetsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id:guid}")]
    [RequirePermission("treatments.read")]
    [ProducesResponseType(typeof(TreatmentSheetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TreatmentSheetResponse>> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetTreatmentSheetByIdQuery
        {
            TreatmentSheetId = id
        };

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }

    [HttpGet("pending-medical-indication")]
    [RequirePermission("treatments.update_medical_indication")]
    [ProducesResponseType(typeof(PaginatedResult<PendingTreatmentSheetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<PendingTreatmentSheetResponse>>> ListPendingMedicalIndication(
        [FromQuery] string? search,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new ListPendingMedicalIndicationTreatmentSheetsQuery
        {
            Search = search,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }

    [HttpPut("{id:guid}/medical-indication")]
    [RequirePermission("treatments.update_medical_indication")]
    [ProducesResponseType(typeof(TreatmentSheetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TreatmentSheetResponse>> UpdateMedicalIndication(
        [FromRoute] Guid id,
        [FromBody] UpdateTreatmentSheetMedicalIndicationRequest request,
        CancellationToken cancellationToken)
    {
        var updatedByUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(updatedByUserIdClaim, out var updatedByUserId))
        {
            return Unauthorized();
        }

        var command = new UpdateTreatmentSheetMedicalIndicationCommand
        {
            TreatmentSheetId = id,
            IndicationDate = request.IndicationDate,
            EntryTime = request.EntryTime,
            ExitTime = request.ExitTime,
            AssignedStaffName = request.AssignedStaffName,
            TherapyName = request.TherapyName,
            NervousSystemIndications = request.NervousSystemIndications,
            DecompressSpine = request.DecompressSpine,
            DecompressNeck = request.DecompressNeck,
            DecompressBack = request.DecompressBack,
            EndocrineNerves = request.EndocrineNerves,
            EndocrineDefenses = request.EndocrineDefenses,
            EndocrineHormones = request.EndocrineHormones,
            CardiovascularReflexologyWith = request.CardiovascularReflexologyWith,
            DigestiveColonReflexologyWith = request.DigestiveColonReflexologyWith,
            RespiratoryReflexologyWith = request.RespiratoryReflexologyWith,
            UrinaryReflexologyWithAcidFruits = request.UrinaryReflexologyWithAcidFruits,
            OtherIndications = request.OtherIndications,
            Observations = request.Observations,
            UpdatedByUserId = updatedByUserId
        };

        var response = await _sender.Send(command, cancellationToken);

        return Ok(response);
    }
}
