using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;
namespace EventLensAI.Application.Features.AI;
public sealed record AIBackgroundDto(Guid Id,Guid? OrganizationId,string Name,BackgroundCategory Category,string ImageUrl);
public sealed record CreateAIBackgroundRequest(Guid OrganizationId,string Name,BackgroundCategory Category,string ImageUrl);
public interface IAIAssetService
{
 Task<IReadOnlyList<AIBackgroundDto>> BackgroundsAsync(Guid? organizationId,CancellationToken ct);
 Task<AIBackgroundDto> AddBackgroundAsync(CreateAIBackgroundRequest request,CancellationToken ct);
}
public sealed class AIAssetService(IAIBackgroundRepository backgrounds,IOrganizationRepository organizations,
 ICurrentUserService currentUser,IUnitOfWork unit):IAIAssetService
{
 public async Task<IReadOnlyList<AIBackgroundDto>> BackgroundsAsync(Guid? organizationId,CancellationToken ct){var user=currentUser.UserId??throw new UnauthorizedException("Authentication is required.");return(await backgrounds.ListAsync(user,organizationId,ct)).Select(Map).ToArray();}
 public async Task<AIBackgroundDto> AddBackgroundAsync(CreateAIBackgroundRequest request,CancellationToken ct){var user=currentUser.UserId??throw new UnauthorizedException("Authentication is required.");var member=await organizations.GetMemberAsync(request.OrganizationId,user,ct);if(member is null||member.Role.Name is not(SystemRoles.Owner or SystemRoles.Manager or SystemRoles.Editor))throw new UnauthorizedException("You cannot add organization backgrounds.");if(string.IsNullOrWhiteSpace(request.ImageUrl))throw new ConflictException("Background image URL is required.");var asset=new AIBackground(request.OrganizationId,request.Name,request.Category,request.ImageUrl);await backgrounds.AddAsync(asset,ct);await unit.SaveChangesAsync(ct);return Map(asset);}
 private static AIBackgroundDto Map(AIBackground x)=>new(x.Id,x.OrganizationId,x.Name,x.Category,x.ImageUrl);
}
