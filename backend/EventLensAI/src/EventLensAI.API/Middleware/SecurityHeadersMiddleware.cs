namespace EventLensAI.API.Middleware;

public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;
        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
        if (!context.Request.Path.StartsWithSegments("/swagger"))
            headers.Append("Content-Security-Policy", "default-src 'none'; frame-ancestors 'none'");
        await next(context);
    }
}
