using LibrarySystem.Data;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Repositories;
using LibrarySystem.Helper;
using AutoMapper;
using LibrarySystem.Filters;
using LibrarySystem.Services;
namespace LibrarySystem.Extensions;

public static class ApplicationServiceExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        #region Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{typeof(ApplicationServiceExtension).Assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);
        });
        #endregion

        #region DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultSqlConnection")));
        #endregion

        #region Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IBookRepository, BookRepository>();
        #endregion

        // Register AutoMapper in the IoC container
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

        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ActionExecutionLoggingFilter>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IBorrowerService, BorrowerService>();
        return services;
    }
}