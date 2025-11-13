namespace Guess.Application.Interfaces;

/// <summary>
/// Interface for JWT token operations
/// </summary>
public interface IJwtHelper
{
    /// <summary>
    /// Generates a JWT access token for the specified user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="email">User email</param>
    /// <param name="roles">User roles</param>
    /// <returns>JWT token string</returns>
    Task<string> GenerateAccessTokenAsync(string userId, string email, IList<string> roles);

    /// <summary>
    /// Validates a JWT token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    bool ValidateToken(string token);

    /// <summary>
    /// Extracts user ID from JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID if valid token, null otherwise</returns>
    string? GetUserIdFromToken(string token);
}