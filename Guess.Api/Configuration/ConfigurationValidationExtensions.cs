using Guess.Application.Services;
using Guess.Domain.Models;

namespace Guess.Api.Configuration;

/// <summary>
/// Configuration validation extensions for dependency injection
/// </summary>
public static class ConfigurationValidationExtensions
{
    /// <summary>
    /// Adds configuration validation services
    /// </summary>
    public static IServiceCollection AddConfigurationValidation(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind and validate JWT settings
        services.Configure<JwtSettings>(options =>
        {
            configuration.GetSection(JwtSettings.Section).Bind(options);
        });

        // Bind and validate database settings
        services.Configure<DatabaseSettings>(options =>
        {
            configuration.GetSection(DatabaseSettings.Section).Bind(options);
        });

        services.AddSingleton<ConfigurationValidationService>();

        return services;
    }

    /// <summary>
    /// Validates configuration on application startup
    /// </summary>
    public static IServiceProvider ValidateConfigurationOnStartup(this IServiceProvider serviceProvider)
    {
        var validator = serviceProvider.GetRequiredService<ConfigurationValidationService>();
        validator.ValidateConfiguration();
        return serviceProvider;
    }
}