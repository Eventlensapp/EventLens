using EventLensAI.Application.DTOs.Events;
using FluentValidation;

namespace EventLensAI.Application.Validators;

public sealed class UpsertEventRequestValidator : AbstractValidator<UpsertEventRequest>
{
    public UpsertEventRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.Venue).MaximumLength(500);
        RuleFor(x => x.Address).MaximumLength(1000);
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate);
        RuleFor(x => x).Must(x => x.EndDate - x.StartDate <= TimeSpan.FromDays(31))
            .WithMessage("Event duration cannot exceed 31 days.");
        RuleFor(x => x.PrimaryColor).Matches("^#[0-9A-Fa-f]{6}$");
        RuleFor(x => x.SecondaryColor).Matches("^#[0-9A-Fa-f]{6}$");
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue);
        RuleFor(x => x.GuestLimit).GreaterThan(0).When(x => x.GuestLimit.HasValue);
        RuleFor(x => x.PhotoLimit).GreaterThan(0).When(x => x.PhotoLimit.HasValue);
    }
}
public sealed class EventSearchRequestValidator : AbstractValidator<EventSearchRequest>
{
    public EventSearchRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.SortBy).Must(x => new[] { "name", "startDate", "endDate", "status", "createdAt" }
            .Contains(x, StringComparer.OrdinalIgnoreCase));
    }
}
public sealed class UpdateEventSettingsRequestValidator : AbstractValidator<UpdateEventSettingsRequest>
{
    public UpdateEventSettingsRequestValidator()
    {
        RuleFor(x => x.Countdown).InclusiveBetween(0, 30);
        RuleFor(x => x.Language).NotEmpty().MaximumLength(10);
    }
}
public sealed class CreateEventRequestValidator:AbstractValidator<CreateEventRequest>
{
    public CreateEventRequestValidator()
    {
        RuleFor(x=>x.OrganizationId).NotEmpty();RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x=>x.Description).MaximumLength(4000);RuleFor(x=>x.EventTypeId).NotEmpty().When(x=>!x.TemplateId.HasValue);
        RuleFor(x=>x.EndDate).GreaterThan(x=>x.StartDate).When(x=>!x.TemplateId.HasValue);RuleFor(x=>x.Timezone).NotEmpty().MaximumLength(100);
        RuleFor(x=>x.VenueName).MaximumLength(500);RuleFor(x=>x.Address).MaximumLength(1000);
        RuleFor(x=>x.City).MaximumLength(120);RuleFor(x=>x.Country).MaximumLength(120);
        RuleFor(x=>x.ContactPerson).MaximumLength(200);RuleFor(x=>x.ContactEmail).EmailAddress().When(x=>!string.IsNullOrWhiteSpace(x.ContactEmail));
        RuleFor(x=>x.ContactPhone).MaximumLength(50);
        RuleFor(x=>x.Status).Must(x=>x is EventLensAI.Domain.Enums.EventStatus.Draft or EventLensAI.Domain.Enums.EventStatus.Upcoming)
            .WithMessage("New events can only start as Draft or Upcoming.");
    }
}
public sealed class CreateEventTypeRequestValidator:AbstractValidator<CreateEventTypeRequest>
{public CreateEventTypeRequestValidator(){RuleFor(x=>x.OrganizationId).NotEmpty();RuleFor(x=>x.Name).NotEmpty().MaximumLength(120);RuleFor(x=>x.Description).MaximumLength(1000);RuleFor(x=>x.Icon).MaximumLength(100);RuleFor(x=>x.Color).Matches("^#[0-9A-Fa-f]{6}$");}}
public sealed class UpdateEventTypeRequestValidator:AbstractValidator<UpdateEventTypeRequest>
{public UpdateEventTypeRequestValidator(){RuleFor(x=>x.Name).NotEmpty().MaximumLength(120);RuleFor(x=>x.Description).MaximumLength(1000);RuleFor(x=>x.Icon).MaximumLength(100);RuleFor(x=>x.Color).Matches("^#[0-9A-Fa-f]{6}$");}}
public sealed class CreateEventTemplateRequestValidator:AbstractValidator<CreateEventTemplateRequest>
{public CreateEventTemplateRequestValidator(){RuleFor(x=>x.OrganizationId).NotEmpty();RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);RuleFor(x=>x.Description).MaximumLength(2000);RuleFor(x=>x.EventTypeId).NotEmpty();RuleFor(x=>x.DefaultDurationMinutes).InclusiveBetween(1,44640);}}
public sealed class UpdateEventTemplateRequestValidator:AbstractValidator<UpdateEventTemplateRequest>
{public UpdateEventTemplateRequestValidator(){RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);RuleFor(x=>x.Description).MaximumLength(2000);RuleFor(x=>x.EventTypeId).NotEmpty();RuleFor(x=>x.DefaultDurationMinutes).InclusiveBetween(1,44640);}}
public sealed class CloneEventTemplateRequestValidator:AbstractValidator<CloneEventTemplateRequest>
{public CloneEventTemplateRequestValidator()=>RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);}
public sealed class UpdateEventCoreRequestValidator:AbstractValidator<UpdateEventCoreRequest>
{
    public UpdateEventCoreRequestValidator()
    {
        RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);RuleFor(x=>x.EventTypeId).NotEmpty();
        RuleFor(x=>x.EndDate).GreaterThan(x=>x.StartDate);RuleFor(x=>x.Timezone).NotEmpty().MaximumLength(100);
        RuleFor(x=>x.ContactEmail).EmailAddress().When(x=>!string.IsNullOrWhiteSpace(x.ContactEmail));
    }
}
public sealed class CloneEventRequestValidator:AbstractValidator<CloneEventRequest>
{
    public CloneEventRequestValidator(){RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);RuleFor(x=>x.EndDate).GreaterThan(x=>x.StartDate);}
}
public sealed class CreateVenueRequestValidator:AbstractValidator<CreateVenueRequest>
{public CreateVenueRequestValidator(){RuleFor(x=>x.VenueTypeId).NotEmpty();RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);RuleFor(x=>x.ContactEmail).EmailAddress().When(x=>!string.IsNullOrWhiteSpace(x.ContactEmail));RuleFor(x=>x.Latitude).InclusiveBetween(-90,90).When(x=>x.Latitude.HasValue);RuleFor(x=>x.Longitude).InclusiveBetween(-180,180).When(x=>x.Longitude.HasValue);RuleFor(x=>x.Capacity).GreaterThan(0).When(x=>x.Capacity.HasValue);}}
public sealed class UpdateVenueRequestValidator:AbstractValidator<UpdateVenueRequest>
{public UpdateVenueRequestValidator(){Include(new CreateVenueRequestValidatorAdapter());}private sealed class CreateVenueRequestValidatorAdapter:AbstractValidator<UpdateVenueRequest>{public CreateVenueRequestValidatorAdapter(){RuleFor(x=>x.VenueTypeId).NotEmpty();RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);RuleFor(x=>x.ContactEmail).EmailAddress().When(x=>!string.IsNullOrWhiteSpace(x.ContactEmail));}}}
public sealed class CreateScheduleRequestValidator:AbstractValidator<CreateScheduleRequest>
{public CreateScheduleRequestValidator(){RuleFor(x=>x.ScheduleTypeId).NotEmpty();RuleFor(x=>x.Title).NotEmpty().MaximumLength(200);RuleFor(x=>x.EndDateTime).GreaterThan(x=>x.StartDateTime);}}
public sealed class UpdateScheduleRequestValidator:AbstractValidator<UpdateScheduleRequest>
{public UpdateScheduleRequestValidator(){RuleFor(x=>x.ScheduleTypeId).NotEmpty();RuleFor(x=>x.Title).NotEmpty().MaximumLength(200);RuleFor(x=>x.EndDateTime).GreaterThan(x=>x.StartDateTime);}}
