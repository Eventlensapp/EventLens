using EventLensAI.Application.DTOs.Organizations;
using EventLensAI.Application.Exceptions;
using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Application.Interfaces.Persistence;
using EventLensAI.Application.Interfaces.Services;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Application.Services;

public sealed class OrganizationService(
    IOrganizationRepository organizations, IAuditRepository audits,
    IUserRepository users, ITokenService tokens, IPasswordService passwords, IEmailService emailService,
    ICurrentUserService current, IUnitOfWork unitOfWork) : IOrganizationService
{
    public async Task<IReadOnlyList<OrganizationDto>> ListAsync(CancellationToken ct)
    {
        var userId = RequireUser();
        return (await organizations.ListForUserAsync(userId, ct)).Select(Map).ToArray();
    }
    public async Task<OrganizationDto> GetAsync(Guid id, CancellationToken ct)
    {
        await RequireMember(id, ct);
        return Map(await organizations.GetAsync(id, ct) ?? throw new NotFoundException("Organization not found."));
    }
    public async Task<OrganizationDto> CreateAsync(CreateOrganizationRequest request, CancellationToken ct)
    {
        var userId = RequireUser(); var slug = await UniqueSlug(request.Name, ct);
        var entity = new Organization(request.Name, slug, userId);
        entity.Update(request.Name, slug, request.Description, request.Logo, request.Website,
            request.Email, request.Phone, request.Address, request.TimeZone, request.PrimaryColor, request.SecondaryColor);
        entity.SetCountry(request.Country);
        entity.SetType(request.OrganizationType);
        await organizations.AddAsync(entity, ct);
        await organizations.AddMemberAsync(new OrganizationMember(entity.Id, userId, SystemRoles.OwnerId), ct);
        await Audit(entity.Id, AuditAction.Create, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Map(entity);
    }
    public async Task<OrganizationDto> UpdateAsync(Guid id, UpdateOrganizationRequest request, CancellationToken ct)
    {
        await RequireRole(id, [SystemRoles.Owner], ct);
        var entity = await organizations.GetAsync(id, ct) ?? throw new NotFoundException("Organization not found.");
        entity.Update(request.Name, entity.Slug, request.Description, request.Logo, request.Website,
            request.Email, request.Phone, request.Address, request.TimeZone, request.PrimaryColor, request.SecondaryColor);
        entity.SetCountry(request.Country); entity.SetType(request.OrganizationType); entity.MarkUpdated(RequireUser());
        await Audit(id, AuditAction.Update, ct); await unitOfWork.SaveChangesAsync(ct);
        return Map(entity);
    }
    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await RequireRole(id, [SystemRoles.Owner], ct);
        var entity = await organizations.GetAsync(id, ct) ?? throw new NotFoundException("Organization not found.");
        entity.Archive(); entity.SoftDelete(RequireUser()); await Audit(id, AuditAction.Archive, ct); await unitOfWork.SaveChangesAsync(ct);
    }
    public async Task<IReadOnlyList<OrganizationMemberDto>> ListMembersAsync(Guid id, CancellationToken ct)
    {
        await RequireMember(id, ct);
        return (await organizations.ListMembersAsync(id, ct))
            .Select(MapMember).ToArray();
    }
    public async Task<InvitationResponse> InviteAsync(Guid id, InviteMemberRequest request, CancellationToken ct)
    {
        await RequireRole(id, [SystemRoles.Owner, SystemRoles.Manager], ct);
        var role = await AllowedRole(request.Role, ct);
        var raw = tokens.CreateRefreshToken();
        var invitation = new OrganizationInvitation(id, request.Email, role.Id, tokens.HashRefreshToken(raw), RequireUser());
        await organizations.AddInvitationAsync(invitation, ct);
        await Audit(id, AuditAction.Invite, ct); await unitOfWork.SaveChangesAsync(ct);
        await emailService.SendOrganizationInvitationAsync(invitation.Email,
            (await organizations.GetAsync(id,ct))!.Name,current.Email??"An organization administrator",
            $"/invitations/{raw}",ct);
        return new(invitation.Id, invitation.Email, role.Name, invitation.ExpiresAt, raw);
    }
    public async Task AcceptInvitationAsync(string token, CancellationToken ct)
    {
        var invitation = await organizations.GetInvitationByHashAsync(tokens.HashRefreshToken(token), ct)
            ?? throw new NotFoundException("Invitation not found.");
        if (!invitation.IsValid) throw new ConflictException("Invitation is expired or already accepted.");
        if (!string.Equals(invitation.Email, current.Email, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedException("Invitation belongs to a different email address.");
        if (await organizations.GetMemberAsync(invitation.OrganizationId, RequireUser(), ct) is null)
            await organizations.AddMemberAsync(new OrganizationMember(invitation.OrganizationId, RequireUser(), invitation.RoleId, invitation.InvitedBy), ct);
        invitation.Accept(); await unitOfWork.SaveChangesAsync(ct);
    }
    public async Task RemoveMemberAsync(Guid id, Guid memberId, CancellationToken ct)
    {
        await RequireRole(id, [SystemRoles.Owner], ct);
        var member = await organizations.GetMemberByIdAsync(id, memberId, ct)
            ?? throw new NotFoundException("Member not found.");
        if (member.UserId == RequireUser()) throw new ConflictException("Owners cannot remove themselves.");
        member.SoftDelete(RequireUser()); await Audit(id, AuditAction.RemoveMember, ct); await unitOfWork.SaveChangesAsync(ct);
    }
    public async Task ChangeRoleAsync(Guid id, Guid memberId, string roleName, CancellationToken ct)
    {
        await RequireRole(id, [SystemRoles.Owner, SystemRoles.Manager], ct);
        var member = await organizations.GetMemberByIdAsync(id, memberId, ct)
            ?? throw new NotFoundException("Member not found.");
        var role = await AllowedRole(roleName, ct);
        if(member.Role.Name==SystemRoles.Owner||role.Name==SystemRoles.Owner)
            await RequireRole(id,[SystemRoles.Owner],ct);
        member.ChangeRole(role.Id); member.MarkUpdated(RequireUser());
        await Audit(id, AuditAction.ChangeRole, ct); await unitOfWork.SaveChangesAsync(ct);
    }
    public async Task<OrganizationMemberDto> AddMemberAsync(Guid id,AddOrganizationMemberRequest request,CancellationToken ct)
    {
        await RequireRole(id,[SystemRoles.Owner,SystemRoles.Manager],ct);
        var user=await users.GetByNormalizedEmailAsync(request.Email.Trim().ToUpperInvariant(),ct)
            ?? throw new NotFoundException("Registered user not found.");
        if(await organizations.GetMemberAsync(id,user.Id,ct) is not null) throw new ConflictException("User is already a member.");
        var role=await AllowedRole(request.Role,ct);
        if(role.Name==SystemRoles.Owner) await RequireRole(id,[SystemRoles.Owner],ct);
        var member=new OrganizationMember(id,user.Id,role.Id,RequireUser());await organizations.AddMemberAsync(member,ct);
        await Audit(id,AuditAction.Invite,ct);await unitOfWork.SaveChangesAsync(ct);
        return new(member.Id,user.Id,$"{user.FirstName} {user.LastName}",user.Email,role.Name,member.JoinedAt,member.Status,user.LastLoginAt);
    }
    public async Task<OrganizationMemberDto> CreateMemberAsync(Guid id,CreateOrganizationMemberRequest request,CancellationToken ct)
    {
        await RequireRole(id,[SystemRoles.Owner,SystemRoles.Manager],ct);
        if(await users.GetByNormalizedEmailAsync(request.Email.Trim().ToUpperInvariant(),ct) is not null) throw new ConflictException("An account with this email already exists.");
        var role=await AllowedRole(request.Role,ct);
        if(role.Name==SystemRoles.Owner) await RequireRole(id,[SystemRoles.Owner],ct);
        var user=new User(request.FirstName,request.LastName,request.Email,passwords.Hash(request.TemporaryPassword),null);
        user.SetTemporaryPassword(user.PasswordHash);await users.AddUserAsync(user,ct);
        var member=new OrganizationMember(id,user.Id,role.Id,RequireUser());await organizations.AddMemberAsync(member,ct);
        await users.AddActivityAsync(new(user.Id,"account.created-by-admin","Account created with a temporary password.",current.IPAddress,null),ct);
        await Audit(id,AuditAction.Invite,ct);await unitOfWork.SaveChangesAsync(ct);
        return new(member.Id,user.Id,$"{user.FirstName} {user.LastName}",user.Email,role.Name,member.JoinedAt,member.Status,null);
    }
    public async Task RemoveMemberByUserAsync(Guid id,Guid userId,CancellationToken ct)
    {
        await RequireRole(id,[SystemRoles.Owner,SystemRoles.Manager],ct);
        var member=await organizations.GetMemberAsync(id,userId,ct)??throw new NotFoundException("Member not found.");
        if(member.Role.Name==SystemRoles.Owner) throw new ConflictException("Owner membership requires ownership transfer.");
        member.SoftDelete(RequireUser());await Audit(id,AuditAction.RemoveMember,ct);await unitOfWork.SaveChangesAsync(ct);
    }
    public async Task ChangeRoleByUserAsync(Guid id,Guid userId,string role,CancellationToken ct)
    {
        var member=await organizations.GetMemberAsync(id,userId,ct)??throw new NotFoundException("Member not found.");
        await ChangeRoleAsync(id,member.Id,role,ct);
    }
    public async Task<IReadOnlyList<InvitationDto>> ListInvitationsAsync(Guid id,CancellationToken ct)
    {await RequireRole(id,[SystemRoles.Owner,SystemRoles.Manager],ct);return (await organizations.ListInvitationsAsync(id,ct)).Select(x=>MapInvitation(x,false)).ToArray();}
    public async Task<InvitationDto> GetInvitationAsync(string token,CancellationToken ct)
    {var x=await organizations.GetInvitationByHashAsync(tokens.HashRefreshToken(token),ct)??throw new NotFoundException("Invitation not found.");var exists=await users.GetByNormalizedEmailAsync(x.Email.ToUpperInvariant(),ct)is not null;return MapInvitation(x,exists);}
    public async Task CancelInvitationAsync(Guid invitationId,CancellationToken ct)
    {var invitation=await organizations.GetInvitationAsync(invitationId,ct)??throw new NotFoundException("Invitation not found.");await RequireRole(invitation.OrganizationId,[SystemRoles.Owner,SystemRoles.Manager],ct);invitation.Cancel();await unitOfWork.SaveChangesAsync(ct);}
    private async Task<Role> AllowedRole(string name, CancellationToken ct)
    {
        var allowed = new[] { SystemRoles.Owner, SystemRoles.Manager, SystemRoles.Photographer, SystemRoles.BoothOperator, SystemRoles.Designer, SystemRoles.MarketingManager, SystemRoles.Viewer };
        if (!allowed.Contains(name, StringComparer.OrdinalIgnoreCase)) throw new ConflictException("Role is invalid.");
        return await organizations.GetRoleAsync(name, ct) ?? throw new NotFoundException("Role not found.");
    }
    private async Task<OrganizationMember> RequireMember(Guid id, CancellationToken ct)
    {
        if (current.Roles.Contains(SystemRoles.SuperAdmin)) return new OrganizationMember(id, RequireUser(), SystemRoles.SuperAdminId);
        return await organizations.GetMemberAsync(id, RequireUser(), ct) ?? throw new UnauthorizedException("Organization access denied.");
    }
    private async Task RequireRole(Guid id, string[] roles, CancellationToken ct)
    {
        if (current.Roles.Contains(SystemRoles.SuperAdmin)) return;
        var member = await RequireMember(id, ct);
        if (!roles.Contains(member.Role.Name)) throw new UnauthorizedException("Organization permission denied.");
    }
    private async Task<string> UniqueSlug(string name, CancellationToken ct)
    {
        var baseSlug = Slug(name); var slug = baseSlug; var suffix = 2;
        while (await organizations.SlugExistsAsync(slug, null, ct)) slug = $"{baseSlug}-{suffix++}";
        return slug;
    }
    internal static string Slug(string value)
    {
        var chars = value.Trim().ToLowerInvariant().Select(c => char.IsLetterOrDigit(c) ? c : '-').ToArray();
        return string.Join('-', new string(chars).Split('-', StringSplitOptions.RemoveEmptyEntries));
    }
    private async Task Audit(Guid resourceId, AuditAction action, CancellationToken ct) =>
        await audits.AddAsync(new AuditLog(RequireUser(), nameof(Organization), resourceId, action, current.IPAddress), ct);
    private Guid RequireUser() => current.UserId ?? throw new UnauthorizedException("Authentication is required.");
    private static OrganizationDto Map(Organization x) => new(x.Id, x.Name, x.Slug, x.Description, x.Logo, x.Website,
        x.Email, x.Phone, x.Address, x.Country, x.TimeZone, x.PrimaryColor, x.SecondaryColor, x.Plan,
        x.StorageUsed, x.StorageLimit, x.IsActive, x.CreatedAt, x.UpdatedAt, x.OrganizationType, x.Status);
    private static OrganizationMemberDto MapMember(OrganizationMember x)=>new(x.Id,x.UserId,$"{x.User.FirstName} {x.User.LastName}",x.User.Email,x.Role.Name,x.JoinedAt,x.Status,x.User.LastLoginAt);
    private static InvitationDto MapInvitation(OrganizationInvitation x,bool exists)=>new(x.Id,x.OrganizationId,x.Organization.Name,x.Email,x.Role.Name,
        x.Status==OrganizationInvitationStatus.Pending&&x.ExpiresAt<=DateTime.UtcNow?OrganizationInvitationStatus.Expired:x.Status,x.ExpiresAt,x.AcceptedAt,exists);
}
