using Guess.Api.Middleware;
using Guess.Api.Services;
using Guess.Application.Services;

namespace Guess.Api.Configuration;

/// <summary>
/// Middleware pipeline configuration extensions
/// </summary>
public static class MiddlewareServiceExtensions
{
    /// <summary>
    /// Configures the complete HTTP request pipeline in proper order
    /// </summary>
    public static IApplicationBuilder UseApplicationPipeline(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        // Global exception handling (must be first)
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

        // Development-specific middleware
        if (environment.IsDevelopment())
        {
            app.UseSwaggerDocumentation(environment);
        }
        else
        {
            app.UseHsts();
        }

        // CORS must be early in the pipeline (before authentication)
        app.UseCorsPolicy();

        // Security and logging middleware
        app.UseMiddleware<SecurityHeadersMiddleware>();
        app.UseMiddleware<RateLimitingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();

        // Standard ASP.NET Core middleware
        app.UseHttpsRedirection();

        // Routing must come before authentication/authorization
        app.UseRouting();
        
        // Authentication and authorization must be between UseRouting and UseEndpoints
        app.UseAuthenticationAndAuthorization();
        
        // Endpoints configuration
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        return app;
    }

    /// <summary>
    /// Validates configuration at startup
    /// </summary>
    public static IApplicationBuilder ValidateConfiguration(this IApplicationBuilder app)
    {
        var configValidation = app.ApplicationServices.GetRequiredService<ConfigurationValidationService>();
        configValidation.ValidateConfiguration();
        
        return app;
    }
}