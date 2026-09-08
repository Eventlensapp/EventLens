using EventLensAI.Application.Common;using EventLensAI.Application.DTOs.Organizations;using EventLensAI.Application.Services;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;
namespace EventLensAI.API.Controllers;
[ApiController,Authorize,Route("api/organizations/{organizationId:guid}/security")]
public sealed class OrganizationSecurityController(IOrganizationSecurityService service):ControllerBase
{
 [HttpGet("members/{userId:guid}/permissions")]public async Task<ActionResult<ApiResponse<IReadOnlyList<PermissionDto>>>>Permissions(Guid organizationId,Guid userId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<PermissionDto>>.Ok(await service.GetMemberPermissionsAsync(organizationId,userId,ct)));
 [HttpPut("members/{userId:guid}/permissions")]public async Task<ActionResult<ApiResponse<object>>>SetPermission(Guid organizationId,Guid userId,SetMemberPermissionRequest request,CancellationToken ct){await service.SetMemberPermissionAsync(organizationId,userId,request,ct);return Ok(ApiResponse<object>.Ok(new{},"Permission updated."));}
 [HttpPost("ownership-transfers")]public async Task<ActionResult<ApiResponse<OwnershipTransferDto>>>Start(Guid organizationId,CreateOwnershipTransferRequest request,CancellationToken ct)=>Ok(ApiResponse<OwnershipTransferDto>.Ok(await service.StartTransferAsync(organizationId,request,ct),"Ownership transfer started."));
 [HttpGet("ownership-transfers/pending")]public async Task<ActionResult<ApiResponse<OwnershipTransferDto?>>>Pending(Guid organizationId,CancellationToken ct)=>Ok(ApiResponse<OwnershipTransferDto?>.Ok(await service.GetPendingTransferAsync(organizationId,ct)));
 [HttpPost("ownership-transfers/{transferId:guid}/accept")]public async Task<ActionResult<ApiResponse<object>>>Accept(Guid organizationId,Guid transferId,CancellationToken ct){await service.AcceptTransferAsync(organizationId,transferId,ct);return Ok(ApiResponse<object>.Ok(new{},"Ownership transferred."));}
 [HttpDelete("ownership-transfers/{transferId:guid}")]public async Task<ActionResult<ApiResponse<object>>>Cancel(Guid organizationId,Guid transferId,CancellationToken ct){await service.CancelTransferAsync(organizationId,transferId,ct);return Ok(ApiResponse<object>.Ok(new{},"Transfer cancelled."));}
}
