using EventLensAI.Application.DTOs.Organizations;
using EventLensAI.Application.DTOs.Storage;
using EventLensAI.Application.Features.Entitlements;
using EventLensAI.Application.Validators;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;

namespace EventLensAI.UnitTests;

public sealed class OrganizationModuleTests
{
    [Fact]
    public async Task Validator_accepts_a_valid_organization()
    {
        var request = new CreateOrganizationRequest(
            "EventLens Studio", null, null, "https://eventlens.example",
            "owner@example.com", null, null, "NP", "Asia/Kathmandu",
            "#111111", "#FFFFFF");

        var result = await new CreateOrganizationRequestValidator().ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Creator_can_own_multiple_organizations()
    {
        var userId = Guid.NewGuid();
        var first = new Organization("Wedding Studio", "wedding-studio", userId);
        var second = new Organization("Corporate Events", "corporate-events", userId);
        var memberships = new[]
        {
            new OrganizationMember(first.Id, userId, SystemRoles.OwnerId),
            new OrganizationMember(second.Id, userId, SystemRoles.OwnerId)
        };
        Assert.Equal(2, memberships.Select(x => x.OrganizationId).Distinct().Count());
    }

    [Fact]
    public void Organization_can_set_type_and_archive_without_hard_delete()
    {
        var organization = new Organization("Studio", "studio", Guid.NewGuid());
        organization.SetType(OrganizationType.PhotographyStudio);
        organization.Archive();
        Assert.Equal(OrganizationType.PhotographyStudio, organization.OrganizationType);
        Assert.Equal(OrganizationStatus.Archived, organization.Status);
        Assert.False(organization.IsActive);
        Assert.False(organization.IsDeleted);
    }

    [Fact]
    public void Invitation_can_be_cancelled_and_not_accepted_afterwards()
    {
        var invitation = new OrganizationInvitation(Guid.NewGuid(), "member@example.com",
            SystemRoles.PhotographerId, "hash", Guid.NewGuid());
        invitation.Cancel();
        Assert.Equal(OrganizationInvitationStatus.Cancelled, invitation.Status);
        Assert.False(invitation.IsValid);
        Assert.Throws<InvalidOperationException>(() => invitation.Accept());
    }

    [Fact]
    public void Team_roles_include_specialized_organization_roles()
    {
        Assert.NotEqual(Guid.Empty, SystemRoles.BoothOperatorId);
        Assert.NotEqual(Guid.Empty, SystemRoles.DesignerId);
        Assert.NotEqual(Guid.Empty, SystemRoles.MarketingManagerId);
    }

    [Fact]
    public void Ownership_transfer_requires_acceptance_and_is_single_use()
    {
        var transfer = new OrganizationOwnershipTransfer(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(2));
        Assert.True(transfer.IsValid);
        transfer.Accept();
        Assert.Equal(OwnershipTransferStatus.Accepted, transfer.Status);
        Assert.NotNull(transfer.AcceptedAt);
        Assert.Throws<InvalidOperationException>(() => transfer.Accept());
    }

    [Fact]
    public void Member_permission_override_can_be_changed()
    {
        var value = new OrganizationMemberPermission(Guid.NewGuid(), SystemPermissions.All[2].Id, false);
        value.Set(true);
        Assert.True(value.Allowed);
    }

    [Fact]
    public void Department_supports_update_and_soft_archive()
    {
        var actor = Guid.NewGuid();
        var department = new Department(Guid.NewGuid(), "Photography", "Capture team", null);
        department.Update("Creative Photography", null, actor, OrganizationalUnitStatus.Inactive);
        Assert.Equal("Creative Photography", department.Name);
        Assert.Equal(OrganizationalUnitStatus.Inactive, department.Status);
        department.Archive(actor);
        Assert.True(department.IsDeleted);
        Assert.Equal(OrganizationalUnitStatus.Archived, department.Status);
    }

    [Fact]
    public void Branch_normalizes_code_and_supports_soft_archive()
    {
        var actor = Guid.NewGuid();
        var branch = new Branch(Guid.NewGuid(), "Kathmandu", "ktm");
        branch.Update("Kathmandu Office", "ktm-01", "Durbar Marg", "Kathmandu", "Nepal",
            "Asia/Kathmandu", null, "ktm@example.com", actor, OrganizationalUnitStatus.Active);
        Assert.Equal("KTM-01", branch.Code);
        branch.Archive(actor);
        Assert.True(branch.IsDeleted);
        Assert.Equal(OrganizationalUnitStatus.Archived, branch.Status);
    }

    [Fact]
    public async Task Structure_validators_reject_invalid_names_codes_and_members()
    {
        Assert.False((await new CreateDepartmentRequestValidator().ValidateAsync(new CreateDepartmentRequest("", null, null))).IsValid);
        Assert.False((await new CreateBranchRequestValidator().ValidateAsync(
            new CreateBranchRequest("London", "invalid code!", null, null, null, "Europe/London", null, null, null))).IsValid);
        Assert.False((await new AssignUnitMemberRequestValidator().ValidateAsync(new AssignUnitMemberRequest(Guid.Empty))).IsValid);
    }

    [Fact]
    public void Brand_kit_is_the_single_tenant_branding_source()
    {
        var kit = new BrandKit(Guid.NewGuid());
        kit.SetLogo(BrandLogoType.Primary, "logos/primary.webp");
        kit.UpdateColors("#111111","#222222","#333333","#FFFFFF","#F5F5F5","#101010","#00AA00","#FFAA00","#CC0000");
        kit.UpdateTypography("Inter","Roboto","Montserrat","Inter",1.1m);
        Assert.Equal("logos/primary.webp",kit.PrimaryLogo);
        Assert.Equal("#333333",kit.AccentColor);
        Assert.Equal(1.1m,kit.FontScale);
    }

    [Fact]
    public void Brand_theme_can_duplicate_and_activate()
    {
        var source=new BrandTheme(Guid.NewGuid(),"Luxury","Gold theme",BrandThemeKind.Theme,"{\"style\":\"luxury\"}");
        var copy=source.Duplicate("Luxury Copy");
        copy.Activate();
        Assert.Equal("Luxury Copy",copy.Name);
        Assert.True(copy.IsActive);
        Assert.True(copy.IsDefault);
    }

    [Fact]
    public async Task Brand_kit_validator_rejects_invalid_colors_and_scale()
    {
        var request=new UpdateBrandKitRequest("red","#222222","#333333","#FFFFFF","#F5F5F5","#101010","#00AA00","#FFAA00","#CC0000",
            "Inter","Inter","Inter","Inter",3m,new(false,null,.5m,"BottomRight",.2m,10),
            new("#000000","#FFFFFF",true,null,false,"M",512),new("Studio",null,"#000000",null,null,null));
        Assert.False((await new UpdateBrandKitRequestValidator().ValidateAsync(request)).IsValid);
    }

    [Fact]
    public void Storage_file_supports_metadata_copy_archive_and_restore()
    {
        var file=new StorageFile(Guid.NewGuid(),null,Guid.NewGuid(),"Portrait","portrait.jpg",".jpg","image/jpeg",1024,"key","ABC",Guid.NewGuid(),StorageVisibility.Organization);
        file.MarkClean();file.Favourite(true);file.SetDetails("Best photo",StorageVisibility.Private);
        var copy=file.Copy(Guid.NewGuid(),Guid.NewGuid());
        file.SoftDelete(Guid.NewGuid());file.Restore(Guid.NewGuid());
        Assert.Equal(StorageScanStatus.Clean,file.ScanStatus);Assert.True(file.IsFavourite);Assert.False(file.IsDeleted);
        Assert.Equal(2,copy.Version);Assert.Equal(file.Id,copy.PreviousVersionId);
    }

    [Fact]
    public void Storage_usage_tracks_quota_without_becoming_negative()
    {
        var usage=new StorageUsage(Guid.NewGuid(),10_000);usage.Add(4_000);usage.Add(-4_000);usage.Add(-100);
        Assert.Equal(0,usage.UsedBytes);Assert.Equal(0,usage.FileCount);
    }

    [Fact]
    public async Task Storage_validators_reject_invalid_folder_and_page()
    {
        Assert.False((await new CreateStorageFolderRequestValidator().ValidateAsync(new CreateStorageFolderRequest("",null))).IsValid);
        Assert.False((await new StorageSearchRequestValidator().ValidateAsync(new StorageSearchRequest(Guid.Empty,null,null,null,null,null,null,null,null,false,0,500))).IsValid);
    }

    [Fact]
    public void Subscription_trial_expires_and_plan_can_change()
    {
        var subscription=new OrganizationSubscription(Guid.NewGuid(),Guid.NewGuid(),DateTime.UtcNow);
        subscription.StartTrial(Guid.NewGuid(),14);
        Assert.Equal(EntitlementSubscriptionStatus.Trial,subscription.Status);
        subscription.Evaluate(DateTime.UtcNow.AddDays(15));
        Assert.Equal(EntitlementSubscriptionStatus.Expired,subscription.Status);
    }

    [Fact]
    public void Entitlement_plan_supports_limits_and_archive()
    {
        var plan=new EntitlementPlan(Guid.NewGuid(),SubscriptionPlan.Professional,"Professional",49,490,"Studio plan",3);
        plan.SetLimits(100,10,200_000,100,1000,250,25000,50000,100000);
        Assert.Equal(100,plan.MaximumEvents);
        plan.Archive();
        Assert.False(plan.IsActive);
    }

    [Fact]
    public async Task Entitlement_validators_reject_bad_plan_and_usage()
    {
        var plan=new SaveEntitlementPlanRequest(SubscriptionPlan.Creator,"",-1,-1,"",0,new Dictionary<string,bool>(),0,0,-2,0,0,0,0,0,0);
        Assert.False((await new SaveEntitlementPlanRequestValidator().ValidateAsync(plan)).IsValid);
        Assert.False((await new TrackUsageRequestValidator().ValidateAsync(new TrackUsageRequest(UsageMetric.ApiRequests,0,null))).IsValid);
    }

    [Fact]
    public async Task Validator_rejects_an_invalid_website_and_color()
    {
        var request = new CreateOrganizationRequest(
            "EventLens Studio", null, null, "javascript:alert(1)",
            null, null, null, null, "UTC", "#123", "#FFFFFF");

        var result = await new CreateOrganizationRequestValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Website");
        Assert.Contains(result.Errors, x => x.PropertyName == "PrimaryColor");
    }
}
