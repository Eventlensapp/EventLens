using System.Text.Json;
using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Entities;

namespace EventLensAI.Application.Features.Photos.Templates;

public interface ITemplateService
{
    Task<IReadOnlyList<TemplateDto>> ListAsync(Guid? organizationId, CancellationToken ct);
    Task<TemplateDto> CreateAsync(UpsertTemplateRequest request, CancellationToken ct);
    Task<TemplateDto> UpdateAsync(Guid id, UpsertTemplateRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

public sealed class TemplateService(ITemplateRepository templates, IOrganizationRepository organizations,
    ICurrentUserService currentUser, IUnitOfWork unitOfWork) : ITemplateService
{
    public async Task<IReadOnlyList<TemplateDto>> ListAsync(Guid? organizationId, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Authentication is required.");
        return (await templates.ListAsync(userId, organizationId, ct)).Select(Map).ToArray();
    }
    public async Task<TemplateDto> CreateAsync(UpsertTemplateRequest request, CancellationToken ct)
    {
        var userId = await Authorize(request.OrganizationId, ct);
        EnsureJson(request.ConfigurationJson);
        var template = new Template(request.OrganizationId, request.Name, request.Category, request.ConfigurationJson, userId);
        template.Update(request.Name, request.Category, request.PreviewImage, request.ConfigurationJson, request.IsPremium);
        await templates.AddAsync(template, ct); await unitOfWork.SaveChangesAsync(ct); return Map(template);
    }
    public async Task<TemplateDto> UpdateAsync(Guid id, UpsertTemplateRequest request, CancellationToken ct)
    {
        var template = await templates.GetAsync(id, ct) ?? throw new NotFoundException("Template was not found.");
        if (template.OrganizationId is null) throw new UnauthorizedException("Global templates cannot be changed here.");
        await Authorize(template.OrganizationId.Value, ct); EnsureJson(request.ConfigurationJson);
        if (template.OrganizationId != request.OrganizationId) throw new ConflictException("A template cannot move between organizations.");
        template.Update(request.Name, request.Category, request.PreviewImage, request.ConfigurationJson, request.IsPremium);
        await unitOfWork.SaveChangesAsync(ct); return Map(template);
    }
    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var template = await templates.GetAsync(id, ct) ?? throw new NotFoundException("Template was not found.");
        if (template.OrganizationId is null) throw new UnauthorizedException("Global templates cannot be deleted here.");
        var userId = await Authorize(template.OrganizationId.Value, ct); template.SoftDelete(userId); await unitOfWork.SaveChangesAsync(ct);
    }
    private async Task<Guid> Authorize(Guid organizationId, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Authentication is required.");
        var member = await organizations.GetMemberAsync(organizationId, userId, ct);
        if (member is null || member.Role.Name is not (SystemRoles.Owner or SystemRoles.Manager or SystemRoles.Editor))
            throw new UnauthorizedException("You cannot manage templates for this organization.");
        return userId;
    }
    private static void EnsureJson(string value) { try { JsonDocument.Parse(value); } catch (JsonException) { throw new ConflictException("Template configuration must be valid JSON."); } }
    private static TemplateDto Map(Template t) => new(t.Id, t.OrganizationId, t.Name, t.Category, t.PreviewImage, t.ConfigurationJson, t.IsPremium, t.CreatedBy, t.CreatedAt);
}
