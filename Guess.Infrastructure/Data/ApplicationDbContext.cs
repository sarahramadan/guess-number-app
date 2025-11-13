using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Guess.Domain.Entities;
using Guess.Infrastructure.Data.Configurations;

namespace Guess.Infrastructure.Data;

/// <summary>
/// Main database context for the Guess Number application
/// Inherits from IdentityDbContext to support ASP.NET Core Identity
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Game sessions table
    /// </summary>
    public DbSet<GameSession> GameSessions { get; set; }

    /// <summary>
    /// Game attempts table
    /// </summary>
    public DbSet<GameAttempt> GameAttempts { get; set; }

    /// <summary>
    /// User game statistics table
    /// </summary>
    public DbSet<UserGameStatistics> UserGameStatistics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatically apply all entity configurations from the current assembly
        // This includes both domain entities and Identity table configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Apply database seeding
        DatabaseSeedConfiguration.ApplySeeding(modelBuilder);
    }
}