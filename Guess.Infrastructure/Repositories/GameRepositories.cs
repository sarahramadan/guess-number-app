using Microsoft.EntityFrameworkCore;
using Guess.Application.Interfaces;
using Guess.Domain.Entities;
using Guess.Domain.Enums;
using Guess.Infrastructure.Data;

namespace Guess.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for GameSession entity
/// </summary>
public class GameSessionRepository : Repository<GameSession>, IGameSessionRepository
{
    public GameSessionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<GameSession>> GetActiveGamesByUserIdAsync(string userId)
    {
        return await _dbSet
            .Include(g => g.UserGameStatistics)
            .Where(g => g.UserGameStatistics.UserId == userId && g.Status == GameStatus.InProgress)
            .OrderByDescending(g => g.StartedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<GameSession>> GetActiveGamesByUserStatsIdAsync(Guid userStatsId)
    {
        return await _dbSet
            .Where(g => g.UserGameStatisticsId == userStatsId && g.Status == GameStatus.InProgress)
            .OrderByDescending(g => g.StartedAt)
            .ToListAsync();
    }

    public async Task<GameSession?> GetGameWithAttemptsAsync(Guid gameId)
    {
        return await _dbSet
            .Include(g => g.Attempts.OrderBy(a => a.AttemptNumber))
            .Include(g => g.UserGameStatistics)
            .FirstOrDefaultAsync(g => g.Id == gameId);
    }

    public async Task<(IEnumerable<GameSession> games, int totalCount)> GetGamesByUserAsync(
        string userId, 
        int page, 
        int pageSize,
        GameStatus? status = null,
        GameDifficulty? difficulty = null)
    {
        var query = _dbSet
            .Include(g => g.UserGameStatistics)
            .Where(g => g.UserGameStatistics.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(g => g.Status == status.Value);
        }

        if (difficulty.HasValue)
        {
            query = query.Where(g => g.Difficulty == difficulty.Value);
        }

        var totalCount = await query.CountAsync();

        var games = await query
            .Include(g => g.Attempts)
            .OrderByDescending(g => g.StartedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (games, totalCount);
    }

    public async Task<(int totalGames, int gamesWon, int totalScore, decimal averageScore)> GetUserStatsAsync(string userId)
    {
        var stats = await _dbSet
            .Include(g => g.UserGameStatistics)
            .Where(g => g.UserGameStatistics.UserId == userId && g.Status != GameStatus.InProgress)
            .GroupBy(g => g.UserGameStatistics.UserId)
            .Select(g => new
            {
                TotalGames = g.Count(),
                GamesWon = g.Count(x => x.Status == GameStatus.Won),
                TotalScore = g.Sum(x => x.Score),
                AverageScore = g.Average(x => (decimal)x.Score)
            })
            .FirstOrDefaultAsync();

        if (stats == null)
        {
            return (0, 0, 0, 0);
        }

        return (stats.TotalGames, stats.GamesWon, stats.TotalScore, stats.AverageScore);
    }

    public async Task<IEnumerable<(string userId, string displayName, int totalScore, int gamesWon, decimal winRate)>> GetLeaderboardAsync(int count = 10)
    {
        return await _dbSet
            .Where(g => g.Status != GameStatus.InProgress)
            .Include(g => g.UserGameStatistics)
                .ThenInclude(u => u.User)
            .GroupBy(g => new { g.UserGameStatistics.UserId, g.UserGameStatistics.User.FirstName, g.UserGameStatistics.User.LastName })
            .Select(g => new
            {
                UserId = g.Key.UserId,
                DisplayName = g.Key.FirstName + " " + g.Key.LastName,
                TotalScore = g.Sum(x => x.Score),
                GamesWon = g.Count(x => x.Status == GameStatus.Won),
                TotalGames = g.Count(),
                WinRate = g.Count() > 0 ? (decimal)g.Count(x => x.Status == GameStatus.Won) / g.Count() * 100 : 0
            })
            .OrderByDescending(x => x.TotalScore)
            .Take(count)
            .Select(x => ValueTuple.Create(x.UserId, x.DisplayName, x.TotalScore, x.GamesWon, x.WinRate))
            .ToListAsync();
    }
}

/// <summary>
/// Repository implementation for GameAttempt entity
/// </summary>
public class GameAttemptRepository : Repository<GameAttempt>, IGameAttemptRepository
{
    public GameAttemptRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<GameAttempt>> GetAttemptsByGameSessionAsync(Guid gameSessionId)
    {
        return await _dbSet
            .Where(a => a.GameSessionId == gameSessionId)
            .OrderBy(a => a.AttemptNumber)
            .ToListAsync();
    }

    public async Task<GameAttempt?> GetLatestAttemptAsync(Guid gameSessionId)
    {
        return await _dbSet
            .Where(a => a.GameSessionId == gameSessionId)
            .OrderByDescending(a => a.AttemptNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetAttemptCountAsync(Guid gameSessionId)
    {
        return await _dbSet
            .CountAsync(a => a.GameSessionId == gameSessionId);
    }
}