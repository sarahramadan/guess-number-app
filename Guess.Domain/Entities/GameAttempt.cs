using Guess.Domain.Enums;

namespace Guess.Domain.Entities;

/// <summary>
/// Represents a single guess attempt within a game session
/// </summary>
public class GameAttempt
{
    /// <summary>
    /// Unique identifier for the attempt
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the game session this attempt belongs to
    /// </summary>
    public Guid GameSessionId { get; set; }
    
    /// <summary>
    /// The number guessed by the user
    /// </summary>
    public int GuessedNumber { get; set; }
    
    /// <summary>
    /// The attempt number in sequence (1, 2, 3, etc.)
    /// </summary>
    public int AttemptNumber { get; set; }
    
    /// <summary>
    /// Result of this attempt
    /// </summary>
    public GuessResult Result { get; set; }
    
    /// <summary>
    /// Hint provided after this attempt
    /// </summary>
    public string Hint { get; set; } = string.Empty;
    
    /// <summary>
    /// Date and time when the attempt was made
    /// </summary>
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Time taken for this attempt (from previous attempt or game start)
    /// </summary>
    public TimeSpan? TimeTaken { get; set; }
    
    /// <summary>
    /// Navigation property to the game session this attempt belongs to
    /// </summary>
    public virtual GameSession GameSession { get; set; } = null!;
}