using Guess.Api.Services;
using Guess.Infrastructure.Data;
using Guess.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Guess.Api.Configuration;

/// <summary>
/// Database configuration extensions for Entity Framework and database services
/// </summary>
public static class DatabaseServiceExtensions
{
    /// <summary>
    /// Configures Entity Framework and database services
    /// </summary>
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database context configuration
        // Database context configuration with retry policy and logging
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
            {
                // Configure retry policy
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(2),
                    errorCodesToAdd: null);

                // Configure command timeout
                npgsqlOptions.CommandTimeout(30);

                // Add migrations assembly - point to Infrastructure where DbContext and migrations are located
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });

            // Configure logging
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);

            // Enable service provider validation in development
            var environment = services.BuildServiceProvider().GetService<IWebHostEnvironment>();
            if (environment?.IsDevelopment() == true)
            {
                options.EnableServiceProviderCaching(false);
                options.EnableSensitiveDataLogging(true);
                options.EnableDetailedErrors(true);
            }
        });

        // Configure strongly typed database settings
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.Section));

        return services;
    }

    /// <summary>
    /// Initializes database with migrations and seeding (Application Builder extension)
    /// </summary>
    public static async Task<IApplicationBuilder> InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitialization");
        var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        
        try
        {
            logger.LogInformation("Initializing database...");
            
            // Create database if it doesn't exist
            await context.Database.EnsureCreatedAsync();
            
            // Apply pending migrations
            if (context.Database.GetPendingMigrations().Any())
            {
                logger.LogInformation("Applying pending migrations...");
                await context.Database.MigrateAsync();
            }
            
            logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize database. Application will continue without database connection.");
            
            // In development, we want to continue running even if database is not available
            if (environment.IsDevelopment())
            {
                logger.LogWarning("Running in development mode without database connection");
            }
            else
            {
                // In production, database is critical - re-throw the exception
                throw;
            }
        }

        return app;
    }
}