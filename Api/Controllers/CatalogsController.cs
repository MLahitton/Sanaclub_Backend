using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sanaclub.Api.Authorization.Permissions;
using Sanaclub.Application.Catalogs.PatientFormOptions;

namespace Sanaclub.Api.Controllers;

[ApiController]
[Route("api/v1/catalogs")]
[Authorize]
public sealed class CatalogsController : ControllerBase
{
    private readonly ISender _sender;

    public CatalogsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("patient-form-options")]
    [RequirePermission("catalogs.read")]
    [ProducesResponseType(typeof(PatientFormOptionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PatientFormOptionsResponse>> GetPatientFormOptions(
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetPatientFormOptionsQuery(), cancellationToken);

        return Ok(response);
    }
}
