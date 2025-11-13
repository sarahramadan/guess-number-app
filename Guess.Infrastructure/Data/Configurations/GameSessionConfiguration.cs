using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Guess.Domain.Entities;

namespace Guess.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for GameSession entity
/// </summary>
public class GameSessionConfiguration : IEntityTypeConfiguration<GameSession>
{
    public void Configure(EntityTypeBuilder<GameSession> builder)
    {
        // Table configuration with constraints
        builder.ToTable("GameSessions", t =>
        {
            t.HasCheckConstraint("CK_GameSessions_MinRange", "\"MinRange\" >= 1");
            t.HasCheckConstraint("CK_GameSessions_MaxRange", "\"MaxRange\" >= \"MinRange\"");
            t.HasCheckConstraint("CK_GameSessions_MaxAttempts", "\"MaxAttempts\" >= 1");
            t.HasCheckConstraint("CK_GameSessions_AttemptsCount", "\"AttemptsCount\" >= 0");
            t.HasCheckConstraint("CK_GameSessions_Score", "\"Score\" >= 0");
        });

        // Primary key
        builder.HasKey(e => e.Id);

        // Properties configuration
        builder.Property(e => e.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(e => e.UserGameStatisticsId)
            .IsRequired();

        builder.Property(e => e.SecretNumber)
            .IsRequired();

        builder.Property(e => e.AttemptsCount)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(e => e.MaxAttempts)
            .HasDefaultValue(10)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Score)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(e => e.StartedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(e => e.EndedAt)
            .IsRequired(false);

        builder.Property(e => e.MinRange)
            .HasDefaultValue(1)
            .IsRequired();

        builder.Property(e => e.MaxRange)
            .HasDefaultValue(43) // Updated to match requirements
            .IsRequired();

        builder.Property(e => e.Difficulty)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Indexes for query performance
        builder.HasIndex(e => e.UserGameStatisticsId)
            .HasDatabaseName("IX_GameSessions_UserGameStatisticsId");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_GameSessions_Status");

        builder.HasIndex(e => e.StartedAt)
            .HasDatabaseName("IX_GameSessions_StartedAt");

        builder.HasIndex(e => e.EndedAt)
            .HasDatabaseName("IX_GameSessions_EndedAt");

        builder.HasIndex(e => e.Difficulty)
            .HasDatabaseName("IX_GameSessions_Difficulty");

        // Composite index for common queries
        builder.HasIndex(e => new { e.UserGameStatisticsId, e.Status })
            .HasDatabaseName("IX_GameSessions_UserGameStatisticsId_Status");

        builder.HasIndex(e => new { e.UserGameStatisticsId, e.StartedAt })
            .HasDatabaseName("IX_GameSessions_UserGameStatisticsId_StartedAt");

        // Relationships
        builder.HasOne(e => e.UserGameStatistics)
            .WithMany(u => u.GameSessions)
            .HasForeignKey(e => e.UserGameStatisticsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(gs => gs.Attempts)
            .WithOne(a => a.GameSession)
            .HasForeignKey(a => a.GameSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}