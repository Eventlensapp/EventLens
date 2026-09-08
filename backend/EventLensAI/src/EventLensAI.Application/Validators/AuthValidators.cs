using EventLensAI.Application.DTOs.Auth;
using FluentValidation;

namespace EventLensAI.Application.Validators;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.Password)
            .NotEmpty().MinimumLength(12).MaximumLength(128)
            .Matches("[A-Z]").WithMessage("Password requires an uppercase letter.")
            .Matches("[a-z]").WithMessage("Password requires a lowercase letter.")
            .Matches("[0-9]").WithMessage("Password requires a number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password requires a special character.");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.AcceptTerms).Equal(true).WithMessage("You must accept the terms and privacy policy.");
    }
}

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(128);
    }
}

public sealed class EmailRequestValidator:AbstractValidator<EmailRequest>{public EmailRequestValidator()=>RuleFor(x=>x.Email).NotEmpty().EmailAddress().MaximumLength(320);}
public sealed class TokenRequestValidator:AbstractValidator<TokenRequest>{public TokenRequestValidator()=>RuleFor(x=>x.Token).NotEmpty().MaximumLength(512);}
public sealed class ResetPasswordRequestValidator:AbstractValidator<ResetPasswordRequest>{public ResetPasswordRequestValidator(){RuleFor(x=>x.Token).NotEmpty().MaximumLength(512);RuleFor(x=>x.Password).NotEmpty().MinimumLength(12).MaximumLength(128).Matches("[A-Z]").Matches("[a-z]").Matches("[0-9]").Matches("[^a-zA-Z0-9]");RuleFor(x=>x.ConfirmPassword).Equal(x=>x.Password);}}
public sealed class UpdateProfileRequestValidator:AbstractValidator<UpdateProfileRequest>{public UpdateProfileRequestValidator(){RuleFor(x=>x.FirstName).NotEmpty().MaximumLength(100);RuleFor(x=>x.LastName).NotEmpty().MaximumLength(100);RuleFor(x=>x.Phone).MaximumLength(30);RuleFor(x=>x.ProfileImage).MaximumLength(2048).Must(x=>string.IsNullOrWhiteSpace(x)||Uri.TryCreate(x,UriKind.Absolute,out _));RuleFor(x=>x.TimeZone).NotEmpty().MaximumLength(100);RuleFor(x=>x.Language).NotEmpty().MaximumLength(10);}}
public sealed class UpdatePreferencesRequestValidator:AbstractValidator<UpdatePreferencesRequest>{public UpdatePreferencesRequestValidator(){RuleFor(x=>x.Theme).Must(x=>x is "system" or "dark" or "light");RuleFor(x=>x.Language).NotEmpty().MaximumLength(10);}}
public sealed class CreateApiKeyRequestValidator:AbstractValidator<CreateApiKeyRequest>{public CreateApiKeyRequestValidator(){RuleFor(x=>x.Name).NotEmpty().MaximumLength(100);RuleFor(x=>x.ExpiresAt).Must(x=>x is null||x>DateTime.UtcNow).WithMessage("Expiration must be in the future.");}}
