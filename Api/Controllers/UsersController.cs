using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sanaclub.Api.Authorization.Permissions;
using Sanaclub.Api.Contracts.Users;
using Sanaclub.Application.Common.Models;
using Sanaclub.Application.Users.Activate;
using Sanaclub.Application.Users.Common;
using Sanaclub.Application.Users.Create;
using Sanaclub.Application.Users.Deactivate;
using Sanaclub.Application.Users.GetById;
using Sanaclub.Application.Users.List;
using Sanaclub.Application.Users.ResetPassword;
using Sanaclub.Application.Users.Update;

namespace Sanaclub.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [RequirePermission("users.read")]
    [ProducesResponseType(typeof(PaginatedResult<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResult<UserResponse>>> List(
        [FromQuery] UserListQueryParameters queryParameters,
        CancellationToken cancellationToken)
    {
        var query = new ListUsersQuery
        {
            Search = queryParameters.Search,
            Role = queryParameters.Role,
            IsActive = queryParameters.IsActive,
            PageNumber = queryParameters.PageNumber,
            PageSize = queryParameters.PageSize
        };

        var response = await _sender.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission("users.read")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery
        {
            UserId = id
        };

        var response = await _sender.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [RequirePermission("users.create")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var createdByUserId = ParseUserIdClaim();
        if (createdByUserId is null)
        {
            return Unauthorized();
        }

        var command = new CreateUserCommand
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            RoleCode = request.RoleCode,
            TemporaryPassword = request.TemporaryPassword,
            CreatedByUserId = createdByUserId.Value
        };

        var response = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission("users.update")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var updatedByUserId = ParseUserIdClaim();
        if (updatedByUserId is null)
        {
            return Unauthorized();
        }

        var command = new UpdateUserCommand
        {
            UserId = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            RoleCode = request.RoleCode,
            UpdatedByUserId = updatedByUserId.Value
        };

        var response = await _sender.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPost("{id:guid}/activate")]
    [RequirePermission("users.change_status")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> Activate(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var activatedByUserId = ParseUserIdClaim();
        if (activatedByUserId is null)
        {
            return Unauthorized();
        }

        var command = new ActivateUserCommand
        {
            UserId = id,
            ActivatedByUserId = activatedByUserId.Value
        };

        var response = await _sender.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPost("{id:guid}/deactivate")]
    [RequirePermission("users.change_status")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> Deactivate(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var deactivatedByUserId = ParseUserIdClaim();
        if (deactivatedByUserId is null)
        {
            return Unauthorized();
        }

        var command = new DeactivateUserCommand
        {
            UserId = id,
            DeactivatedByUserId = deactivatedByUserId.Value
        };

        var response = await _sender.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPost("{id:guid}/reset-password")]
    [RequirePermission("users.reset_password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResetPassword(
        [FromRoute] Guid id,
        [FromBody] ResetUserPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var resetByUserId = ParseUserIdClaim();
        if (resetByUserId is null)
        {
            return Unauthorized();
        }

        var command = new ResetUserPasswordCommand
        {
            UserId = id,
            NewTemporaryPassword = request.NewTemporaryPassword,
            ResetByUserId = resetByUserId.Value
        };

        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    private Guid? ParseUserIdClaim()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        return null;
    }
}

