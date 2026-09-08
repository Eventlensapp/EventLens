using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoBooth.Application.Abstractions.Persistence;
using PhotoBooth.Application.Modules.Templates;
using PhotoBooth.Domain.Entities;

namespace PhotoBooth.Api.Controllers;

[ApiController, Authorize, Route("api/templates")]
public sealed class TemplatesController(IRepository<Template> repository, IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TemplateResponse>>> List(CancellationToken cancellationToken) =>
        Ok((await repository.ListAsync(cancellationToken)).Select(ToResponse));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TemplateResponse>> Get(Guid id, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(ToResponse(item));
    }

    [HttpPost]
    public async Task<ActionResult<TemplateResponse>> Create(CreateTemplateRequest request, CancellationToken cancellationToken)
    {
        var item = new Template(request.Name, request.Definition);
        await repository.AddAsync(item, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, ToResponse(item));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(id, cancellationToken);
        if (item is null) return NotFound();
        repository.Remove(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static TemplateResponse ToResponse(Template item) =>
        new(item.Id, item.Name, item.Definition);
}
