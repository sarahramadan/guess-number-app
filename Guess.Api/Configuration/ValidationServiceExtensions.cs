using FluentValidation;
using FluentValidation.AspNetCore;

namespace Guess.Api.Configuration;

/// <summary>
/// FluentValidation configuration extensions
/// </summary>
public static class ValidationServiceExtensions
{
    /// <summary>
    /// Configures FluentValidation for automatic validation
    /// </summary>
    public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
    {
        // Register all validators from the Application assembly
        services.AddValidatorsFromAssemblyContaining<Guess.Application.Validators.RegisterRequestValidator>();
        
        // Add FluentValidation integration with ASP.NET Core
        services.AddFluentValidationAutoValidation();

        return services;
    }
}