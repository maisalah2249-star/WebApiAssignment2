namespace LibrarySystem.Middlewares;

/// <summary>
/// Copies HttpContext.TraceIdentifier into the X-Request-Id response header and logs the assignment.
/// </summary>
public sealed class RequestIdHeaderMiddleware(ILogger<RequestIdHeaderMiddleware> logger) : IMiddleware
{
    // IMiddleware is activated through DI for each request, so it can safely receive scoped services.
    // It must be registered in the container (see AddServices).
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var requestId = context.TraceIdentifier;
        context.Response.Headers["X-Request-Id"] = requestId;

        logger.LogDebug(
            "Assigned request ID {RequestId} to {Method} {Path}",
            requestId,
            context.Request.Method,
            context.Request.Path);

        await next(context);
    }
}