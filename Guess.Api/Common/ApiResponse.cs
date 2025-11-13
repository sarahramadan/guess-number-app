using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Guess.Api.Common;

/// <summary>
/// Standardized API response wrapper
/// </summary>
/// <typeparam name="T">Response data type</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the request was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response data
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Error message (if any)
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Validation errors (if any)
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Request timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a successful response
    /// </summary>
    public static ApiResponse<T> SuccessResult(T data, int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            StatusCode = statusCode
        };
    }

    /// <summary>
    /// Creates an error response
    /// </summary>
    public static ApiResponse<T> ErrorResult(string message, int statusCode = 400, Dictionary<string, string[]>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Errors = errors
        };
    }
}

/// <summary>
/// Non-generic version for responses without data
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// Creates a successful response without data
    /// </summary>
    public static ApiResponse SuccessResult(string? message = null, int statusCode = 200)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            StatusCode = statusCode
        };
    }
}

/// <summary>
/// Helper class for creating standardized controller responses
/// </summary>
public static class ResponseHelper
{
    /// <summary>
    /// Creates a success response with data
    /// </summary>
    public static IActionResult Success<T>(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var response = ApiResponse<T>.SuccessResult(data, (int)statusCode);
        return new ObjectResult(response) { StatusCode = (int)statusCode };
    }

    /// <summary>
    /// Creates a success response without data
    /// </summary>
    public static IActionResult Success(string? message = null, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var response = ApiResponse.SuccessResult(message, (int)statusCode);
        return new ObjectResult(response) { StatusCode = (int)statusCode };
    }

    /// <summary>
    /// Creates a created response (201)
    /// </summary>
    public static IActionResult Created<T>(T data, string? location = null)
    {
        var response = ApiResponse<T>.SuccessResult(data, 201);
        var result = new ObjectResult(response) { StatusCode = 201 };
        
        if (!string.IsNullOrEmpty(location))
        {
            result.Value = response;
        }
        
        return result;
    }

    /// <summary>
    /// Creates a bad request response (400)
    /// </summary>
    public static IActionResult BadRequest(string message, Dictionary<string, string[]>? errors = null)
    {
        var response = ApiResponse.ErrorResult(message, 400, errors);
        return new BadRequestObjectResult(response);
    }

    /// <summary>
    /// Creates a not found response (404)
    /// </summary>
    public static IActionResult NotFound(string message = "Resource not found")
    {
        var response = ApiResponse.ErrorResult(message, 404);
        return new NotFoundObjectResult(response);
    }

    /// <summary>
    /// Creates an unauthorized response (401)
    /// </summary>
    public static IActionResult Unauthorized(string message = "Unauthorized access")
    {
        var response = ApiResponse.ErrorResult(message, 401);
        return new UnauthorizedObjectResult(response);
    }

    /// <summary>
    /// Creates a conflict response (409)
    /// </summary>
    public static IActionResult Conflict(string message)
    {
        var response = ApiResponse.ErrorResult(message, 409);
        return new ConflictObjectResult(response);
    }
}