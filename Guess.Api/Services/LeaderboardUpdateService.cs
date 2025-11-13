using Guess.Application.Interfaces;
using Guess.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Guess.Api.Services;

/// <summary>
/// Background service for updating user statistics and maintaining leaderboard cache
/// </summary>
public class LeaderboardUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LeaderboardUpdateService> _logger;
    private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(5); // Update every 5 minutes

    public LeaderboardUpdateService(
        IServiceProvider serviceProvider,
        ILogger<LeaderboardUpdateService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Leaderboard Update Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateUserStatistics();
                await Task.Delay(_updateInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating leaderboard statistics");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Wait 1 minute before retry
            }
        }

        _logger.LogInformation("Leaderboard Update Service stopped");
    }

    private async Task UpdateUserStatistics()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        try
        {
            _logger.LogDebug("Starting user statistics initialization check");

            // Get all users who don't have statistics records yet
            var usersWithoutStats = await context.Users
                .Where(u => !context.UserGameStatistics.Any(ugs => ugs.UserId == u.Id))
                .Select(u => u.Id)
                .ToListAsync();

            foreach (var userId in usersWithoutStats)
            {
                // Create initial statistics for users who don't have them
                var userStats = await unitOfWork.UserGameStatistics.CreateInitialStatisticsAsync(userId);
                
                // For migration purposes, we need to handle existing games that might not be linked yet
                // This is a one-time migration scenario
                _logger.LogDebug("Created initial statistics for user {UserId}", userId);
            }

            await unitOfWork.SaveChangesAsync();
            
            if (usersWithoutStats.Any())
            {
                _logger.LogDebug("Created initial statistics for {Count} users", usersWithoutStats.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user statistics");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Leaderboard Update Service is stopping");
        await base.StopAsync(stoppingToken);
    }
}