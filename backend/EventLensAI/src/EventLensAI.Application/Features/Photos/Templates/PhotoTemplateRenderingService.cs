using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Entities;

namespace EventLensAI.Application.Features.Photos.Templates;

public sealed class PhotoTemplateService(IPhotoTemplateRepository repo,IOrganizationRepository orgs,ICurrentUserService current,IUnitOfWork uow):IPhotoTemplateService
{
    async Task<Guid> Authorize(Guid org,CancellationToken ct)
    {
        var uid=current.UserId??throw new UnauthorizedException("Authentication is required.");
        if(await orgs.GetMemberAsync(org,uid,ct) is null)throw new UnauthorizedException("Organization access denied.");
        return uid;
    }
    public async Task<IReadOnlyList<PhotoTemplateDto>> ListAsync(Guid org,CancellationToken ct){await Authorize(org,ct);var values=await repo.ListAsync(org,ct);var result=new List<PhotoTemplateDto>();foreach(var x in values)result.Add(await Map(x,ct));return result;}
    public async Task<PhotoTemplateDto> GetAsync(Guid id,CancellationToken ct){var x=await Find(id,ct);await Authorize(x.OrganizationId,ct);return await Map(x,ct);}
    public async Task<PhotoTemplateDto> CreateAsync(UpsertPhotoTemplateRequest r,CancellationToken ct)
    {
        var uid=await Authorize(r.OrganizationId,ct);var x=new PhotoTemplate(r.OrganizationId,r.Name,r.Description,r.Category,r.Width,r.Height,r.Resolution,r.AspectRatio,r.IsPublic,uid);
        x.Update(r.Name,r.Description,r.Category,r.Width,r.Height,r.Resolution,r.AspectRatio,r.IsPublic,r.IsActive);
        await repo.AddAsync(x,ct);await repo.AddLayoutAsync(new TemplateLayout(x.Id,r.LayoutType,r.Width,r.Height,r.BackgroundColor,r.BackgroundImage),ct);
        await repo.AddElementsAsync(r.Elements.Select(e=>new TemplateElement(x.Id,e.ElementType,e.PositionX,e.PositionY,e.Width,e.Height,e.Rotation,e.LayerOrder,e.StyleConfiguration)),ct);
        await uow.SaveChangesAsync(ct);return await Map(x,ct);
    }
    public async Task<PhotoTemplateDto> UpdateAsync(Guid id,UpsertPhotoTemplateRequest r,CancellationToken ct)
    {
        var x=await Find(id,ct);await Authorize(x.OrganizationId,ct);if(x.OrganizationId!=r.OrganizationId)throw new ConflictException("A template cannot move between organizations.");
        x.Update(r.Name,r.Description,r.Category,r.Width,r.Height,r.Resolution,r.AspectRatio,r.IsPublic,r.IsActive);
        var layout=await repo.LayoutAsync(id,ct);if(layout is not null)repo.RemoveLayout(layout);repo.RemoveElements(await repo.ElementsAsync(id,ct));
        await repo.AddLayoutAsync(new TemplateLayout(id,r.LayoutType,r.Width,r.Height,r.BackgroundColor,r.BackgroundImage),ct);
        await repo.AddElementsAsync(r.Elements.Select(e=>new TemplateElement(id,e.ElementType,e.PositionX,e.PositionY,e.Width,e.Height,e.Rotation,e.LayerOrder,e.StyleConfiguration)),ct);
        await uow.SaveChangesAsync(ct);return await Map(x,ct);
    }
    public async Task DeleteAsync(Guid id,CancellationToken ct){var x=await Find(id,ct);var uid=await Authorize(x.OrganizationId,ct);x.SoftDelete(uid);await uow.SaveChangesAsync(ct);}
    public async Task<PhotoTemplateDto> DuplicateAsync(Guid id,CancellationToken ct){var x=await GetAsync(id,ct);return await CreateAsync(new(x.OrganizationId,$"{x.Name} copy",x.Description,x.Category,x.Width,x.Height,x.Resolution,x.AspectRatio,false,true,x.Layout.LayoutType,x.Layout.BackgroundColor,x.Layout.BackgroundImage,x.Elements.Select(e=>new UpsertTemplateElement(e.ElementType,e.PositionX,e.PositionY,e.Width,e.Height,e.Rotation,e.LayerOrder,e.StyleConfiguration)).ToArray()),ct);}
    public async Task<PhotoTemplateDto> SetActiveAsync(Guid id,bool active,CancellationToken ct){var x=await Find(id,ct);await Authorize(x.OrganizationId,ct);x.SetActive(active);await uow.SaveChangesAsync(ct);return await Map(x,ct);}
    async Task<PhotoTemplate> Find(Guid id,CancellationToken ct)=>await repo.GetAsync(id,ct)??throw new NotFoundException("Photo template was not found.");
    async Task<PhotoTemplateDto> Map(PhotoTemplate x,CancellationToken ct){var l=await repo.LayoutAsync(x.Id,ct)??throw new NotFoundException("Template layout was not found.");var es=await repo.ElementsAsync(x.Id,ct);return new(x.Id,x.OrganizationId,x.Name,x.Description,x.Category,x.Width,x.Height,x.Resolution,x.AspectRatio,x.IsPublic,x.IsActive,new(l.Id,l.LayoutType,l.CanvasWidth,l.CanvasHeight,l.BackgroundColor,l.BackgroundImage),es.Select(e=>new TemplateElementDto(e.Id,e.ElementType,e.PositionX,e.PositionY,e.Width,e.Height,e.Rotation,e.LayerOrder,e.StyleConfiguration)).ToArray(),x.CreatedAt);}
}
public sealed class TemplateRendererService(IPhotoTemplateRepository repo,IPhotoComposerService composer,IOrganizationRepository orgs,ICurrentUserService current):ITemplateRendererService
{
    public async Task<RenderedPhotoDto> RenderAsync(Guid id,RenderTemplateRequest r,CancellationToken ct){var uid=current.UserId??throw new UnauthorizedException("Authentication is required.");if(await orgs.GetMemberAsync(r.OrganizationId,uid,ct)is null)throw new UnauthorizedException("Organization access denied.");var x=await repo.GetAsync(id,ct)??throw new NotFoundException("Template not found.");if(x.OrganizationId!=r.OrganizationId&&!x.IsPublic)throw new UnauthorizedException("Template access denied.");var l=await repo.LayoutAsync(id,ct)??throw new NotFoundException("Layout not found.");var bytes=await composer.ComposeAsync(x,l,await repo.ElementsAsync(id,ct),r,ct);return new(id,"image/jpeg",l.CanvasWidth,l.CanvasHeight,bytes.LongLength,$"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}",r.Preview);}
}
public sealed class StickerService(IPhotoTemplateRepository repo,IOrganizationRepository orgs,ICurrentUserService current):IStickerService
{
    public async Task<IReadOnlyList<StickerDto>> ListAsync(Guid org,CancellationToken ct){var uid=current.UserId??throw new UnauthorizedException("Authentication is required.");if(await orgs.GetMemberAsync(org,uid,ct)is null)throw new UnauthorizedException("Organization access denied.");return(await repo.StickersAsync(org,ct)).Select(x=>new StickerDto(x.Id,x.Name,x.Category,x.IsPublic)).ToArray();}
}
