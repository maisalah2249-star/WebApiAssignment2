using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Errors;

/// <summary>
/// Keeps middleware and MVC exception responses consistent while keeping implementation details
/// out of production responses. Full exception data belongs in server logs, not the API payload.
/// </summary>
public static class ExceptionProblemDetailsFactory
{
    public static ProblemDetails Create(Exception exception, HttpContext context, IHostEnvironment environment)
    {
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Detail = environment.IsDevelopment() ? exception.Message : "Please try again later.",
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] = context.TraceIdentifier;
        return problem;
    }
}