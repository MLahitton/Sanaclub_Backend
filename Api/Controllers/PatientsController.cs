using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sanaclub.Api.Authorization.Permissions;
using Sanaclub.Api.Contracts.Patients;
using Sanaclub.Application.Common.Models;
using Sanaclub.Application.Patients.Common;
using Sanaclub.Application.Patients.Create;
using Sanaclub.Application.Patients.GetById;
using Sanaclub.Application.Patients.List;

namespace Sanaclub.Api.Controllers;

[ApiController]
[Route("api/v1/patients")]
[Authorize]
public sealed class PatientsController : ControllerBase
{
    private readonly ISender _sender;

    public PatientsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [RequirePermission("patients.create")]
    [ProducesResponseType(typeof(PatientResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PatientResponse>> Create(
        [FromBody] CreatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var createdByUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(createdByUserIdClaim, out var createdByUserId))
        {
            return Unauthorized();
        }

        var command = new CreatePatientCommand
        {
            IdentificationTypeId = request.IdentificationTypeId,
            IdentificationNumber = request.IdentificationNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            GenderId = request.GenderId,
            CivilStatusId = request.CivilStatusId,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Address = request.Address,
            PatientStatusId = request.PatientStatusId,
            CreatedByUserId = createdByUserId
        };

        var response = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission("patients.read")]
    [ProducesResponseType(typeof(PatientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PatientResponse>> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetPatientByIdQuery
        {
            PatientId = id
        };

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }

    [HttpGet]
    [RequirePermission("patients.read")]
    [ProducesResponseType(typeof(PaginatedResult<PatientResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<PatientResponse>>> List(
        [FromQuery] PatientListQueryParameters queryParameters,
        CancellationToken cancellationToken)
    {
        var query = new ListPatientsQuery
        {
            Search = queryParameters.Search,
            PageNumber = queryParameters.PageNumber,
            PageSize = queryParameters.PageSize
        };

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }
}
