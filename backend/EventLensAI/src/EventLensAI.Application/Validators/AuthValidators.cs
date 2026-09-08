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
