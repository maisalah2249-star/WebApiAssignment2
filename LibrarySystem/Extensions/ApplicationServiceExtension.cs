using AutoMapper;
using LibrarySystem.Data;
using LibrarySystem.Data.Interceptors;
using LibrarySystem.Filters;
using LibrarySystem.Helper;
using LibrarySystem.Middlewares;
using LibrarySystem.Repositories;
using LibrarySystem.Services;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Extensions;

public static class ApplicationServiceExtension
{
    public const string CorsPolicyName = "ConfigurationOrigins";

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{typeof(ApplicationServiceExtension).Assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);
        });

        // DbContext + Interceptors
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultSqlConnection"));
            options.AddInterceptors(new SoftDeleteInterceptor(), new CreatedAtInterceptor(), new ModifiedAtInterceptor());
        });

        // AutoMapper
        services.AddSingleton<IMapper>(_ =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BookProfile>();
                cfg.AddProfile<AuthorProfile>();
                cfg.AddProfile<BorrowerProfile>();
            });

            return config.CreateMapper();
        });

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IBorrowerRepository, BorrowerRepository>();

        // Services
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IBorrowerService, BorrowerService>();

        // Middlewares: IMiddleware-based ones must be registered in the container
        services.AddTransient<RequestIdHeaderMiddleware>();

        // Action filters (used through [ServiceFilter])
        services.AddScoped<ActionExecutionLoggingFilter>();
        services.AddScoped<ActionTimingFilter>();
        services.AddScoped<ExceptionHandlingFilter>();

        // CORS: only explicitly configured origins get browser access
        var corsOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options =>
            options.AddPolicy(CorsPolicyName, policy =>
            {
                if (corsOrigins.Length > 0)
                {
                    policy.WithOrigins(corsOrigins);
                }

                // Credentials stay disabled
                policy.AllowAnyHeader().AllowAnyMethod();
            }));

        return services;
    }
}