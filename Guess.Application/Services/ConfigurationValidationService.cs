using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Guess.Domain.Models;

namespace Guess.Application.Services;

/// <summary>
/// Service for validating application configuration
/// </summary>
public class ConfigurationValidationService
{
    private readonly ILogger<ConfigurationValidationService> _logger;
    private readonly JwtSettings _jwtSettings;
    private readonly DatabaseSettings _databaseSettings;

    public ConfigurationValidationService(
        ILogger<ConfigurationValidationService> logger,
        IOptions<JwtSettings> jwtSettings,
        IOptions<DatabaseSettings> databaseSettings)
    {
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
        _databaseSettings = databaseSettings.Value;
    }

    /// <summary>
    /// Validates all configuration settings
    /// </summary>
    public void ValidateConfiguration()
    {
        _logger.LogInformation("Starting configuration validation");

        ValidateJwtSettings();
        ValidateDatabaseSettings();

        _logger.LogInformation("Configuration validation completed successfully");
    }

    private void ValidateJwtSettings()
    {
        if (string.IsNullOrEmpty(_jwtSettings.SecretKey))
            throw new ArgumentException("JWT SecretKey is required");

        if (_jwtSettings.SecretKey.Length < 32)
            throw new ArgumentException("JWT SecretKey must be at least 32 characters long");

        if (string.IsNullOrEmpty(_jwtSettings.Issuer))
            throw new ArgumentException("JWT Issuer is required");

        if (string.IsNullOrEmpty(_jwtSettings.Audience))
            throw new ArgumentException("JWT Audience is required");

        if (_jwtSettings.AccessTokenExpiryMinutes <= 0)
            throw new ArgumentException("JWT AccessTokenExpiryMinutes must be greater than 0");

        if (_jwtSettings.RefreshTokenExpiryDays <= 0)
            throw new ArgumentException("JWT RefreshTokenExpiryDays must be greater than 0");

        _logger.LogDebug("JWT settings validation passed");
    }

    private void ValidateDatabaseSettings()
    {
        if (string.IsNullOrEmpty(_databaseSettings.DefaultConnection))
            throw new ArgumentException("Database DefaultConnection is required");

        if (!_databaseSettings.DefaultConnection.Contains("Database=") && 
            !_databaseSettings.DefaultConnection.Contains("Initial Catalog="))
            throw new ArgumentException("Database connection string must specify a database name");

        _logger.LogDebug("Database settings validation passed");
    }
}