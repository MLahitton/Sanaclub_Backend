using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sanaclub.Api.Authorization.Permissions;
using Sanaclub.Api.Contracts.Patients;
using Sanaclub.Application.Common.Models;
using Sanaclub.Application.Patients.Common;
using Sanaclub.Application.Patients.Create;
using Sanaclub.Application.Patients.Archive;
using Sanaclub.Application.Patients.GetById;
using Sanaclub.Application.Patients.List;
using Sanaclub.Application.Patients.Update;

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
            CityOrMunicipality = request.CityOrMunicipality,
            Occupation = request.Occupation,
            EmergencyContactName = request.EmergencyContactName,
            EmergencyContactRelationship = request.EmergencyContactRelationship,
            EmergencyContactPhone = request.EmergencyContactPhone,
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

    [HttpPut("{id:guid}")]
    [RequirePermission("patients.update")]
    [ProducesResponseType(typeof(PatientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PatientResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var updatedByUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(updatedByUserIdClaim, out var updatedByUserId))
        {
            return Unauthorized();
        }

        var command = new UpdatePatientCommand
        {
            PatientId = id,
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
            CityOrMunicipality = request.CityOrMunicipality,
            Occupation = request.Occupation,
            EmergencyContactName = request.EmergencyContactName,
            EmergencyContactRelationship = request.EmergencyContactRelationship,
            EmergencyContactPhone = request.EmergencyContactPhone,
            PatientStatusId = request.PatientStatusId,
            UpdatedByUserId = updatedByUserId
        };

        var response = await _sender.Send(command, cancellationToken);

        return Ok(response);
    }

    [HttpPost("{id:guid}/archive")]
    [RequirePermission("patients.archive")]
    [ProducesResponseType(typeof(PatientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PatientResponse>> Archive(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var archivedByUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(archivedByUserIdClaim, out var archivedByUserId))
        {
            return Unauthorized();
        }

        var command = new ArchivePatientCommand
        {
            PatientId = id,
            ArchivedByUserId = archivedByUserId
        };

        var response = await _sender.Send(command, cancellationToken);

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
            IsActive = queryParameters.IsActive,
            PatientStatusId = queryParameters.PatientStatusId,
            PageNumber = queryParameters.PageNumber,
            PageSize = queryParameters.PageSize
        };

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }
}
