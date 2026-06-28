using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sanaclub.Api.Authorization.Permissions;
using Sanaclub.Application.Common.Models;
using Sanaclub.Application.Patients.ClinicalSummary;

namespace Sanaclub.Api.Controllers;

[ApiController]
[Route("api/v1/patients")]
[Authorize]
public sealed class PatientClinicalSummaryController : ControllerBase
{
    private readonly ISender _sender;

    public PatientClinicalSummaryController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("clinical-summary/search")]
    [RequirePermission("patients.read")]
    [ProducesResponseType(typeof(PaginatedResult<PatientClinicalSummarySearchResultResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<PatientClinicalSummarySearchResultResponse>>> Search(
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchPatientClinicalSummariesQuery
        {
            Search = search,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }

    [HttpGet("{patientId:guid}/clinical-summary")]
    [RequirePermission("patients.read")]
    [ProducesResponseType(typeof(PatientClinicalSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PatientClinicalSummaryResponse>> GetClinicalSummary(
        [FromRoute] Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPatientClinicalSummaryQuery
        {
            PatientId = patientId
        };

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }
}
