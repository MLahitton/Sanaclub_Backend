using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sanaclub.Api.Authorization.Permissions;
using Sanaclub.Api.Contracts.Appointments;
using Sanaclub.Application.Appointments.Cancel;
using Sanaclub.Application.Appointments.Common;
using Sanaclub.Application.Appointments.Confirm;
using Sanaclub.Application.Appointments.Create;
using Sanaclub.Application.Appointments.GetById;
using Sanaclub.Application.Appointments.List;
using Sanaclub.Application.Appointments.ListTherapists;
using Sanaclub.Application.Appointments.Update;
using Sanaclub.Application.Common.Models;

namespace Sanaclub.Api.Controllers;

[ApiController]
[Route("api/v1/appointments")]
[Authorize]
public sealed class AppointmentsController : ControllerBase
{
    private readonly ISender _sender;

    public AppointmentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [RequirePermission("appointments.create")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AppointmentResponse>> Create(
        [FromBody] CreateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var scheduledByUserId = ParseUserIdClaim();
        if (scheduledByUserId is null)
        {
            return Unauthorized();
        }

        var command = new CreateAppointmentCommand
        {
            PatientId = request.PatientId,
            TherapistUserId = request.TherapistUserId,
            AppointmentDate = request.AppointmentDate,
            StartTime = request.StartTime,
            Notes = request.Notes,
            ScheduledByUserId = scheduledByUserId.Value
        };

        var response = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    [RequirePermission("appointments.read")]
    [ProducesResponseType(typeof(PaginatedResult<AppointmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<AppointmentResponse>>> List(
        [FromQuery] AppointmentListQueryParameters queryParameters,
        CancellationToken cancellationToken)
    {
        var query = new ListAppointmentsQuery
        {
            Date = queryParameters.Date,
            FromDate = queryParameters.FromDate,
            ToDate = queryParameters.ToDate,
            TherapistUserId = queryParameters.TherapistUserId,
            PatientId = queryParameters.PatientId,
            ClinicalReferenceType = queryParameters.ClinicalReferenceType,
            ClinicalReferenceId = queryParameters.ClinicalReferenceId,
            Status = queryParameters.Status,
            IncludeCancelled = queryParameters.IncludeCancelled,
            PageNumber = queryParameters.PageNumber,
            PageSize = queryParameters.PageSize
        };

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }

    [HttpGet("therapists")]
    [RequirePermission("appointments.create")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AppointmentTherapistResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyCollection<AppointmentTherapistResponse>>> ListTherapists(
        CancellationToken cancellationToken)
    {
        var query = new ListAppointmentTherapistsQuery();
        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission("appointments.read")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AppointmentResponse>> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetAppointmentByIdQuery
        {
            AppointmentId = id
        };

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission("appointments.update")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AppointmentResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var updatedByUserId = ParseUserIdClaim();
        if (updatedByUserId is null)
        {
            return Unauthorized();
        }

        var command = new UpdateAppointmentCommand
        {
            AppointmentId = id,
            TherapistUserId = request.TherapistUserId,
            AppointmentDate = request.AppointmentDate,
            StartTime = request.StartTime,
            Notes = request.Notes,
            UpdatedByUserId = updatedByUserId.Value
        };

        var response = await _sender.Send(command, cancellationToken);

        return Ok(response);
    }

    [HttpPost("{id:guid}/confirm")]
    [RequirePermission("appointments.confirm")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AppointmentResponse>> Confirm(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var confirmedByUserId = ParseUserIdClaim();
        if (confirmedByUserId is null)
        {
            return Unauthorized();
        }

        var command = new ConfirmAppointmentCommand
        {
            AppointmentId = id,
            ConfirmedByUserId = confirmedByUserId.Value
        };

        var response = await _sender.Send(command, cancellationToken);

        return Ok(response);
    }

    [HttpPost("{id:guid}/cancel")]
    [RequirePermission("appointments.cancel")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AppointmentResponse>> Cancel(
        [FromRoute] Guid id,
        [FromBody] CancelAppointmentRequest? request,
        CancellationToken cancellationToken)
    {
        var cancelledByUserId = ParseUserIdClaim();
        if (cancelledByUserId is null)
        {
            return Unauthorized();
        }

        var command = new CancelAppointmentCommand
        {
            AppointmentId = id,
            Notes = request?.Notes,
            CancelledByUserId = cancelledByUserId.Value
        };

        var response = await _sender.Send(command, cancellationToken);

        return Ok(response);
    }

    private Guid? ParseUserIdClaim()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userIdClaim, out var userId)
            ? userId
            : null;
    }
}
