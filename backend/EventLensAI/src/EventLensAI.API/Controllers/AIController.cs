using EventLensAI.Application.Common;
using EventLensAI.Application.Features.AI;
using EventLensAI.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EventLensAI.API.Controllers;
[ApiController,Authorize,Route("api/ai"),Produces("application/json")]
public sealed class AIController(IAIProcessingService service,IAIAssetService assets):ControllerBase
{
 [HttpPost("background")]public Task<ActionResult<ApiResponse<AIJobDto>>> Background(CreateAIJobRequest request,CancellationToken ct)=>Queue(request.Background is null?AIJobType.BackgroundRemoval:AIJobType.BackgroundReplacement,request,ct);
 [HttpPost("style")]public Task<ActionResult<ApiResponse<AIJobDto>>> Style(CreateAIJobRequest request,CancellationToken ct)=>Queue(AIJobType.StyleTransfer,request,ct);
 [HttpPost("enhance")]public Task<ActionResult<ApiResponse<AIJobDto>>> Enhance(CreateAIJobRequest request,CancellationToken ct)=>Queue(AIJobType.FaceEnhancement,request,ct);
 [HttpPost("upscale")]public Task<ActionResult<ApiResponse<AIJobDto>>> Upscale(CreateAIJobRequest request,CancellationToken ct)=>Queue(AIJobType.ImageUpscale,request,ct);
 [HttpPost("props")]public Task<ActionResult<ApiResponse<AIJobDto>>> Props(CreateAIJobRequest request,CancellationToken ct)=>Queue(AIJobType.AIProps,request,ct);
 [HttpPost("magic-erase")]public Task<ActionResult<ApiResponse<AIJobDto>>> MagicErase(CreateAIJobRequest request,CancellationToken ct)=>Queue(AIJobType.MagicErase,request,ct);
 [HttpPost("generative-fill")]public Task<ActionResult<ApiResponse<AIJobDto>>> GenerativeFill(CreateAIJobRequest request,CancellationToken ct)=>Queue(AIJobType.GenerativeFill,request,ct);
 [HttpPost("memory-book")]public async Task<ActionResult<ApiResponse<AIJobDto>>> MemoryBook(MemoryBookRequest request,CancellationToken ct)=>Ok(ApiResponse<AIJobDto>.Ok(await service.QueueMemoryBookAsync(request,ct),"Memory book queued."));
 [HttpPost("template")]public Task<ActionResult<ApiResponse<AIJobDto>>> Template(CreateAIJobRequest request,CancellationToken ct)=>Queue(AIJobType.TemplateGeneration,request,ct);
 [HttpPost("smart-selection")]public Task<ActionResult<ApiResponse<AIJobDto>>> SmartSelection(CreateAIJobRequest request,CancellationToken ct)=>Queue(AIJobType.SmartPhotoSelection,request,ct);
 [HttpGet("jobs")]public async Task<ActionResult<ApiResponse<IReadOnlyList<AIJobDto>>>> Jobs([FromQuery]Guid? eventId,[FromQuery]AIJobStatus? status,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<AIJobDto>>.Ok(await service.ListAsync(eventId,status,ct)));
 [HttpGet("jobs/{id:guid}")]public async Task<ActionResult<ApiResponse<AIJobDto>>> Job(Guid id,CancellationToken ct)=>Ok(ApiResponse<AIJobDto>.Ok(await service.GetAsync(id,ct)));
 [HttpPost("jobs/{id:guid}/retry")]public async Task<ActionResult<ApiResponse<AIJobDto>>> Retry(Guid id,CancellationToken ct)=>Ok(ApiResponse<AIJobDto>.Ok(await service.RetryAsync(id,ct),"Job requeued."));
 [HttpGet("backgrounds")]public async Task<ActionResult<ApiResponse<IReadOnlyList<AIBackgroundDto>>>> Backgrounds([FromQuery]Guid? organizationId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<AIBackgroundDto>>.Ok(await assets.BackgroundsAsync(organizationId,ct)));
 [HttpPost("backgrounds")]public async Task<ActionResult<ApiResponse<AIBackgroundDto>>> AddBackground(CreateAIBackgroundRequest request,CancellationToken ct)=>Ok(ApiResponse<AIBackgroundDto>.Ok(await assets.AddBackgroundAsync(request,ct),"Background added."));
 private async Task<ActionResult<ApiResponse<AIJobDto>>> Queue(AIJobType type,CreateAIJobRequest request,CancellationToken ct)=>Accepted(ApiResponse<AIJobDto>.Ok(await service.QueueAsync(type,request,ct),"AI job queued."));
}
