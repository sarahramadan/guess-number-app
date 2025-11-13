using Guess.Domain.Enums;

namespace Guess.Domain.Entities;

/// <summary>
/// Represents a game session where a user plays the number guessing game
/// </summary>
public class GameSession
{
    /// <summary>
    /// Unique identifier for the game session
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the user game statistics this session belongs to
    /// </summary>
    public Guid UserGameStatisticsId { get; set; }
    
    /// <summary>
    /// The secret number to be guessed (1-100)
    /// </summary>
    public int SecretNumber { get; set; }
    
    /// <summary>
    /// Current number of attempts made
    /// </summary>
    public int AttemptsCount { get; set; } = 0;
    
    /// <summary>
    /// Maximum allowed attempts
    /// </summary>
    public int MaxAttempts { get; set; } = 10;
    
    /// <summary>
    /// Current status of the game
    /// </summary>
    public GameStatus Status { get; set; } = GameStatus.InProgress;
    
    /// <summary>
    /// Score for this game session
    /// </summary>
    public int Score { get; set; } = 0;
    
    /// <summary>
    /// Date and time when the game started
    /// </summary>
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when the game ended (if completed)
    /// </summary>
    public DateTime? EndedAt { get; set; }
    
    /// <summary>
    /// Duration of the game
    /// </summary>
    public TimeSpan? Duration => EndedAt.HasValue ? EndedAt.Value - StartedAt : null;
    
    /// <summary>
    /// Minimum range for the secret number
    /// </summary>
    public int MinRange { get; set; } = 1;
    
    /// <summary>
    /// Maximum range for the secret number
    /// </summary>
    public int MaxRange { get; set; } = 43; // Updated to match requirements
    
    /// <summary>
    /// Difficulty level of the game
    /// </summary>
    public GameDifficulty Difficulty { get; set; } = GameDifficulty.Normal;
    
    /// <summary>
    /// Navigation property to the user's game statistics
    /// </summary>
    public virtual UserGameStatistics UserGameStatistics { get; set; } = null!;
    
    /// <summary>
    /// Navigation property for game attempts
    /// </summary>
    public virtual ICollection<GameAttempt> Attempts { get; set; } = new List<GameAttempt>();
}