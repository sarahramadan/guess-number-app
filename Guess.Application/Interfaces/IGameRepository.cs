using Guess.Domain.Entities;
using Guess.Domain.Enums;

namespace Guess.Application.Interfaces;

/// <summary>
/// Repository interface for GameSession entity with specific operations
/// </summary>
public interface IGameSessionRepository : IRepository<GameSession>
{
    /// <summary>
    /// Gets active game sessions for a user
    /// </summary>
    Task<IEnumerable<GameSession>> GetActiveGamesByUserIdAsync(string userId);

    /// <summary>
    /// Gets active game sessions for a user by UserGameStatistics ID
    /// </summary>
    Task<IEnumerable<GameSession>> GetActiveGamesByUserStatsIdAsync(Guid userStatsId);

    /// <summary>
    /// Gets a game session with all attempts
    /// </summary>
    Task<GameSession?> GetGameWithAttemptsAsync(Guid gameId);

    /// <summary>
    /// Gets game sessions by user with pagination
    /// </summary>
    Task<(IEnumerable<GameSession> games, int totalCount)> GetGamesByUserAsync(
        string userId, 
        int page, 
        int pageSize,
        GameStatus? status = null,
        GameDifficulty? difficulty = null);

    /// <summary>
    /// Gets user's game statistics
    /// </summary>
    Task<(int totalGames, int gamesWon, int totalScore, decimal averageScore)> GetUserStatsAsync(string userId);

    /// <summary>
    /// Gets leaderboard (top players by score)
    /// </summary>
    Task<IEnumerable<(string userId, string displayName, int totalScore, int gamesWon, decimal winRate)>> GetLeaderboardAsync(int count = 10);
}

/// <summary>
/// Repository interface for GameAttempt entity with specific operations
/// </summary>
public interface IGameAttemptRepository : IRepository<GameAttempt>
{
    /// <summary>
    /// Gets attempts for a specific game session
    /// </summary>
    Task<IEnumerable<GameAttempt>> GetAttemptsByGameSessionAsync(Guid gameSessionId);

    /// <summary>
    /// Gets the latest attempt for a game session
    /// </summary>
    Task<GameAttempt?> GetLatestAttemptAsync(Guid gameSessionId);

    /// <summary>
    /// Gets attempt count for a game session
    /// </summary>
    Task<int> GetAttemptCountAsync(Guid gameSessionId);
}