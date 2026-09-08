using FluentValidation;
namespace EventLensAI.Application.Features.AI;
public sealed class CreateAIJobRequestValidator:AbstractValidator<CreateAIJobRequest>
{
    public CreateAIJobRequestValidator(){RuleFor(x=>x.EventId).NotEmpty();RuleFor(x=>x.InputImage).NotEmpty().MaximumLength(2048);RuleFor(x=>x.Prompt).MaximumLength(4000);RuleFor(x=>x.NegativePrompt).MaximumLength(2000);RuleFor(x=>x.UpscaleFactor).Must(x=>x is null or 2 or 4 or 8);}
}
