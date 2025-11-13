using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Guess.Infrastructure.Data.Configurations;

/// <summary>
/// Database seeding configuration for initial data
/// </summary>
public class DatabaseSeedConfiguration
{
    /// <summary>
    /// Seeds default roles into the database
    /// </summary>
    /// <param name="modelBuilder">The model builder instance</param>
    public static void SeedRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "admin-role-id",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "admin-concurrency-stamp"
            },
            new IdentityRole
            {
                Id = "player-role-id", 
                Name = "Player",
                NormalizedName = "PLAYER",
                ConcurrencyStamp = "player-concurrency-stamp"
            },
            new IdentityRole
            {
                Id = "moderator-role-id",
                Name = "Moderator", 
                NormalizedName = "MODERATOR",
                ConcurrencyStamp = "moderator-concurrency-stamp"
            }
        );
    }

    /// <summary>
    /// Seeds game difficulty configurations
    /// </summary>
    /// <param name="modelBuilder">The model builder instance</param>
    public static void SeedGameConfigurations(ModelBuilder modelBuilder)
    {
        // This could be extended to seed default game configurations
        // For example, predefined difficulty settings, achievements, etc.
        
        // Example: Seed default game statistics or leaderboard data
        // modelBuilder.Entity<GameStatistics>().HasData(...);
    }

    /// <summary>
    /// Applies all seeding configurations
    /// </summary>
    /// <param name="modelBuilder">The model builder instance</param>
    public static void ApplySeeding(ModelBuilder modelBuilder)
    {
        SeedRoles(modelBuilder);
        SeedGameConfigurations(modelBuilder);
    }
}