using System.Net;
using System.Text.Json;
using FluentValidation;
using Guess.Domain.Exceptions;

namespace Guess.Api.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            ValidationException validationEx => new ErrorResponse
            {
                Title = "Validation Error",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "One or more validation errors occurred.",
                Errors = validationEx.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
            },
            AuthenticationException authEx => new ErrorResponse
            {
                Title = "Authentication Failed",
                Status = (int)HttpStatusCode.Unauthorized,
                Detail = authEx.Message
            },
            BusinessLogicException businessEx => new ErrorResponse
            {
                Title = "Business Logic Error",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = businessEx.Message
            },
            UnauthorizedAccessException => new ErrorResponse
            {
                Title = "Unauthorized",
                Status = (int)HttpStatusCode.Unauthorized,
                Detail = "You are not authorized to access this resource."
            },
            ArgumentException argEx => new ErrorResponse
            {
                Title = "Bad Request",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = argEx.Message
            },
            KeyNotFoundException => new ErrorResponse
            {
                Title = "Not Found",
                Status = (int)HttpStatusCode.NotFound,
                Detail = "The requested resource was not found."
            },
            InvalidOperationException invalidOpEx => new ErrorResponse
            {
                Title = "Invalid Operation",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = invalidOpEx.Message
            },
            _ => new ErrorResponse
            {
                Title = "Internal Server Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = "An unexpected error occurred. Please try again later."
            }
        };

        context.Response.StatusCode = errorResponse.Status;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse, jsonOptions);
        await context.Response.WriteAsync(jsonResponse);
    }
}

/// <summary>
/// Standard error response format
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Error title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// HTTP status code
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Error detail/message
    /// </summary>
    public string Detail { get; set; } = string.Empty;

    /// <summary>
    /// Field-specific validation errors
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// Unique trace identifier
    /// </summary>
    public string TraceId { get; set; } = System.Diagnostics.Activity.Current?.Id ?? Guid.NewGuid().ToString();

    /// <summary>
    /// Timestamp when error occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}