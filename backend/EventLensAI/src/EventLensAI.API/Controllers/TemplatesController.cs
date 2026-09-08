using EventLensAI.Application.Common;
using EventLensAI.Application.Features.Photos.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventLensAI.API.Controllers;

[ApiController, Authorize, Route("api/templates"), Produces("application/json")]
public sealed class TemplatesController(ITemplateService service,IPhotoTemplateService photoTemplates,ITemplateRendererService renderer) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TemplateDto>>>> List([FromQuery] Guid? organizationId, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<TemplateDto>>.Ok(await service.ListAsync(organizationId, ct)));
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TemplateDto>>> Create(UpsertTemplateRequest request, CancellationToken ct) =>
        Ok(ApiResponse<TemplateDto>.Ok(await service.CreateAsync(request, ct), "Template created."));
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TemplateDto>>> Update(Guid id, UpsertTemplateRequest request, CancellationToken ct) =>
        Ok(ApiResponse<TemplateDto>.Ok(await service.UpdateAsync(id, request, ct), "Template updated."));
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    { await service.DeleteAsync(id, ct); return Ok(ApiResponse<object>.Ok(new { }, "Template deleted.")); }

    [HttpGet("photo")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PhotoTemplateDto>>>> PhotoList([FromQuery] Guid organizationId,CancellationToken ct)=>
        Ok(ApiResponse<IReadOnlyList<PhotoTemplateDto>>.Ok(await photoTemplates.ListAsync(organizationId,ct)));
    [HttpGet("photo/{id:guid}")]
    public async Task<ActionResult<ApiResponse<PhotoTemplateDto>>> PhotoGet(Guid id,CancellationToken ct)=>Ok(ApiResponse<PhotoTemplateDto>.Ok(await photoTemplates.GetAsync(id,ct)));
    [HttpPost("photo")]
    public async Task<ActionResult<ApiResponse<PhotoTemplateDto>>> PhotoCreate(UpsertPhotoTemplateRequest request,CancellationToken ct)=>Ok(ApiResponse<PhotoTemplateDto>.Ok(await photoTemplates.CreateAsync(request,ct),"Photo template created."));
    [HttpPut("photo/{id:guid}")]
    public async Task<ActionResult<ApiResponse<PhotoTemplateDto>>> PhotoUpdate(Guid id,UpsertPhotoTemplateRequest request,CancellationToken ct)=>Ok(ApiResponse<PhotoTemplateDto>.Ok(await photoTemplates.UpdateAsync(id,request,ct),"Photo template updated."));
    [HttpDelete("photo/{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> PhotoDelete(Guid id,CancellationToken ct){await photoTemplates.DeleteAsync(id,ct);return Ok(ApiResponse<object>.Ok(new{},"Photo template deleted."));}
    [HttpPost("photo/{id:guid}/duplicate")]
    public async Task<ActionResult<ApiResponse<PhotoTemplateDto>>> Duplicate(Guid id,CancellationToken ct)=>Ok(ApiResponse<PhotoTemplateDto>.Ok(await photoTemplates.DuplicateAsync(id,ct),"Template duplicated."));
    [HttpPost("photo/{id:guid}/active")]
    public async Task<ActionResult<ApiResponse<PhotoTemplateDto>>> Active(Guid id,[FromQuery]bool value,CancellationToken ct)=>Ok(ApiResponse<PhotoTemplateDto>.Ok(await photoTemplates.SetActiveAsync(id,value,ct)));
    [HttpPost("{id:guid}/render")]
    public async Task<ActionResult<ApiResponse<RenderedPhotoDto>>> Render(Guid id,RenderTemplateRequest request,CancellationToken ct)=>Ok(ApiResponse<RenderedPhotoDto>.Ok(await renderer.RenderAsync(id,request with{Preview=false},ct)));
    [HttpPost("{id:guid}/preview")]
    public async Task<ActionResult<ApiResponse<RenderedPhotoDto>>> Preview(Guid id,RenderTemplateRequest request,CancellationToken ct)=>Ok(ApiResponse<RenderedPhotoDto>.Ok(await renderer.RenderAsync(id,request with{Preview=true},ct)));
}
