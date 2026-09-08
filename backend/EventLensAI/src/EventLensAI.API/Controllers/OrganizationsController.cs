using EventLensAI.Application.Common;
using EventLensAI.Application.DTOs.Organizations;
using EventLensAI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventLensAI.API.Controllers;

[ApiController, Authorize]
[Route("api/organizations")]
[Produces("application/json")]
public sealed class OrganizationsController(IOrganizationService service) : ControllerBase
{
    /// <summary>Lists organizations available to the current user.</summary>
    [HttpGet]
    [ProducesResponseType<ApiResponse<IReadOnlyList<OrganizationDto>>>(200)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<OrganizationDto>>>> List(CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<OrganizationDto>>.Ok(await service.ListAsync(ct)));

    /// <summary>Gets the active organization selected by the client using X-Organization-Id.</summary>
    [HttpGet("current")]
    public async Task<ActionResult<ApiResponse<OrganizationDto>>> Current(CancellationToken ct)
    {
        if (!Request.Headers.TryGetValue("X-Organization-Id", out var value) ||
            !Guid.TryParse(value.ToString(), out var organizationId))
            return BadRequest(ApiResponse<OrganizationDto>.Fail("X-Organization-Id header is required."));
        return Ok(ApiResponse<OrganizationDto>.Ok(await service.GetAsync(organizationId, ct)));
    }

    /// <summary>Gets an organization and its current subscription/storage profile.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<OrganizationDto>>> Get(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<OrganizationDto>.Ok(await service.GetAsync(id, ct)));

    /// <summary>Creates an organization and assigns the current user as Owner.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrganizationDto>>> Create(
        CreateOrganizationRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, ApiResponse<OrganizationDto>.Ok(result, "Organization created."));
    }

    /// <summary>Updates organization profile and branding fields.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<OrganizationDto>>> Update(
        Guid id, UpdateOrganizationRequest request, CancellationToken ct) =>
        Ok(ApiResponse<OrganizationDto>.Ok(await service.UpdateAsync(id, request, ct), "Organization updated."));

    /// <summary>Soft-deletes an organization.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Organization archived."));
    }

    [HttpGet("{id:guid}/legacy-members")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<OrganizationMemberDto>>>> Members(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<OrganizationMemberDto>>.Ok(await service.ListMembersAsync(id, ct)));

    /// <summary>Invites a team member. The returned token is intended for an email delivery adapter.</summary>
    [HttpPost("{id:guid}/legacy-members/invite")]
    public async Task<ActionResult<ApiResponse<InvitationResponse>>> Invite(
        Guid id, InviteMemberRequest request, CancellationToken ct) =>
        Ok(ApiResponse<InvitationResponse>.Ok(await service.InviteAsync(id, request, ct), "Invitation created."));

    [HttpPost("invitations/accept")]
    public async Task<ActionResult<ApiResponse<object>>> Accept(AcceptInvitationRequest request, CancellationToken ct)
    {
        await service.AcceptInvitationAsync(request.Token, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Invitation accepted."));
    }

    [HttpDelete("{id:guid}/legacy-members/{memberId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Remove(Guid id, Guid memberId, CancellationToken ct)
    {
        await service.RemoveMemberAsync(id, memberId, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Member removed."));
    }

    [HttpPut("{id:guid}/legacy-members/{memberId:guid}/role")]
    public async Task<ActionResult<ApiResponse<object>>> ChangeRole(
        Guid id, Guid memberId, ChangeMemberRoleRequest request, CancellationToken ct)
    {
        await service.ChangeRoleAsync(id, memberId, request.Role, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Member role updated."));
    }
}
