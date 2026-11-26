using LibrarySystem.Data;
using LibrarySystem.Extensions;
using LibrarySystem.Middlewares;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddServices(builder.Configuration);

        var app = builder.Build();

        // Automatic migration + seeding
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                var dbContext = services.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.MigrateAsync();
                await LibraryContextSeeding.SeedAsync(dbContext);

                var authDbContext = services.GetRequiredService<AuthenticationDbContext>();
                await authDbContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, ex.Message);
            }
        }

        // Middlewares: the order matters
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<RequestTimingMiddleware>();
        app.UseMiddleware<RequestIdHeaderMiddleware>();
        app.UseMiddleware<RateLimitingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            // Demo endpoint (outside MVC) to prove ExceptionHandlingMiddleware works
            app.MapGet("/debug/error", (HttpContext _) =>
            {
                throw new InvalidOperationException("Test exception to demonstrate the exception middleware.");
            });
        }

        app.UseHttpsRedirection();

        app.UseCors(ApplicationServiceExtension.CorsPolicyName);

        app.UseAuthentication();   // who are you?  (must come before UseAuthorization)
        app.UseAuthorization();    // are you allowed?

        app.MapControllers();

        app.Run();
    }
}