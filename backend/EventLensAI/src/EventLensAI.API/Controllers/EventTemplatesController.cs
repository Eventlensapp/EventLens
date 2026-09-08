using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Events;
using EventLensAI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventLensAI.API.Controllers;

[ApiController,Authorize,Route("api/event-templates"),Produces("application/json")]
public sealed class EventTemplatesController(IEventTemplateService service):ControllerBase
{
    [HttpGet]public async Task<ActionResult<ApiResponse<IReadOnlyList<EventTemplateDto>>>>List([FromQuery]Guid organizationId,CancellationToken ct)=>
        Ok(ApiResponse<IReadOnlyList<EventTemplateDto>>.Ok(await service.ListAsync(organizationId,ct)));
    [HttpGet("{id:guid}")]public async Task<ActionResult<ApiResponse<EventTemplateDto>>>Get(Guid id,CancellationToken ct)=>
        Ok(ApiResponse<EventTemplateDto>.Ok(await service.GetAsync(id,ct)));
    [HttpPost,Authorize(Policy="ManageEvents")]public async Task<ActionResult<ApiResponse<EventTemplateDto>>>Create(CreateEventTemplateRequest request,CancellationToken ct)
    {var x=await service.CreateAsync(request,ct);return CreatedAtAction(nameof(Get),new{id=x.Id},ApiResponse<EventTemplateDto>.Ok(x,"Event template created."));}
    [HttpPut("{id:guid}"),Authorize(Policy="ManageEvents")]public async Task<ActionResult<ApiResponse<EventTemplateDto>>>Update(Guid id,UpdateEventTemplateRequest request,CancellationToken ct)=>
        Ok(ApiResponse<EventTemplateDto>.Ok(await service.UpdateAsync(id,request,ct),"Event template updated."));
    [HttpDelete("{id:guid}"),Authorize(Policy="ManageEvents")]public async Task<ActionResult<ApiResponse<object>>>Archive(Guid id,CancellationToken ct)
    {await service.ArchiveAsync(id,ct);return Ok(ApiResponse<object>.Ok(new{},"Event template archived."));}
    [HttpPost("{id:guid}/clone"),Authorize(Policy="ManageEvents")]public async Task<ActionResult<ApiResponse<EventTemplateDto>>>Clone(Guid id,CloneEventTemplateRequest request,CancellationToken ct)=>
        Ok(ApiResponse<EventTemplateDto>.Ok(await service.CloneAsync(id,request,ct),"Event template cloned."));
}
