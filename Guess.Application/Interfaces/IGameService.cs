using Guess.Application.DTOs;
using Guess.Domain.Models;
using Guess.Domain.Enums;

namespace Guess.Application.Interfaces;

/// <summary>
/// Interface for game service with history functionality
/// </summary>
public interface IGameService
{
    /// <summary>
    /// Creates a new game session for a user (1-43 range)
    /// </summary>
    Task<Result<GameSessionDto>> CreateGameSessionAsync(string userId, CreateGameSessionDto request);

    /// <summary>
    /// Makes a guess in an existing game session (with higher/lower hints)
    /// </summary>
    Task<Result<GameAttemptDto>> MakeGuessAsync(string userId, Guid gameSessionId, MakeGuessDto request);

    /// <summary>
    /// Gets user's game statistics (shows best attempts when logging back)
    /// </summary>
    Task<Result<GameStatsDto>> GetUserStatsAsync(string userId);

    /// <summary>
    /// Gets user's game history with pagination and filtering
    /// </summary>
    Task<Result<PagedResult<GameSessionDto>>> GetGameHistoryAsync(string userId, int page, int pageSize, GameStatus? status = null, GameDifficulty? difficulty = null);

    /// <summary>
    /// Gets a specific game session details with attempts
    /// </summary>
    Task<Result<GameSessionDto>> GetGameSessionAsync(string userId, Guid gameSessionId);
}

