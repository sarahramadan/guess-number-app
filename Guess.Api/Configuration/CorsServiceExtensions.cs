using Microsoft.Extensions.Configuration;

namespace Guess.Api.Configuration;

/// <summary>
/// CORS configuration extensions
/// </summary>
public static class CorsServiceExtensions
{
    /// <summary>
    /// Configures CORS policies
    /// </summary>
    public static IServiceCollection AddCorsServices(this IServiceCollection services, IWebHostEnvironment environment)
    {
        // Get configuration to read CORS settings
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultPolicy", policy =>
            {
                if (environment.IsDevelopment())
                {
                    // Allow multiple development URLs
                    policy.WithOrigins(
                              "http://localhost:4200",
                              "http://localhost:64054", 
                              "https://localhost:4200",
                              "https://localhost:64054")
                          .AllowCredentials()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                }
                else
                {
                    // Get allowed origins from configuration or environment variable
                    var allowedOrigins = configuration["CorsAllowedOrigins"] 
                                       ?? Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS") 
                                       ?? "https://yourdomain.com";
                    
                    var origins = allowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                .Select(origin => origin.Trim())
                                                .ToArray();
                    
                    policy.WithOrigins(origins)
                          .AllowCredentials()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Adds CORS to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app, string policyName = "DefaultPolicy")
    {
        app.UseCors(policyName);
        return app;
    }
}