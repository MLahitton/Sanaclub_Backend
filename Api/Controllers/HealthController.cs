using Microsoft.AspNetCore.Mvc;

namespace Sanaclub.Api.Controllers;

[ApiController]
[Route("api/v1/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "Healthy",
            service = "Sanaclub API",
            timestampUtc = DateTime.UtcNow
        });
    }
}