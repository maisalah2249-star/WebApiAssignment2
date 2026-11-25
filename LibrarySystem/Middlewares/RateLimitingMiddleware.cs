namespace LibrarySystem.Middlewares;

/// <summary>
/// Simple fixed-window rate limiter: at most 15 API requests per 10 seconds.
/// </summary>
public sealed class RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
{
    private const int MaxRequests = 15;
    private static readonly TimeSpan Window = TimeSpan.FromSeconds(10);
    private static readonly object Locker = new();

    private static int _counter;
    private static DateTime _windowStart = DateTime.UtcNow;

    public async Task InvokeAsync(HttpContext context)
    {
        // Only API calls are limited, so Swagger UI static files are not counted
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await next(context);
            return;
        }

        bool allowed;

        lock (Locker)
        {
            var now = DateTime.UtcNow;

            if (now - _windowStart > Window)
            {
                _windowStart = now;
                _counter = 0;
            }

            _counter++;
            allowed = _counter <= MaxRequests;
        }

        if (!allowed)
        {
            logger.LogWarning("Rate limit exceeded for request {TraceId}", context.TraceIdentifier);

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers["Retry-After"] = "10";
            await context.Response.WriteAsync("Rate Limit Exceeded!");
            return;
        }

        await next(context);
    }
}