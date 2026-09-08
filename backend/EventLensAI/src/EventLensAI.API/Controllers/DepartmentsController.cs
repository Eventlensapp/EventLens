using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Organizations;
using EventLensAI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventLensAI.API.Controllers;

[ApiController, Authorize]
public sealed class DepartmentsController(IDepartmentService service) : ControllerBase
{
    [HttpGet("api/organizations/{organizationId:guid}/departments")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DepartmentDto>>>> List(Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<DepartmentDto>>.Ok(await service.ListAsync(organizationId,ct)));
    [HttpPost("api/organizations/{organizationId:guid}/departments")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> Create(Guid organizationId,CreateDepartmentRequest request,CancellationToken ct){var data=await service.CreateAsync(organizationId,request,ct);return CreatedAtAction(nameof(List),new{organizationId},ApiResponse<DepartmentDto>.Ok(data,"Department created."));}
    [HttpPut("api/organizations/{organizationId:guid}/departments/{id:guid}")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> Update(Guid organizationId,Guid id,UpdateDepartmentRequest request,CancellationToken ct)=>Ok(ApiResponse<DepartmentDto>.Ok(await service.UpdateAsync(organizationId,id,request,ct),"Department updated."));
    [HttpDelete("api/organizations/{organizationId:guid}/departments/{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Archive(Guid organizationId,Guid id,CancellationToken ct){await service.ArchiveAsync(organizationId,id,ct);return Ok(ApiResponse<object>.Ok(new{},"Department archived."));}
    [HttpGet("api/departments/{departmentId:guid}/members")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UnitMemberDto>>>> Members(Guid departmentId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<UnitMemberDto>>.Ok(await service.ListMembersAsync(departmentId,ct)));
    [HttpPost("api/departments/{departmentId:guid}/members")]
    public async Task<ActionResult<ApiResponse<object>>> AddMember(Guid departmentId,AssignUnitMemberRequest request,CancellationToken ct){await service.AddMemberAsync(departmentId,request,ct);return Ok(ApiResponse<object>.Ok(new{},"Member assigned."));}
    [HttpDelete("api/departments/{departmentId:guid}/members/{userId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveMember(Guid departmentId,Guid userId,CancellationToken ct){await service.RemoveMemberAsync(departmentId,userId,ct);return Ok(ApiResponse<object>.Ok(new{},"Member removed."));}
}
