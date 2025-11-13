using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Guess.Api.Configuration;

/// <summary>
/// Swagger/OpenAPI documentation configuration extensions
/// </summary>
public static class SwaggerServiceExtensions
{
    /// <summary>
    /// Configures Swagger documentation
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // Configure for each API version
            options.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = "Guess Number API", 
                Version = "v1.0",
                Description = "A secure number guessing game API with user authentication and authorization"
            });
            
            options.SwaggerDoc("v2", new OpenApiInfo 
            { 
                Title = "Guess Number API", 
                Version = "v2.0",
                Description = "Enhanced version with improved responses and additional features"
            });

            // JWT Bearer authentication in Swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            // Include XML documentation
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }

    /// <summary>
    /// Configures Swagger in the application pipeline
    /// </summary>
    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Guess Number API v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Guess Number API v2");
                c.RoutePrefix = string.Empty; // Set Swagger UI at apps root
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                c.DefaultModelsExpandDepth(-1); // Hide models section by default
            });
        }

        return app;
    }
}