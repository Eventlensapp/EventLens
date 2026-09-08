using EventLensAI.Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EventLensAI.API.Filters;

public sealed class ValidationFilter(IServiceProvider services) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var errors = new Dictionary<string, string[]>();
        foreach (var argument in context.ActionArguments.Values.Where(value => value is not null))
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(argument!.GetType());
            if (services.GetService(validatorType) is not IValidator validator) continue;
            var result = await validator.ValidateAsync(
                new ValidationContext<object>(argument), context.HttpContext.RequestAborted);
            foreach (var group in result.Errors.GroupBy(error => error.PropertyName))
                errors[group.Key] = group.Select(error => error.ErrorMessage).Distinct().ToArray();
        }
        if (errors.Count > 0)
        {
            context.Result = new BadRequestObjectResult(
                new ApiResponse<object>(false, "Validation failed.", new { errors }));
            return;
        }
        await next();
    }
}
