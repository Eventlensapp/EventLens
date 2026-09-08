using EventLensAI.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventLensAI.API.Controllers;

[ApiController]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public ActionResult<ApiResponse<object>> Get() =>
        Ok(ApiResponse<object>.Ok(new { status = "healthy", utc = DateTime.UtcNow }));
}
