using FluentValidation;

namespace EventLensAI.Application.Features.Booth;
public sealed class UpdateBoothConfigurationValidator:AbstractValidator<UpdateBoothConfigurationRequest>
{public UpdateBoothConfigurationValidator(){RuleFor(x=>x.OrganizationId).NotEmpty();RuleFor(x=>x.BoothName).NotEmpty().MaximumLength(120);RuleFor(x=>x.BoothMode).NotEmpty().MaximumLength(40);RuleFor(x=>x.DefaultCamera).MaximumLength(200);RuleFor(x=>x.DefaultResolution).NotEmpty().Matches("^\\d{3,5}x\\d{3,5}$");RuleFor(x=>x.DefaultAspectRatio).NotEmpty().MaximumLength(20);RuleFor(x=>x.Language).NotEmpty().MaximumLength(20);RuleFor(x=>x.Theme).NotEmpty().MaximumLength(40);RuleFor(x=>x.CountdownDefault).InclusiveBetween(0,30);RuleFor(x=>x.CaptureCountDefault).InclusiveBetween(1,20);}}
public sealed class PermissionRequestValidator:AbstractValidator<PermissionRequest>
{public PermissionRequestValidator(){RuleFor(x=>x.OrganizationId).NotEmpty();RuleFor(x=>x.Permission).NotEmpty().Must(x=>new[]{"camera","microphone","notifications","file-system"}.Contains(x,StringComparer.OrdinalIgnoreCase));}}
public sealed class UpdateCameraPreferenceValidator:AbstractValidator<UpdateCameraPreferenceRequest>
{public UpdateCameraPreferenceValidator(){RuleFor(x=>x.OrganizationId).NotEmpty();RuleFor(x=>x.PreferredCameraId).MaximumLength(512);RuleFor(x=>x.PreferredResolution).Must(x=>new[]{"640x480","1280x720","1920x1080","2560x1440","3840x2160"}.Contains(x)).WithMessage("Choose a supported resolution profile.");RuleFor(x=>x.AspectRatio).Must(x=>new[]{"16:9","4:3","1:1"}.Contains(x)).WithMessage("Choose a supported aspect ratio.");}}
public sealed class CreateSessionRequestValidator:AbstractValidator<CreateSessionRequest>
{public CreateSessionRequestValidator(){RuleFor(x=>x.OrganizationId).NotEmpty();RuleFor(x=>x.EventId).NotEmpty();RuleFor(x=>x.BoothId).NotEmpty();RuleFor(x=>x.GuestName).MaximumLength(120);RuleFor(x=>x.GuestEmail).EmailAddress().MaximumLength(320).When(x=>!string.IsNullOrWhiteSpace(x.GuestEmail));RuleFor(x=>x.DeviceInformation).MaximumLength(1000);RuleFor(x=>x.MetadataJson).NotEmpty().MaximumLength(4000);}}
public sealed class RecoverSessionRequestValidator:AbstractValidator<RecoverSessionRequest>
{public RecoverSessionRequestValidator(){RuleFor(x=>x.OrganizationId).NotEmpty();RuleFor(x=>x.SessionToken).NotEmpty().MaximumLength(100);RuleFor(x=>x.TimeoutSeconds).InclusiveBetween(30,3600);}}
