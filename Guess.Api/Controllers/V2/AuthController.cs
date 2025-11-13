using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Guess.Application.Interfaces;
using Guess.Domain.Models;
using Guess.Api.Controllers;
using System.Net;

namespace Guess.Api.Controllers.V2;

/// <summary>
/// Authentication controller v2.0 with enhanced features
/// </summary>
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
public class AuthController : BaseController
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthenticationService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user account with enhanced validation
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>Authentication response with tokens and user info</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        
        if (!result.IsSuccess)
        {
            if (result.Errors.Any())
            {
                return BadRequest(result.Error ?? "Registration failed", 
                    new Dictionary<string, string[]> { { "General", result.Errors.ToArray() } });
            }
            return BadRequest(result.Error ?? "Registration failed");
        }

        _logger.LogInformation("User registered successfully: {Email}", request.Email);
        
        // V2: Return additional metadata
        var response = new
        {
            Data = result.Data,
            Version = "2.0",
            Message = "Registration successful",
            Features = new[] { "Enhanced Security", "Better Validation", "Improved Responses" }
        };
        
        return Created(response);
    }

    /// <summary>
    /// Authenticate a user and return access tokens with enhanced response
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication response with tokens and user info</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
            return Unauthorized(result.Error ?? "Invalid credentials");
        }

        _logger.LogInformation("User logged in successfully: {Email}", request.Email);
        
        // V2: Return additional login metadata
        var response = new
        {
            Data = result.Data,
            Version = "2.0",
            LoginTime = DateTime.UtcNow,
            ClientInfo = new
            {
                UserAgent = Request.Headers["User-Agent"].ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            }
        };
        
        return Success(response);
    }

    /// <summary>
    /// Get current user profile information with enhanced details
    /// </summary>
    /// <returns>Enhanced user information</returns>
    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile()
    {
        var userId = GetCurrentUserId();
        var email = GetCurrentUserEmail();
        var roles = GetCurrentUserRoles();

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
        {
            return Unauthorized("Invalid user token");
        }

        // V2: Enhanced profile information
        var userInfo = new
        {
            Id = userId,
            Email = email,
            Roles = roles,
            IsAuthenticated = true,
            Version = "2.0",
            ProfileFeatures = new[]
            {
                "Enhanced Security",
                "Detailed Statistics",
                "Advanced Game Features"
            },
            LastAccessed = DateTime.UtcNow
        };

        return Success(userInfo);
    }

    /// <summary>
    /// Health check endpoint for v2.0 API
    /// </summary>
    /// <returns>Enhanced service health status</returns>
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult HealthCheck()
    {
        var healthInfo = new
        { 
            Status = "Healthy", 
            Service = "Authentication Service",
            Version = "2.0",
            Timestamp = DateTime.UtcNow,
            Features = new[] { "API Versioning", "Enhanced Responses", "Better Documentation" }
        };
        
        return Success(healthInfo);
    }
}