namespace Guess.Domain.Models;

/// <summary>
/// Request model for user registration
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// User's email address (used as username)
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// User's password
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Password confirmation
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;
    
    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
}

/// <summary>
/// Request model for user login
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// User's password
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether to remember the user login
    /// </summary>
    public bool RememberMe { get; set; } = false;
}

/// <summary>
/// Response model for successful authentication
/// </summary>
public class AuthenticationResponse
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Refresh token for obtaining new access tokens
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Token expiration time
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// User information
    /// </summary>
    public UserInfo User { get; set; } = new();
}

/// <summary>
/// User information model
/// </summary>
public class UserInfo
{
    /// <summary>
    /// User's unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// User's display name
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's roles
    /// </summary>
    public List<string> Roles { get; set; } = new();
    
    /// <summary>
    /// User's game statistics
    /// </summary>
    public UserStats Stats { get; set; } = new();
}

/// <summary>
/// User statistics model
/// </summary>
public class UserStats
{
    /// <summary>
    /// Total score across all games
    /// </summary>
    public int TotalScore { get; set; }
    
    /// <summary>
    /// Total number of games played
    /// </summary>
    public int GamesPlayed { get; set; }
    
    /// <summary>
    /// Total number of games won
    /// </summary>
    public int GamesWon { get; set; }
    
    /// <summary>
    /// Average score
    /// </summary>
    public decimal AverageScore { get; set; }
    
    /// <summary>
    /// Win rate percentage
    /// </summary>
    public decimal WinRate { get; set; }
    
    /// <summary>
    /// Lowest number of guesses to win a game (best attempts)
    /// </summary>
    public int? BestAttempts { get; set; }
}

/// <summary>
/// Request model for token refresh
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// The refresh token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Request model for password change
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// Current password
    /// </summary>
    public string CurrentPassword { get; set; } = string.Empty;
    
    /// <summary>
    /// New password
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;
    
    /// <summary>
    /// New password confirmation
    /// </summary>
    public string ConfirmNewPassword { get; set; } = string.Empty;
}