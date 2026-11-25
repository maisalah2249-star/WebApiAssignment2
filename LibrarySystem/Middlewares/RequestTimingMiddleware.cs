using System.Diagnostics;

namespace LibrarySystem.Middlewares;

/// <summary>
/// Measures HTTP request processing time and logs the elapsed duration and any failure.
/// </summary>
public sealed class RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
{
    // Conventional middleware: constructed with the next RequestDelegate and exposes InvokeAsync.
    // Instances are long-lived, so constructor dependencies must be safe for that lifetime
    // (a logger is fine, a scoped DbContext is not).
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        Exception? failure = null;

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            failure = ex;
            throw;
        }
        finally
        {
            stopwatch.Stop();

            if (failure is null)
            {
                logger.LogInformation(
                    "HTTP {Method} {Path} returned {StatusCode} in {ElapsedMilliseconds} ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.Elapsed.TotalMilliseconds);
            }
            else
            {
                logger.LogError(
                    failure,
                    "HTTP {Method} {Path} failed after {ElapsedMilliseconds} ms",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.Elapsed.TotalMilliseconds);
            }
        }
    }
}