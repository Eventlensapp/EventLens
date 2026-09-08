using FluentValidation;

namespace EventLensAI.Application.Features.Photos.Templates;

public sealed class UpsertTemplateRequestValidator : AbstractValidator<UpsertTemplateRequest>
{
    public UpsertTemplateRequestValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty(); RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).IsInEnum(); RuleFor(x => x.ConfigurationJson).NotEmpty().MaximumLength(1_000_000);
        RuleFor(x => x.PreviewImage).MaximumLength(2048);
    }
}
