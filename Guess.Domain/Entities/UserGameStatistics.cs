namespace Guess.Domain.Entities;

/// <summary>
/// Represents aggregated game statistics for a user
/// This entity separates game-related metrics from user profile information
/// </summary>
public class UserGameStatistics
{
    /// <summary>
    /// Unique identifier for the statistics record
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the user these statistics belong to
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Total number of games played
    /// </summary>
    public int GamesPlayed { get; set; } = 0;
    
    /// <summary>
    /// Total number of games won
    /// </summary>
    public int GamesWon { get; set; } = 0;
    
    /// <summary>
    /// User's total score across all games
    /// </summary>
    public int TotalScore { get; set; } = 0;
    
    /// <summary>
    /// User's lowest number of guesses to win a game (best attempts)
    /// </summary>
    public int? BestAttempts { get; set; } = null;
    
    /// <summary>
    /// Date when these statistics were last updated
    /// </summary>
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// User's win rate percentage
    /// </summary>
    public decimal WinRate => GamesPlayed > 0 ? (decimal)GamesWon / GamesPlayed * 100 : 0;
    
    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;
    
    /// <summary>
    /// Navigation property for user's game sessions
    /// </summary>
    public virtual ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
}