using Microsoft.EntityFrameworkCore;
using Guess.Application.Interfaces;
using Guess.Domain.Entities;
using Guess.Infrastructure.Data;

namespace Guess.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for UserGameStatistics entity
/// </summary>
public class UserGameStatisticsRepository : Repository<UserGameStatistics>, IUserGameStatisticsRepository
{
    public UserGameStatisticsRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets game statistics for a specific user
    /// </summary>
    public async Task<UserGameStatistics?> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    /// <summary>
    /// Updates user game statistics after a game completion
    /// </summary>
    public async Task<UserGameStatistics> UpdateStatisticsAsync(string userId, int gameScore, bool isWin, int attempts = 0)
    {
        var statistics = await GetByUserIdAsync(userId);
        
        if (statistics == null)
        {
            statistics = await CreateInitialStatisticsAsync(userId);
        }

        // Update statistics
        statistics.GamesPlayed++;
        statistics.TotalScore += gameScore;
        
        if (isWin)
        {
            statistics.GamesWon++;
            
            // Track best attempts (lowest number of guesses to win)
            if (attempts > 0 && (statistics.BestAttempts == null || attempts < statistics.BestAttempts))
            {
                statistics.BestAttempts = attempts;
            }
        }
        
        statistics.LastUpdatedAt = DateTime.UtcNow;
        
        Update(statistics);
        return statistics;
    }

    /// <summary>
    /// Gets top players by total score with pagination
    /// </summary>
    public async Task<(IEnumerable<UserGameStatistics> statistics, int totalCount)> GetTopPlayersByScoreAsync(int page, int pageSize)
    {
        var query = _dbSet
            .Include(s => s.User)
            .Where(s => s.GamesPlayed > 0)
            .OrderByDescending(s => s.TotalScore)
            .ThenByDescending(s => s.WinRate);

        var totalCount = await query.CountAsync();
        var statistics = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (statistics, totalCount);
    }

    /// <summary>
    /// Gets top players by win rate with pagination
    /// </summary>
    public async Task<(IEnumerable<UserGameStatistics> statistics, int totalCount)> GetTopPlayersByWinRateAsync(int page, int pageSize, int minGamesPlayed = 5)
    {
        var query = _dbSet
            .Include(s => s.User)
            .Where(s => s.GamesPlayed >= minGamesPlayed)
            .OrderByDescending(s => s.WinRate)
            .ThenByDescending(s => s.TotalScore);

        var totalCount = await query.CountAsync();
        var statistics = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (statistics, totalCount);
    }

    /// <summary>
    /// Creates initial game statistics for a new user
    /// </summary>
    public async Task<UserGameStatistics> CreateInitialStatisticsAsync(string userId)
    {
        var statistics = new UserGameStatistics
        {
            UserId = userId
        };

        await AddAsync(statistics);
        return statistics;
    }
}