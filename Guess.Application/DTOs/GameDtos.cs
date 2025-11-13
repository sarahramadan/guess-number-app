using Guess.Domain.Entities;
using Guess.Domain.Enums;

namespace Guess.Application.DTOs;

/// <summary>
/// DTO for creating a new game session
/// </summary>
public class CreateGameSessionDto
{
    /// <summary>
    /// Game difficulty level
    /// </summary>
    public GameDifficulty Difficulty { get; set; } = GameDifficulty.Normal;

    /// <summary>
    /// Custom minimum range (optional, overrides difficulty default)
    /// </summary>
    public int? CustomMinRange { get; set; }

    /// <summary>
    /// Custom maximum range (optional, overrides difficulty default)
    /// </summary>
    public int? CustomMaxRange { get; set; }

    /// <summary>
    /// Custom maximum attempts (optional, overrides difficulty default)
    /// </summary>
    public int? CustomMaxAttempts { get; set; }
}

/// <summary>
/// DTO for making a guess in a game session
/// </summary>
public class MakeGuessDto
{
    /// <summary>
    /// The number being guessed
    /// </summary>
    public int GuessedNumber { get; set; }
}

/// <summary>
/// DTO for game session response
/// </summary>
public class GameSessionDto
{
    /// <summary>
    /// Game session ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User ID who owns this game
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Current number of attempts made
    /// </summary>
    public int AttemptsCount { get; set; }

    /// <summary>
    /// Maximum allowed attempts
    /// </summary>
    public int MaxAttempts { get; set; }

    /// <summary>
    /// Current status of the game
    /// </summary>
    public GameStatus Status { get; set; }

    /// <summary>
    /// Score for this game session
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Date and time when the game started
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Date and time when the game ended (if completed)
    /// </summary>
    public DateTime? EndedAt { get; set; }

    /// <summary>
    /// Minimum range for the secret number
    /// </summary>
    public int MinRange { get; set; }

    /// <summary>
    /// Maximum range for the secret number
    /// </summary>
    public int MaxRange { get; set; }

    /// <summary>
    /// Difficulty level of the game
    /// </summary>
    public GameDifficulty Difficulty { get; set; }

    /// <summary>
    /// List of attempts made in this game
    /// </summary>
    public List<GameAttemptDto> Attempts { get; set; } = new();
}

/// <summary>
/// DTO for game attempt response
/// </summary>
public class GameAttemptDto
{
    /// <summary>
    /// Attempt ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The number guessed by the user
    /// </summary>
    public int GuessedNumber { get; set; }

    /// <summary>
    /// The attempt number in sequence
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
    public DateTime AttemptedAt { get; set; }
}

/// <summary>
/// DTO for game statistics
/// </summary>
public class GameStatsDto
{
    /// <summary>
    /// Total number of games played
    /// </summary>
    public int TotalGames { get; set; }

    /// <summary>
    /// Total number of games won
    /// </summary>
    public int GamesWon { get; set; }

    /// <summary>
    /// Total score across all games
    /// </summary>
    public int TotalScore { get; set; }

    /// <summary>
    /// Win rate percentage
    /// </summary>
    public decimal WinRate { get; set; }
    
    /// <summary>
    /// Lowest number of guesses to win a game (best attempts)
    /// </summary>
    public int? BestAttempts { get; set; }

    /// <summary>
    /// Statistics by difficulty level
    /// </summary>
    public Dictionary<GameDifficulty, DifficultyStatsDto> StatsByDifficulty { get; set; } = new();
}

/// <summary>
/// DTO for difficulty-specific statistics
/// </summary>
public class DifficultyStatsDto
{
    /// <summary>
    /// Games played at this difficulty
    /// </summary>
    public int GamesPlayed { get; set; }

    /// <summary>
    /// Games won at this difficulty
    /// </summary>
    public int GamesWon { get; set; }

    /// <summary>
    /// Average score at this difficulty
    /// </summary>
    public decimal AverageScore { get; set; }

    /// <summary>
    /// Best score at this difficulty
    /// </summary>
    public int BestScore { get; set; }
}

/// <summary>
/// DTO for paginated results
/// </summary>
/// <typeparam name="T">Type of the items in the result</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// The list of items in the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNext => Page < TotalPages;

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPrevious => Page > 1;
}