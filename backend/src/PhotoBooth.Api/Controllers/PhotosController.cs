using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoBooth.Application.Abstractions.Persistence;
using PhotoBooth.Application.Modules.Photos;
using PhotoBooth.Domain.Entities;

namespace PhotoBooth.Api.Controllers;

[ApiController, Authorize, Route("api/photos")]
public sealed class PhotosController(IRepository<Photo> repository, IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PhotoResponse>>> List(CancellationToken cancellationToken) =>
        Ok((await repository.ListAsync(cancellationToken)).Select(ToResponse));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PhotoResponse>> Get(Guid id, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(ToResponse(item));
    }

    [HttpPost]
    public async Task<ActionResult<PhotoResponse>> Create(CreatePhotoRequest request, CancellationToken cancellationToken)
    {
        var item = new Photo(request.EventId, request.TemplateId, request.StorageUrl);
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

    private static PhotoResponse ToResponse(Photo item) =>
        new(item.Id, item.EventId, item.TemplateId, item.StorageUrl);
}
