using EventLensAI.Application.Common;
using EventLensAI.Application.Features.Photos.PhotoProcessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventLensAI.API.Controllers;

[ApiController, Authorize, Route("api/photo-processing"), Produces("application/json")]
public sealed class PhotoProcessingController(IPhotoProcessingService service) : ControllerBase
{
    [HttpPost("render")]
    public async Task<ActionResult<ApiResponse<RenderResult>>> Render(RenderRequest request, CancellationToken ct) =>
        Ok(ApiResponse<RenderResult>.Ok(await service.RenderAsync(request, ct), "Render completed."));
    [HttpGet("status/{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProcessingStatusDto>>> Status(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<ProcessingStatusDto>.Ok(await service.StatusAsync(id, ct)));
}
