using Guess.Application.Interfaces;
using Guess.Application.Services;
using Guess.Infrastructure.Repositories;
using Guess.Api.Services;
using Guess.Domain.Models;

namespace Guess.Api.Configuration;

/// <summary>
/// Application services configuration extensions
/// </summary>
public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Registers application services and repositories
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application services (business logic)
        services.AddScoped<IAuthenticationService, Guess.Application.Services.AuthenticationService>();
        services.AddScoped<IGameService, GameService>();
        
        // Register JWT helper
        services.AddScoped<IJwtHelper, JwtHelper>();

        // Register repositories and Unit of Work
        services.AddScoped<IUnitOfWork, Guess.Infrastructure.UnitOfWork.UnitOfWork>();
        services.AddScoped<IGameSessionRepository, Guess.Infrastructure.Repositories.GameSessionRepository>();
        services.AddScoped<IGameAttemptRepository, Guess.Infrastructure.Repositories.GameAttemptRepository>();
        services.AddScoped<IUserGameStatisticsRepository, Guess.Infrastructure.Repositories.UserGameStatisticsRepository>();

        return services;
    }

    /// <summary>
    /// Registers background services
    /// </summary>
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<LeaderboardUpdateService>();

        return services;
    }

    /// <summary>
    /// Registers configuration validation and other singleton services
    /// </summary>
    public static IServiceCollection AddConfigurationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure strongly typed settings
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.Section));

        // Register configuration validation service
        services.AddSingleton<ConfigurationValidationService>();

        return services;
    }
}