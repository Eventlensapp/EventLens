using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Organizations;
using EventLensAI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EventLensAI.API.Controllers;
[ApiController,Authorize,Route("api/organizations/{organizationId:guid}/members")]
public sealed class OrganizationMembersController(IOrganizationService service):ControllerBase
{
    [HttpGet] public async Task<ActionResult<ApiResponse<IReadOnlyList<OrganizationMemberDto>>>> List(Guid organizationId,CancellationToken ct)=>
        Ok(ApiResponse<IReadOnlyList<OrganizationMemberDto>>.Ok(await service.ListMembersAsync(organizationId,ct)));
    [HttpPost] public async Task<ActionResult<ApiResponse<OrganizationMemberDto>>> Add(Guid organizationId,AddOrganizationMemberRequest request,CancellationToken ct)=>
        Ok(ApiResponse<OrganizationMemberDto>.Ok(await service.AddMemberAsync(organizationId,request,ct),"Member added."));
    [HttpPost("accounts")] public async Task<ActionResult<ApiResponse<OrganizationMemberDto>>> Create(Guid organizationId,CreateOrganizationMemberRequest request,CancellationToken ct)=>
        StatusCode(201,ApiResponse<OrganizationMemberDto>.Ok(await service.CreateMemberAsync(organizationId,request,ct),"Member account created."));
    [HttpPut("{userId:guid}/role")] public async Task<ActionResult<ApiResponse<object>>> ChangeRole(Guid organizationId,Guid userId,ChangeMemberRoleRequest request,CancellationToken ct)
    {await service.ChangeRoleByUserAsync(organizationId,userId,request.Role,ct);return Ok(ApiResponse<object>.Ok(new{},"Role updated."));}
    [HttpDelete("{userId:guid}")] public async Task<ActionResult<ApiResponse<object>>> Remove(Guid organizationId,Guid userId,CancellationToken ct)
    {await service.RemoveMemberByUserAsync(organizationId,userId,ct);return Ok(ApiResponse<object>.Ok(new{},"Member removed."));}
}
