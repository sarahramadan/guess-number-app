using Guess.Application.DTOs;
using Guess.Application.Interfaces;
using Guess.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using System.Net;

namespace Guess.Api.Controllers
{
    /// <summary>
    /// Game controller for managing number guessing games
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class GameController : BaseController
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        /// <summary>
        /// Create a new game session
        /// </summary>
        /// <param name="request">Game creation parameters</param>
        /// <returns>Created game session details</returns>
        [HttpPost]
        public async Task<IActionResult> CreateGameAsync([FromBody] CreateGameSessionDto request)
        {
            var userId = RequireAuthenticatedUser();
            var result = await _gameService.CreateGameSessionAsync(userId, request);
            
            if (!result.IsSuccess)
            {
                if (result.Errors.Any())
                {
                    return BadRequest(result.Error ?? "Game creation failed", 
                        new Dictionary<string, string[]> { { "General", result.Errors.ToArray() } });
                }
                return BadRequest(result.Error ?? "Game creation failed");
            }

            return Created(result.Data, $"/api/game/{result.Data!.Id}");
        }

        /// <summary>
        /// Make a guess in a game session
        /// </summary>
        /// <param name="gameSessionId">Game session identifier</param>
        /// <param name="request">Guess details</param>
        /// <returns>Result of the guess attempt</returns>
        [HttpPost("{gameSessionId}/guess")]
        public async Task<IActionResult> MakeGuessAsync([FromRoute] Guid gameSessionId, [FromBody] MakeGuessDto request)
        {
            var userId = RequireAuthenticatedUser();
            var result = await _gameService.MakeGuessAsync(userId, gameSessionId, request);
            
            if (!result.IsSuccess)
            {
                if (result.Errors.Any())
                {
                    return BadRequest(result.Error ?? "Guess failed", 
                        new Dictionary<string, string[]> { { "General", result.Errors.ToArray() } });
                }
                return BadRequest(result.Error ?? "Guess failed");
            }

            return Success(result.Data!);
        }

        /// <summary>
        /// Get user's game statistics (shows best attempts when user logs back)
        /// </summary>
        /// <returns>User's game statistics</returns>
        [HttpGet("stats")]
        public async Task<IActionResult> GetUserStatsAsync()
        {
            var userId = RequireAuthenticatedUser();
            var result = await _gameService.GetUserStatsAsync(userId);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error ?? "Failed to retrieve user statistics");
            }

            return Success(result.Data!);
        }

        /// <summary>
        /// Get user's game history with pagination and filtering
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
        /// <param name="status">Filter by game status (optional)</param>
        /// <param name="difficulty">Filter by game difficulty (optional)</param>
        /// <returns>Paginated list of user's game sessions</returns>
        [HttpGet("history")]
        public async Task<IActionResult> GetGameHistoryAsync(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? difficulty = null)
        {
            var userId = RequireAuthenticatedUser();
            
            // Parse enum parameters
            GameStatus? gameStatus = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<GameStatus>(status, true, out var parsedStatus))
            {
                gameStatus = parsedStatus;
            }

            GameDifficulty? gameDifficulty = null;
            if (!string.IsNullOrEmpty(difficulty) && Enum.TryParse<GameDifficulty>(difficulty, true, out var parsedDifficulty))
            {
                gameDifficulty = parsedDifficulty;
            }

            var result = await _gameService.GetGameHistoryAsync(userId, page, pageSize, gameStatus, gameDifficulty);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error ?? "Failed to retrieve game history");
            }

            return Success(result.Data!);
        }

        /// <summary>
        /// Get details of a specific game session with all attempts
        /// </summary>
        /// <param name="gameSessionId">Game session identifier</param>
        /// <returns>Game session details with attempts</returns>
        [HttpGet("{gameSessionId}")]
        public async Task<IActionResult> GetGameSessionAsync([FromRoute] Guid gameSessionId)
        {
            var userId = RequireAuthenticatedUser();
            var result = await _gameService.GetGameSessionAsync(userId, gameSessionId);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error ?? "Failed to retrieve game session");
            }

            return Success(result.Data!);
        }
    }
}