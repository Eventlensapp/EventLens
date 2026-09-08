using EventLensAI.Application.DTOs.Booth;
using FluentValidation;

namespace EventLensAI.Application.Validators;

public sealed class StartBoothSessionRequestValidator : AbstractValidator<StartBoothSessionRequest>
{
    public StartBoothSessionRequestValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
        RuleFor(x => x.Countdown).Must(x => x is 3 or 5 or 10);
        RuleFor(x => x.CaptureMode).IsInEnum();
    }
}

public sealed class BoothSessionCreateEnvelopeValidator:AbstractValidator<BoothSessionCreateEnvelope>
{
    public BoothSessionCreateEnvelopeValidator()
    {
        RuleFor(x=>x.EventId).NotEmpty();RuleFor(x=>x.GuestName).MaximumLength(120);RuleFor(x=>x.GuestEmail).EmailAddress().MaximumLength(320).When(x=>!string.IsNullOrWhiteSpace(x.GuestEmail));
        RuleFor(x=>x.DeviceInformation).MaximumLength(1000);RuleFor(x=>x.MetadataJson).MaximumLength(4000);
        When(x=>x.OrganizationId.HasValue,()=>{RuleFor(x=>x.OrganizationId).NotEmpty();RuleFor(x=>x.BoothId).NotEmpty();RuleFor(x=>x.SessionType).NotNull();});
        When(x=>!x.OrganizationId.HasValue,()=>{RuleFor(x=>x.Countdown).NotNull().InclusiveBetween(3,10);RuleFor(x=>x.CaptureMode).NotNull();});
    }
}
public sealed class RecordCaptureRequestValidator : AbstractValidator<RecordCaptureRequest>
{
    public RecordCaptureRequestValidator() => RuleFor(x => x.PhotoCount).InclusiveBetween(1, 240);
}
