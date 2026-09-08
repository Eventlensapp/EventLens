using FluentValidation;
namespace EventLensAI.Application.Features.Booth;
public sealed class UpdateCaptureConfigurationRequestValidator:AbstractValidator<UpdateCaptureConfigurationRequest>
{
 public UpdateCaptureConfigurationRequestValidator(){RuleFor(x=>x.OrganizationId).NotEmpty();RuleFor(x=>x.EventId).NotEmpty();RuleFor(x=>x.CountdownDuration).InclusiveBetween(0,30);RuleFor(x=>x.NumberOfPhotos).InclusiveBetween(1,20);RuleFor(x=>x.CaptureInterval).InclusiveBetween(0,30);RuleFor(x=>x.ImageQuality).InclusiveBetween(.1m,1m);RuleFor(x=>x.Resolution).Matches(@"^\d{3,5}x\d{3,5}$");}
}
public sealed class StartCaptureRequestValidator:AbstractValidator<StartCaptureRequest>
{
 public StartCaptureRequestValidator(){RuleFor(x=>x.OrganizationId).NotEmpty();RuleFor(x=>x.EventId).NotEmpty();RuleFor(x=>x.SessionId).NotEmpty();RuleFor(x=>x.CaptureNumber).GreaterThan(0);RuleFor(x=>x.FileName).NotEmpty().MaximumLength(255);RuleFor(x=>x.LocalStorageKey).NotEmpty().MaximumLength(500).Must(x=>x.StartsWith("capture:")).WithMessage("Only a private local capture key is accepted.");RuleFor(x=>x.Width).InclusiveBetween(1,16384);RuleFor(x=>x.Height).InclusiveBetween(1,16384);RuleFor(x=>x.FileSize).InclusiveBetween(1,100*1024*1024);RuleFor(x=>x.MimeType).Equal("image/jpeg");}
}
