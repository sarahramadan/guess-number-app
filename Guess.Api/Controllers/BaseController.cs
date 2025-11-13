using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Guess.Api.Common;
using System.Net;

namespace Guess.Api.Controllers;

/// <summary>
/// Base controller with common functionality
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Gets the current authenticated user ID
    /// </summary>
    /// <returns>User ID if authenticated, otherwise null</returns>
    protected string? GetCurrentUserId()
    {
        return User?.Identity?.IsAuthenticated == true 
            ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            : null;
    }

    /// <summary>
    /// Gets the current authenticated user email
    /// </summary>
    /// <returns>User email if authenticated, otherwise null</returns>
    protected string? GetCurrentUserEmail()
    {
        return User?.Identity?.IsAuthenticated == true 
            ? User.FindFirst(ClaimTypes.Email)?.Value 
            : null;
    }

    /// <summary>
    /// Gets the current user roles
    /// </summary>
    /// <returns>List of user roles</returns>
    protected List<string> GetCurrentUserRoles()
    {
        return User?.FindAll(ClaimTypes.Role)?.Select(c => c.Value)?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// Validates that user is authenticated and returns user ID
    /// </summary>
    /// <returns>User ID or throws UnauthorizedAccessException</returns>
    protected string RequireAuthenticatedUser()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User must be authenticated");
        }
        return userId;
    }

    /// <summary>
    /// Creates a standardized success response
    /// </summary>
    protected IActionResult Success<T>(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return ResponseHelper.Success(data, statusCode);
    }

    /// <summary>
    /// Creates a standardized success response without data
    /// </summary>
    protected IActionResult Success(string? message = null, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return ResponseHelper.Success(message, statusCode);
    }

    /// <summary>
    /// Creates a standardized created response
    /// </summary>
    protected IActionResult Created<T>(T data, string? location = null)
    {
        return ResponseHelper.Created(data, location);
    }

    /// <summary>
    /// Creates a standardized bad request response
    /// </summary>
    protected IActionResult BadRequest(string message, Dictionary<string, string[]>? errors = null)
    {
        return ResponseHelper.BadRequest(message, errors);
    }

    /// <summary>
    /// Creates a standardized not found response
    /// </summary>
    protected IActionResult NotFound(string message = "Resource not found")
    {
        return ResponseHelper.NotFound(message);
    }

    /// <summary>
    /// Creates a standardized unauthorized response
    /// </summary>
    protected IActionResult Unauthorized(string message = "Unauthorized access")
    {
        return ResponseHelper.Unauthorized(message);
    }

    /// <summary>
    /// Creates a standardized conflict response
    /// </summary>
    protected IActionResult Conflict(string message)
    {
        return ResponseHelper.Conflict(message);
    }
}