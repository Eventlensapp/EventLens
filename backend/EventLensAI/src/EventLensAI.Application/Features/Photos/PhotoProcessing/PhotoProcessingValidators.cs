using FluentValidation;

namespace EventLensAI.Application.Features.Photos.PhotoProcessing;

public sealed class RenderDocumentValidator : AbstractValidator<RenderDocument>
{
    public RenderDocumentValidator()
    {
        RuleFor(x => x.Width).InclusiveBetween(320, 12000);
        RuleFor(x => x.Height).InclusiveBetween(320, 12000);
        RuleFor(x => x.Background).Matches("^#[0-9A-Fa-f]{6}$");
        RuleFor(x => x.BorderColor).Matches("^#[0-9A-Fa-f]{6}$");
        RuleFor(x => x.Padding).InclusiveBetween(0, 1000);
        RuleFor(x => x.Spacing).InclusiveBetween(0, 1000);
        RuleFor(x => x.Layers.Count).LessThanOrEqualTo(100);
    }
}
public sealed class RegisterPhotoRequestValidator : AbstractValidator<RegisterPhotoRequest>
{
    public RegisterPhotoRequestValidator()
    {
        RuleFor(x => x.EventId).NotEmpty(); RuleFor(x => x.OriginalImageUrl).NotEmpty().MaximumLength(2048);
        RuleFor(x => x.Width).InclusiveBetween(1, 20000); RuleFor(x => x.Height).InclusiveBetween(1, 20000);
        RuleFor(x => x.FileSize).InclusiveBetween(1, 25 * 1024 * 1024);
        RuleFor(x => x.Format).Must(x => x is Domain.Enums.PhotoFormat.Jpeg or Domain.Enums.PhotoFormat.Png);
    }
}
