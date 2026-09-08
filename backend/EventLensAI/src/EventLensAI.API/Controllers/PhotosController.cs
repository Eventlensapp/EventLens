using EventLensAI.Application.Common;
using EventLensAI.Application.Features.Photos.PhotoProcessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventLensAI.API.Controllers;

[ApiController, Authorize, Route("api/photos"), Produces("application/json")]
public sealed class PhotosController(IPhotoProcessingService service) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PhotoDto>>> Get(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<PhotoDto>.Ok(await service.GetAsync(id, ct)));
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PhotoDto>>> Register(RegisterPhotoRequest request, CancellationToken ct) =>
        Ok(ApiResponse<PhotoDto>.Ok(await service.RegisterAsync(request, ct), "Photo registered."));
    [HttpPost("process")]
    public async Task<ActionResult<ApiResponse<RenderResult>>> Process(ProcessPhotoRequest request, CancellationToken ct) =>
        Ok(ApiResponse<RenderResult>.Ok(await service.ProcessAsync(request, ct), "Photo processed."));
    [HttpPost("export")]
    public async Task<ActionResult<ApiResponse<RenderResult>>> Export(ExportPhotoRequest request, CancellationToken ct) =>
        Ok(ApiResponse<RenderResult>.Ok(await service.ExportAsync(request, ct), "Export generated."));
}
