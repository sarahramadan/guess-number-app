using Guess.Domain.Entities;

namespace Guess.Application.Interfaces;

/// <summary>
/// Repository interface for UserGameStatistics entity
/// </summary>
public interface IUserGameStatisticsRepository : IRepository<UserGameStatistics>
{
    /// <summary>
    /// Gets game statistics for a specific user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>User's game statistics or null if not found</returns>
    Task<UserGameStatistics?> GetByUserIdAsync(string userId);

    /// <summary>
    /// Updates user game statistics after a game completion
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="gameScore">Score from the completed game</param>
    /// <param name="isWin">Whether the game was won</param>
    /// <param name="attempts">Number of attempts taken (only tracked for wins)</param>
    /// <returns>Updated statistics</returns>
    Task<UserGameStatistics> UpdateStatisticsAsync(string userId, int gameScore, bool isWin, int attempts = 0);

    /// <summary>
    /// Gets top players by total score with pagination
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of records per page</param>
    /// <returns>Top players and total count</returns>
    Task<(IEnumerable<UserGameStatistics> statistics, int totalCount)> GetTopPlayersByScoreAsync(int page, int pageSize);

    /// <summary>
    /// Gets top players by win rate with pagination
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of records per page</param>
    /// <param name="minGamesPlayed">Minimum games played to be included</param>
    /// <returns>Top players by win rate and total count</returns>
    Task<(IEnumerable<UserGameStatistics> statistics, int totalCount)> GetTopPlayersByWinRateAsync(int page, int pageSize, int minGamesPlayed = 5);

    /// <summary>
    /// Creates initial game statistics for a new user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>Created statistics</returns>
    Task<UserGameStatistics> CreateInitialStatisticsAsync(string userId);
}