using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LibrarySystem.Filters;

public class ActionTimingFilter(ILogger<ActionTimingFilter> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next();
        }
        finally
        {
            stopwatch.Stop();

            logger.LogInformation(
                "MVC action {ActionName} took {ElapsedMilliseconds} ms",
                context.ActionDescriptor.DisplayName,
                stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}