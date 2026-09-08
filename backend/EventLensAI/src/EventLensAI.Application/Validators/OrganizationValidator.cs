using EventLensAI.Application.DTOs.Organizations;
using FluentValidation;

namespace EventLensAI.Application.Validators;

public sealed class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest>
{
    public CreateOrganizationRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Website).Must(value => string.IsNullOrWhiteSpace(value) ||
            Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https");
        RuleFor(x => x.PrimaryColor).Matches("^#[0-9A-Fa-f]{6}$");
        RuleFor(x => x.SecondaryColor).Matches("^#[0-9A-Fa-f]{6}$");
        RuleFor(x => x.TimeZone).NotEmpty().MaximumLength(100);
    }
}
public sealed class UpdateOrganizationRequestValidator : AbstractValidator<UpdateOrganizationRequest>
{
    public UpdateOrganizationRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Website).Must(value => string.IsNullOrWhiteSpace(value) ||
            Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https");
        RuleFor(x => x.PrimaryColor).Matches("^#[0-9A-Fa-f]{6}$");
        RuleFor(x => x.SecondaryColor).Matches("^#[0-9A-Fa-f]{6}$");
        RuleFor(x => x.TimeZone).NotEmpty().MaximumLength(100);
    }
}
public sealed class InviteMemberRequestValidator:AbstractValidator<InviteMemberRequest>
{
    public InviteMemberRequestValidator(){RuleFor(x=>x.Email).NotEmpty().EmailAddress().MaximumLength(320);RuleFor(x=>x.Role).NotEmpty().MaximumLength(50);}
}
public sealed class AddOrganizationMemberRequestValidator:AbstractValidator<AddOrganizationMemberRequest>
{
    public AddOrganizationMemberRequestValidator(){RuleFor(x=>x.Email).NotEmpty().EmailAddress().MaximumLength(320);RuleFor(x=>x.Role).NotEmpty().MaximumLength(50);}
}
public sealed class ChangeMemberRoleRequestValidator:AbstractValidator<ChangeMemberRoleRequest>
{
    public ChangeMemberRoleRequestValidator(){RuleFor(x=>x.Role).NotEmpty().MaximumLength(50);}
}
public sealed class SetMemberPermissionRequestValidator:AbstractValidator<SetMemberPermissionRequest>
{public SetMemberPermissionRequestValidator()=>RuleFor(x=>x.PermissionKey).NotEmpty().MaximumLength(100);}
public sealed class CreateOwnershipTransferRequestValidator:AbstractValidator<CreateOwnershipTransferRequest>
{public CreateOwnershipTransferRequestValidator()=>RuleFor(x=>x.NewOwnerUserId).NotEmpty();}
public sealed class CreateDepartmentRequestValidator:AbstractValidator<CreateDepartmentRequest>
{public CreateDepartmentRequestValidator(){RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);RuleFor(x=>x.Description).MaximumLength(2000);}}
public sealed class UpdateDepartmentRequestValidator:AbstractValidator<UpdateDepartmentRequest>
{public UpdateDepartmentRequestValidator(){RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);RuleFor(x=>x.Description).MaximumLength(2000);RuleFor(x=>x.Status).IsInEnum().NotEqual(Domain.Enums.OrganizationalUnitStatus.Archived);}}
public sealed class CreateBranchRequestValidator:AbstractValidator<CreateBranchRequest>
{public CreateBranchRequestValidator(){RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);RuleFor(x=>x.Code).NotEmpty().MaximumLength(50).Matches("^[A-Za-z0-9_-]+$");RuleFor(x=>x.Timezone).NotEmpty().MaximumLength(100);RuleFor(x=>x.Email).EmailAddress().When(x=>!string.IsNullOrWhiteSpace(x.Email));}}
public sealed class UpdateBranchRequestValidator:AbstractValidator<UpdateBranchRequest>
{public UpdateBranchRequestValidator(){RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);RuleFor(x=>x.Code).NotEmpty().MaximumLength(50).Matches("^[A-Za-z0-9_-]+$");RuleFor(x=>x.Timezone).NotEmpty().MaximumLength(100);RuleFor(x=>x.Email).EmailAddress().When(x=>!string.IsNullOrWhiteSpace(x.Email));RuleFor(x=>x.Status).IsInEnum().NotEqual(Domain.Enums.OrganizationalUnitStatus.Archived);}}
public sealed class AssignUnitMemberRequestValidator:AbstractValidator<AssignUnitMemberRequest>
{public AssignUnitMemberRequestValidator()=>RuleFor(x=>x.UserId).NotEmpty();}
public sealed class UpdateBrandKitRequestValidator:AbstractValidator<UpdateBrandKitRequest>
{public UpdateBrandKitRequestValidator(){var hex="^#[0-9A-Fa-f]{6}$";RuleFor(x=>x.PrimaryColor).Matches(hex);RuleFor(x=>x.SecondaryColor).Matches(hex);RuleFor(x=>x.AccentColor).Matches(hex);RuleFor(x=>x.BackgroundColor).Matches(hex);RuleFor(x=>x.SurfaceColor).Matches(hex);RuleFor(x=>x.TextColor).Matches(hex);RuleFor(x=>x.SuccessColor).Matches(hex);RuleFor(x=>x.WarningColor).Matches(hex);RuleFor(x=>x.DangerColor).Matches(hex);RuleFor(x=>x.PrimaryFont).NotEmpty().MaximumLength(100);RuleFor(x=>x.SecondaryFont).NotEmpty().MaximumLength(100);RuleFor(x=>x.HeadingFont).NotEmpty().MaximumLength(100);RuleFor(x=>x.BodyFont).NotEmpty().MaximumLength(100);RuleFor(x=>x.FontScale).InclusiveBetween(.5m,2m);RuleFor(x=>x.Watermark.Opacity).InclusiveBetween(0,1);RuleFor(x=>x.Watermark.Scale).InclusiveBetween(.01m,1m);RuleFor(x=>x.Qr.PrimaryColor).Matches(hex);RuleFor(x=>x.Qr.BackgroundColor).Matches(hex);RuleFor(x=>x.Qr.DefaultSize).InclusiveBetween(128,4096);RuleFor(x=>x.Email.PrimaryColor).Matches(hex);}}
public sealed class CreateBrandThemeRequestValidator:AbstractValidator<CreateBrandThemeRequest>
{public CreateBrandThemeRequestValidator(){RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);RuleFor(x=>x.Description).MaximumLength(1000);RuleFor(x=>x.ConfigurationJson).NotEmpty().Must(x=>{try{System.Text.Json.JsonDocument.Parse(x).Dispose();return true;}catch{return false;}}).WithMessage("Configuration must be valid JSON.");}}
public sealed class UpdateBrandThemeRequestValidator:AbstractValidator<UpdateBrandThemeRequest>
{public UpdateBrandThemeRequestValidator(){RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);RuleFor(x=>x.Description).MaximumLength(1000);RuleFor(x=>x.ConfigurationJson).NotEmpty();}}
public sealed class DuplicateBrandThemeRequestValidator:AbstractValidator<DuplicateBrandThemeRequest>
{public DuplicateBrandThemeRequestValidator()=>RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);}
