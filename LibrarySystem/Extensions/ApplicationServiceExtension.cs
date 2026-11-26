using System.Text;
using AutoMapper;
using LibrarySystem.Data;
using LibrarySystem.Data.Interceptors;
using LibrarySystem.Entities;
using LibrarySystem.Filters;
using LibrarySystem.Helper;
using LibrarySystem.Middlewares;
using LibrarySystem.Repositories;
using LibrarySystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace LibrarySystem.Extensions;

public static class ApplicationServiceExtension
{
    public const string CorsPolicyName = "ConfigurationOrigins";

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Swagger (with JWT Bearer support)
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{typeof(ApplicationServiceExtension).Assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Paste the JWT token only (without the 'Bearer ' prefix)."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
            });
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

        // Authentication (Identity + JWT)
        services.AddDbContext<AuthenticationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("AuthenticationConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AuthenticationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
            };
        });

        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}