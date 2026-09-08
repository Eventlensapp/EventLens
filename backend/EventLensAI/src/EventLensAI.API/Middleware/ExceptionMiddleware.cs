using System.Net;
using EventLensAI.Application.Common;
using EventLensAI.Application.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EventLensAI.API.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception exception) when (!context.Response.HasStarted)
        {
            var (status, message) = exception switch
            {
                ValidationException => (HttpStatusCode.BadRequest, "Validation failed."),
                UnauthorizedException => (HttpStatusCode.Unauthorized, exception.Message),
                NotFoundException => (HttpStatusCode.NotFound, exception.Message),
                ConflictException => (HttpStatusCode.Conflict, exception.Message),
                DbUpdateException => (HttpStatusCode.Conflict, "The database operation could not be completed."),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            };
            if (exception is DbUpdateException)
                logger.LogError(exception, "Database request failure {TraceId}", context.TraceIdentifier);
            else if ((int)status >= 500)
                logger.LogError(exception, "Unhandled request failure {TraceId}", context.TraceIdentifier);
            else logger.LogWarning(exception, "Request failed with status {StatusCode} {TraceId}", (int)status, context.TraceIdentifier);
            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(
                new ApiResponse<object>(false, message, new { traceId = context.TraceIdentifier }));
        }
    }
}
