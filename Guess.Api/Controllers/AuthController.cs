using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Guess.Application.Interfaces;
using Guess.Domain.Models;
using System.Net;

namespace Guess.Api.Controllers;

/// <summary>
/// Minimal authentication controller for basic game requirements
/// </summary>
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
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
    /// Register a new user account
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
        return Created(result.Data);
    }

    /// <summary>
    /// Authenticate a user and return access tokens (shows best attempts when logging back)
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication response with tokens and user info including game stats</returns>
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
        return Success(result.Data);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New authentication response with fresh tokens</returns>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        
        if (!result.IsSuccess)
        {
            return Unauthorized(result.Error ?? "Invalid refresh token");
        }

        return Success(result.Data);
    }

    /// <summary>
    /// Logout user and invalidate tokens
    /// </summary>
    /// <returns>Success status</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = RequireAuthenticatedUser();
        var result = await _authService.LogoutAsync(userId);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error ?? "Logout failed");
        }

        _logger.LogInformation("User logged out successfully: {UserId}", userId);
        return Success("Logged out successfully");
    }

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="request">Password change request</param>
    /// <returns>Success status</returns>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = RequireAuthenticatedUser();
        var result = await _authService.ChangePasswordAsync(userId, request);
        
        if (!result.IsSuccess)
        {
            if (result.Errors.Any())
            {
                return BadRequest(result.Error ?? "Password change failed", 
                    new Dictionary<string, string[]> { { "General", result.Errors.ToArray() } });
            }
            return BadRequest(result.Error ?? "Password change failed");
        }

        _logger.LogInformation("Password changed successfully for user: {UserId}", userId);
        return Success("Password changed successfully");
    }

    /// <summary>
    /// Get current user profile information (shows best attempts when user logs back)
    /// </summary>
    /// <returns>Current user information with game stats</returns>
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        var email = GetCurrentUserEmail();

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
        {
            return Unauthorized("Invalid user token");
        }

        // Get user's game statistics to show best attempts
        var statsResult = await _authService.GetUserProfileWithStatsAsync(userId);
        
        if (!statsResult.IsSuccess)
        {
            return BadRequest(statsResult.Error ?? "Failed to retrieve profile");
        }

        return Success(statsResult.Data);
    }
}