using Guess.Domain.Models;

namespace Guess.Application.Interfaces;

/// <summary>
/// Interface for authentication services
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="request">Registration request data</param>
    /// <returns>Authentication response with user info and tokens</returns>
    Task<Result<AuthenticationResponse>> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Authenticates a user and returns tokens (includes best attempts when logging back)
    /// </summary>
    /// <param name="request">Login request data</param>
    /// <returns>Authentication response with user info, tokens, and game stats</returns>
    Task<Result<AuthenticationResponse>> LoginAsync(LoginRequest request);

    /// <summary>
    /// Refreshes an access token using a refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New authentication response with fresh tokens</returns>
    Task<Result<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request);

    /// <summary>
    /// Logs out a user by invalidating their tokens
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> LogoutAsync(string userId);

    /// <summary>
    /// Changes a user's password
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="request">Change password request</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);

    /// <summary>
    /// Gets user profile with game statistics (shows best attempts when logging back)
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>User profile with game statistics including best attempts</returns>
    Task<Result<AuthenticationResponse>> GetUserProfileWithStatsAsync(string userId);
}