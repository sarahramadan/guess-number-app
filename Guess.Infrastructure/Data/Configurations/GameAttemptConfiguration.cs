using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Guess.Domain.Entities;

namespace Guess.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for GameAttempt entity
/// </summary>
public class GameAttemptConfiguration : IEntityTypeConfiguration<GameAttempt>
{
    public void Configure(EntityTypeBuilder<GameAttempt> builder)
    {
        // Table configuration with constraints
        builder.ToTable("GameAttempts", t =>
        {
            t.HasCheckConstraint("CK_GameAttempts_AttemptNumber", "\"AttemptNumber\" >= 1");
            t.HasCheckConstraint("CK_GameAttempts_GuessedNumber", "\"GuessedNumber\" >= 1");
        });

        // Primary key
        builder.HasKey(e => e.Id);

        // Properties configuration
        builder.Property(e => e.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        builder.Property(e => e.GameSessionId)
            .IsRequired();

        builder.Property(e => e.GuessedNumber)
            .IsRequired();

        builder.Property(e => e.AttemptNumber)
            .IsRequired();

        builder.Property(e => e.Result)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Hint)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(e => e.AttemptedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(e => e.TimeTaken)
            .IsRequired(false);

        // Indexes for query performance
        builder.HasIndex(e => e.GameSessionId)
            .HasDatabaseName("IX_GameAttempts_GameSessionId");

        builder.HasIndex(e => e.AttemptNumber)
            .HasDatabaseName("IX_GameAttempts_AttemptNumber");

        builder.HasIndex(e => e.AttemptedAt)
            .HasDatabaseName("IX_GameAttempts_AttemptedAt");

        builder.HasIndex(e => e.Result)
            .HasDatabaseName("IX_GameAttempts_Result");

        // Composite index for common queries
        builder.HasIndex(e => new { e.GameSessionId, e.AttemptNumber })
            .IsUnique()
            .HasDatabaseName("IX_GameAttempts_GameSessionId_AttemptNumber");

        builder.HasIndex(e => new { e.GameSessionId, e.AttemptedAt })
            .HasDatabaseName("IX_GameAttempts_GameSessionId_AttemptedAt");

        // Relationships
        builder.HasOne(e => e.GameSession)
            .WithMany(gs => gs.Attempts)
            .HasForeignKey(e => e.GameSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}