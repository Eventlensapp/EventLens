using EventLensAI.Application.DTOs.Events;
using FluentValidation;

namespace EventLensAI.Application.Validators;

public sealed class CreateChecklistItemValidator : AbstractValidator<CreateChecklistItemRequest>
{
    public CreateChecklistItemValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(240);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}
public sealed class UpdateChecklistItemValidator : AbstractValidator<UpdateChecklistItemRequest>
{
    public UpdateChecklistItemValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(240);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Status).IsInEnum();
    }
}
public sealed class CreateStaffAssignmentValidator : AbstractValidator<CreateStaffAssignmentRequest>
{
    public CreateStaffAssignmentValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Role).IsInEnum();
        RuleFor(x => x.EndDateTime).GreaterThan(x => x.StartDateTime);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
public sealed class CreateEventZoneValidator : AbstractValidator<CreateEventZoneRequest>
{
    public CreateEventZoneValidator()
    {
        RuleFor(x => x.VenueId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(160);
        RuleFor(x => x.Capacity).GreaterThan(0).When(x => x.Capacity.HasValue);
    }
}
public sealed class CreateBoothPlacementValidator : AbstractValidator<CreateBoothPlacementRequest>
{
    public CreateBoothPlacementValidator()
    {
        RuleFor(x => x.VenueId).NotEmpty();
        RuleFor(x => x.ZoneId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(160);
        RuleFor(x => x.Status).IsInEnum();
    }
}
