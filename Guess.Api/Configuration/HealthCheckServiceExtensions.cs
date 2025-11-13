using Guess.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Guess.Api.Configuration;

/// <summary>
/// Health checks configuration extensions
/// </summary>
public static class HealthCheckServiceExtensions
{
    /// <summary>
    /// Configures health checks services
    /// </summary>
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>(
                name: "database",
                tags: new[] { "ready" })
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "ready" });

        return services;
    }

    /// <summary>
    /// Maps health check endpoints
    /// </summary>
    public static IApplicationBuilder UseHealthCheckEndpoints(this IApplicationBuilder app)
    {
        app.UseRouting();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health");
            endpoints.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });
        });

        return app;
    }
}