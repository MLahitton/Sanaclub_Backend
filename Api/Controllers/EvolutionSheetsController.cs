using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sanaclub.Api.Authorization.Permissions;
using Sanaclub.Api.Contracts.EvolutionSheets;
using Sanaclub.Application.EvolutionSheets.Common;
using Sanaclub.Application.EvolutionSheets.CompleteNewIndications;
using Sanaclub.Application.EvolutionSheets.GetById;

namespace Sanaclub.Api.Controllers;

[ApiController]
[Route("api/v1/evolution-sheets")]
[Authorize]
public sealed class EvolutionSheetsController : ControllerBase
{
    private readonly ISender _sender;

    public EvolutionSheetsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id:guid}")]
    [RequirePermission("evolutions.read")]
    [ProducesResponseType(typeof(EvolutionSheetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EvolutionSheetResponse>> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetEvolutionSheetByIdQuery
        {
            EvolutionSheetId = id
        };

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }

    [HttpPut("{id:guid}/new-indications")]
    [RequirePermission("evolutions.update_draft")]
    [ProducesResponseType(typeof(EvolutionSheetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EvolutionSheetResponse>> CompleteNewIndications(
        [FromRoute] Guid id,
        [FromBody] CompleteEvolutionSheetNewIndicationsRequest request,
        CancellationToken cancellationToken)
    {
        var completedByUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(completedByUserIdClaim, out var completedByUserId))
        {
            return Unauthorized();
        }

        var command = new CompleteEvolutionSheetNewIndicationsCommand
        {
            EvolutionSheetId = id,
            NewIndications = request.NewIndications,
            CompletedByUserId = completedByUserId
        };

        var response = await _sender.Send(command, cancellationToken);

        return Ok(response);
    }
}
