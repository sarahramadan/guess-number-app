using Microsoft.Extensions.Logging;
using Guess.Application.DTOs;
using Guess.Application.Interfaces;
using Guess.Domain.Entities;
using Guess.Domain.Enums;
using Guess.Domain.Models;
using Guess.Domain.Exceptions;

namespace Guess.Application.Services;

/// <summary>
/// Game service implementation with business logic
/// </summary>
public class GameService : IGameService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GameService> _logger;

    public GameService(IUnitOfWork unitOfWork, ILogger<GameService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<GameSessionDto>> CreateGameSessionAsync(string userId, CreateGameSessionDto request)
    {
        try
        {
            // Get or create user game statistics
            var userStats = await GetOrCreateUserGameStatisticsAsync(userId);

            // Check if user has any active games (limit to 3 concurrent games)
            var activeGames = await _unitOfWork.GameSessions.GetActiveGamesByUserStatsIdAsync(userStats.Id);
            if (activeGames.Count() >= 3)
            {
                return Result<GameSessionDto>.Failure("You can have a maximum of 3 active games at once.");
            }

            // Configure game based on difficulty or custom settings
            var (minRange, maxRange, maxAttempts) = GetGameConfiguration(request);

            // Generate secret number
            var random = new Random();
            var secretNumber = random.Next(minRange, maxRange + 1);

            // Create game session
            var gameSession = new GameSession
            {
                Id = Guid.NewGuid(),
                UserGameStatisticsId = userStats.Id,
                SecretNumber = secretNumber,
                MinRange = minRange,
                MaxRange = maxRange,
                MaxAttempts = maxAttempts,
                Difficulty = request.Difficulty,
                Status = GameStatus.InProgress,
                StartedAt = DateTime.UtcNow
            };

            await _unitOfWork.GameSessions.AddAsync(gameSession);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created new game session {GameId} for user {UserId}", gameSession.Id, userId);

            return Result<GameSessionDto>.Success(MapToGameSessionDto(gameSession));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating game session for user {UserId}", userId);
            return Result<GameSessionDto>.Failure("Failed to create game session");
        }
    }

    public async Task<Result<GameAttemptDto>> MakeGuessAsync(string userId, Guid gameSessionId, MakeGuessDto request)
    {
        try
        {
            // Get game session with attempts
            var gameSession = await _unitOfWork.GameSessions.GetGameWithAttemptsAsync(gameSessionId);
            if (gameSession == null)
            {
                return Result<GameAttemptDto>.Failure("Game session not found");
            }

            if (gameSession.UserGameStatistics.UserId != userId)
            {
                return Result<GameAttemptDto>.Failure("You are not authorized to access this game");
            }

            if (gameSession.Status != GameStatus.InProgress)
            {
                return Result<GameAttemptDto>.Failure("This game is no longer active");
            }

            // Validate guess range
            if (request.GuessedNumber < gameSession.MinRange || request.GuessedNumber > gameSession.MaxRange)
            {
                return Result<GameAttemptDto>.Failure($"Guess must be between {gameSession.MinRange} and {gameSession.MaxRange}");
            }

            // Check if max attempts exceeded
            if (gameSession.AttemptsCount >= gameSession.MaxAttempts)
            {
                return Result<GameAttemptDto>.Failure("Maximum number of attempts reached");
            }

            // Create attempt
            var attempt = new GameAttempt
            {
                Id = Guid.NewGuid(),
                GameSessionId = gameSessionId,
                GuessedNumber = request.GuessedNumber,
                AttemptNumber = gameSession.AttemptsCount + 1,
                AttemptedAt = DateTime.UtcNow
            };

            // Determine result and hint
            if (request.GuessedNumber == gameSession.SecretNumber)
            {
                attempt.Result = GuessResult.Correct;
                attempt.Hint = "Congratulations! You guessed correctly!";
                
                // Update game session - Won
                gameSession.Status = GameStatus.Won;
                gameSession.EndedAt = DateTime.UtcNow;
                gameSession.Score = CalculateScore(gameSession.AttemptsCount + 1, gameSession.MaxAttempts, gameSession.Difficulty);
            }
            else if (request.GuessedNumber < gameSession.SecretNumber)
            {
                attempt.Result = GuessResult.TooLow;
                attempt.Hint = GetSmartHint(request.GuessedNumber, gameSession.SecretNumber, "higher");
            }
            else
            {
                attempt.Result = GuessResult.TooHigh;
                attempt.Hint = GetSmartHint(request.GuessedNumber, gameSession.SecretNumber, "lower");
            }

            // Update attempts count
            gameSession.AttemptsCount++;

            // Check if max attempts reached and game not won
            if (gameSession.AttemptsCount >= gameSession.MaxAttempts && gameSession.Status == GameStatus.InProgress)
            {
                gameSession.Status = GameStatus.Lost;
                gameSession.EndedAt = DateTime.UtcNow;
                gameSession.Score = 0;
                attempt.Hint += $" Game over! The correct number was {gameSession.SecretNumber}.";
            }

            // Save game attempt and session first
            await _unitOfWork.GameAttempts.AddAsync(attempt);
            _unitOfWork.GameSessions.Update(gameSession);
            
            // Update user game statistics if game is complete
            if (gameSession.Status == GameStatus.Won || gameSession.Status == GameStatus.Lost)
            {
                await UpdateUserGameStatisticsAsync(userId, gameSession.Status == GameStatus.Won, gameSession.Score, gameSession.AttemptsCount);
            }
            
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User {UserId} made guess {Guess} in game {GameId}, result: {Result}", 
                userId, request.GuessedNumber, gameSessionId, attempt.Result);

            return Result<GameAttemptDto>.Success(MapToGameAttemptDto(attempt));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error making guess for user {UserId} in game {GameId}", userId, gameSessionId);
            return Result<GameAttemptDto>.Failure("Failed to process guess");
        }
    }

    public async Task<Result<GameSessionDto>> GetGameSessionAsync(string userId, Guid gameSessionId)
    {
        try
        {
            var gameSession = await _unitOfWork.GameSessions.GetGameWithAttemptsAsync(gameSessionId);
            if (gameSession == null)
            {
                return Result<GameSessionDto>.Failure("Game session not found");
            }

            if (gameSession.UserGameStatistics.UserId != userId)
            {
                return Result<GameSessionDto>.Failure("You are not authorized to access this game");
            }

            return Result<GameSessionDto>.Success(MapToGameSessionDto(gameSession));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting game session {GameId} for user {UserId}", gameSessionId, userId);
            return Result<GameSessionDto>.Failure("Failed to retrieve game session");
        }
    }

    public async Task<Result<GameStatsDto>> GetUserStatsAsync(string userId)
    {
        try
        {
            var userStats = await GetOrCreateUserGameStatisticsAsync(userId);

            var stats = new GameStatsDto
            {
                TotalGames = userStats.GamesPlayed,
                GamesWon = userStats.GamesWon,
                TotalScore = userStats.TotalScore,
                WinRate = userStats.WinRate,
                BestAttempts = userStats.BestAttempts
            };

            return Result<GameStatsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stats for user {UserId}", userId);
            return Result<GameStatsDto>.Failure("Failed to retrieve statistics");
        }
    }

    public async Task<Result<PagedResult<GameSessionDto>>> GetGameHistoryAsync(string userId, int page, int pageSize, GameStatus? status = null, GameDifficulty? difficulty = null)
    {
        try
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Limit page size

            var (games, totalCount) = await _unitOfWork.GameSessions.GetGamesByUserAsync(userId, page, pageSize, status, difficulty);
            
            var gameSessionDtos = games.Select(MapToGameSessionDto).ToList();

            var pagedResult = new PagedResult<GameSessionDto>
            {
                Items = gameSessionDtos,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<GameSessionDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting game history for user {UserId}, page {Page}, pageSize {PageSize}", userId, page, pageSize);
            return Result<PagedResult<GameSessionDto>>.Failure("Failed to retrieve game history");
        }
    }

    #region Private Helper Methods

    private static (int minRange, int maxRange, int maxAttempts) GetGameConfiguration(CreateGameSessionDto request)
    {
        // Use custom settings if provided
        if (request.CustomMinRange.HasValue && request.CustomMaxRange.HasValue)
        {
            return (
                request.CustomMinRange.Value, 
                request.CustomMaxRange.Value, 
                request.CustomMaxAttempts ?? 10
            );
        }

        // Use difficulty-based settings
        return request.Difficulty switch
        {
            GameDifficulty.Easy => (1, 30, 10),
            GameDifficulty.Normal => (1, 43, 8),  // Main requirement: 1-43 range
            GameDifficulty.Hard => (1, 60, 6),
            GameDifficulty.Expert => (1, 100, 5),
            _ => (1, 43, 8) // Default to requirement specs
        };
    }

    private static int CalculateScore(int attempts, int maxAttempts, GameDifficulty difficulty)
    {
        var baseScore = difficulty switch
        {
            GameDifficulty.Easy => 100,
            GameDifficulty.Normal => 200,
            GameDifficulty.Hard => 400,
            GameDifficulty.Expert => 800,
            _ => 200
        };

        // Bonus for fewer attempts
        var attemptBonus = (maxAttempts - attempts + 1) * 10;
        return baseScore + attemptBonus;
    }

    private static string GetSmartHint(int guess, int secret, string direction)
    {
        var difference = Math.Abs(guess - secret);
        var proximity = difference switch
        {
            <= 5 => "Very close! ",
            <= 10 => "Close! ",
            <= 20 => "Getting warmer... ",
            _ => ""
        };

        return $"{proximity}Try {direction}!";
    }

    private static GameSessionDto MapToGameSessionDto(GameSession gameSession)
    {
        return new GameSessionDto
        {
            Id = gameSession.Id,
            UserId = gameSession.UserGameStatistics.UserId,
            AttemptsCount = gameSession.AttemptsCount,
            MaxAttempts = gameSession.MaxAttempts,
            Status = gameSession.Status,
            Score = gameSession.Score,
            StartedAt = gameSession.StartedAt,
            EndedAt = gameSession.EndedAt,
            MinRange = gameSession.MinRange,
            MaxRange = gameSession.MaxRange,
            Difficulty = gameSession.Difficulty,
            Attempts = gameSession.Attempts?.Select(MapToGameAttemptDto).ToList() ?? new List<GameAttemptDto>()
        };
    }

    private static GameAttemptDto MapToGameAttemptDto(GameAttempt attempt)
    {
        return new GameAttemptDto
        {
            Id = attempt.Id,
            GuessedNumber = attempt.GuessedNumber,
            AttemptNumber = attempt.AttemptNumber,
            Result = attempt.Result,
            Hint = attempt.Hint,
            AttemptedAt = attempt.AttemptedAt
        };
    }

    private async Task UpdateUserGameStatisticsAsync(string userId, bool isWin, int score, int attempts = 0)
    {
        var userStats = await GetOrCreateUserGameStatisticsAsync(userId);
        await _unitOfWork.UserGameStatistics.UpdateStatisticsAsync(userId, score, isWin, attempts);
    }

    /// <summary>
    /// Gets existing user game statistics or creates new ones if they don't exist
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>UserGameStatistics entity</returns>
    private async Task<UserGameStatistics> GetOrCreateUserGameStatisticsAsync(string userId)
    {
        var userStats = await _unitOfWork.UserGameStatistics.GetByUserIdAsync(userId);
        
        if (userStats == null)
        {
            // Create initial statistics if they don't exist
            userStats = await _unitOfWork.UserGameStatistics.CreateInitialStatisticsAsync(userId);
            await _unitOfWork.SaveChangesAsync();
        }

        return userStats;
    }

    #endregion
}