using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Guess.Application.Interfaces;
using Guess.Application.Services;
using Guess.Domain.Entities;
using Guess.Domain.Models;
using Guess.Domain.Exceptions;

namespace Guess.Application.Services;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtHelper _jwtHelper;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        IJwtHelper jwtHelper,
        ILogger<AuthenticationService> logger)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _jwtHelper = jwtHelper;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    public async Task<Result<AuthenticationResponse>> RegisterAsync(RegisterRequest request)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result<AuthenticationResponse>.Failure("User with this email already exists");
        }

        // Create new user
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = true, // For simplicity, auto-confirm emails
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result<AuthenticationResponse>.Failure(errors);
        }

        // Add default role
        await _userManager.AddToRoleAsync(user, "Player");

        // Generate tokens
        var authResponse = await GenerateAuthenticationResponseAsync(user);
        return Result<AuthenticationResponse>.Success(authResponse);
    }

    /// <summary>
    /// Authenticates a user
    /// </summary>
    public async Task<Result<AuthenticationResponse>> LoginAsync(LoginRequest request)
    {
        // Find user by email
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result<AuthenticationResponse>.Failure("Invalid email or password");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            return Result<AuthenticationResponse>.Failure("Account is deactivated");
        }

        // Verify password
        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            // Increment failed login attempts
            await _userManager.AccessFailedAsync(user);
            
            // Check if user should be locked out
            if (await _userManager.IsLockedOutAsync(user))
            {
                return Result<AuthenticationResponse>.Failure("Account is locked due to multiple failed attempts");
            }
            
            return Result<AuthenticationResponse>.Failure("Invalid email or password");
        }
        
        // Reset failed login attempts on successful login
        await _userManager.ResetAccessFailedCountAsync(user);

        // Generate tokens
        var authResponse = await GenerateAuthenticationResponseAsync(user);
        return Result<AuthenticationResponse>.Success(authResponse);
    }

    /// <summary>
    /// Refreshes access token using refresh token
    /// </summary>
    public Task<Result<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        // In a production app, you would validate the refresh token against stored tokens
        // For simplicity, we'll just decode the user ID from the token
        // This is a simplified implementation - in production, store refresh tokens in database

        // For this example, we'll assume refresh token is valid
        // In production, implement proper refresh token validation
        return Task.FromResult(Result<AuthenticationResponse>.Failure("Refresh token functionality not fully implemented in this demo"));
    }

    /// <summary>
    /// Logs out a user by invalidating their tokens
    /// </summary>
    public async Task<Result> LogoutAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("User not found");
            }

            // Update security stamp to invalidate existing tokens
            await _userManager.UpdateSecurityStampAsync(user);

            _logger.LogInformation("User logged out successfully: {UserId}", userId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user {UserId}", userId);
            return Result.Failure("Logout failed");
        }
    }

    /// <summary>
    /// Changes user password
    /// </summary>
    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("User not found");
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result.Failure(errors);
            }

            // Update security stamp to invalidate existing tokens
            await _userManager.UpdateSecurityStampAsync(user);

            _logger.LogInformation("Password changed successfully for user: {UserId}", userId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", userId);
            return Result.Failure("Password change failed");
        }
    }

    /// <summary>
    /// Gets user profile with game statistics (shows best attempts when logging back)
    /// </summary>
    public async Task<Result<AuthenticationResponse>> GetUserProfileWithStatsAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<AuthenticationResponse>.Failure("User not found");
            }

            var authResponse = await GenerateAuthenticationResponseAsync(user);
            return Result<AuthenticationResponse>.Success(authResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile with stats for user {UserId}", userId);
            return Result<AuthenticationResponse>.Failure("Failed to retrieve user profile");
        }
    }

    /// <summary>
    /// Generates authentication response with tokens and user info including game stats
    /// </summary>
    private async Task<AuthenticationResponse> GenerateAuthenticationResponseAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = await _jwtHelper.GenerateAccessTokenAsync(user.Id, user.Email!, roles);
        var refreshToken = JwtHelper.GenerateRefreshToken();

        // Get user game statistics
        var userStats = await GetOrCreateUserGameStatisticsAsync(user.Id);

        return new AuthenticationResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60), // Should match JWT settings
            User = new UserInfo
            {
                Id = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Roles = roles.ToList(),
                Stats = new UserStats
                {
                    TotalScore = userStats.TotalScore,
                    GamesPlayed = userStats.GamesPlayed,
                    GamesWon = userStats.GamesWon,
                    AverageScore = userStats.GamesPlayed > 0 ? (decimal)userStats.TotalScore / userStats.GamesPlayed : 0,
                    WinRate = userStats.WinRate,
                    BestAttempts = userStats.BestAttempts
                }
            }
        };
    }

    /// <summary>
    /// Gets existing user game statistics or creates new ones if they don't exist
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>UserGameStatistics entity</returns>
    private async Task<UserGameStatistics> GetOrCreateUserGameStatisticsAsync(string userId)
    {
        var userStats = await _unitOfWork.UserGameStatistics.GetByUserIdAsync(userId);
        
        if (userStats == null)
        {
            // Create initial statistics if they don't exist
            userStats = await _unitOfWork.UserGameStatistics.CreateInitialStatisticsAsync(userId);
            await _unitOfWork.SaveChangesAsync();
        }

        return userStats;
    }
}