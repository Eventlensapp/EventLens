using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Organizations;
using EventLensAI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventLensAI.API.Controllers;

[ApiController, Authorize]
public sealed class BranchesController(IBranchService service) : ControllerBase
{
    [HttpGet("api/organizations/{organizationId:guid}/branches")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<BranchDto>>>> List(Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<BranchDto>>.Ok(await service.ListAsync(organizationId,ct)));
    [HttpPost("api/organizations/{organizationId:guid}/branches")]
    public async Task<ActionResult<ApiResponse<BranchDto>>> Create(Guid organizationId,CreateBranchRequest request,CancellationToken ct){var data=await service.CreateAsync(organizationId,request,ct);return CreatedAtAction(nameof(List),new{organizationId},ApiResponse<BranchDto>.Ok(data,"Branch created."));}
    [HttpPut("api/organizations/{organizationId:guid}/branches/{id:guid}")]
    public async Task<ActionResult<ApiResponse<BranchDto>>> Update(Guid organizationId,Guid id,UpdateBranchRequest request,CancellationToken ct)=>Ok(ApiResponse<BranchDto>.Ok(await service.UpdateAsync(organizationId,id,request,ct),"Branch updated."));
    [HttpDelete("api/organizations/{organizationId:guid}/branches/{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Archive(Guid organizationId,Guid id,CancellationToken ct){await service.ArchiveAsync(organizationId,id,ct);return Ok(ApiResponse<object>.Ok(new{},"Branch archived."));}
    [HttpGet("api/branches/{branchId:guid}/members")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UnitMemberDto>>>> Members(Guid branchId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<UnitMemberDto>>.Ok(await service.ListMembersAsync(branchId,ct)));
    [HttpPost("api/branches/{branchId:guid}/members")]
    public async Task<ActionResult<ApiResponse<object>>> AddMember(Guid branchId,AssignUnitMemberRequest request,CancellationToken ct){await service.AddMemberAsync(branchId,request,ct);return Ok(ApiResponse<object>.Ok(new{},"Member assigned."));}
    [HttpDelete("api/branches/{branchId:guid}/members/{userId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveMember(Guid branchId,Guid userId,CancellationToken ct){await service.RemoveMemberAsync(branchId,userId,ct);return Ok(ApiResponse<object>.Ok(new{},"Member removed."));}
}
