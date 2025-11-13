using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Guess.Domain.Entities;

namespace Guess.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for UserGameStatistics entity
/// </summary>
public class UserGameStatisticsConfiguration : IEntityTypeConfiguration<UserGameStatistics>
{
    public void Configure(EntityTypeBuilder<UserGameStatistics> builder)
    {
        // Primary key
        builder.HasKey(u => u.Id);
        
        // Properties
        builder.Property(u => u.Id)
            .HasDefaultValueSql("gen_random_uuid()");
            
        builder.Property(u => u.UserId)
            .IsRequired()
            .HasMaxLength(450); // ASP.NET Core Identity default
            
        builder.Property(u => u.TotalScore)
            .HasDefaultValue(0);
            
        builder.Property(u => u.GamesPlayed)
            .HasDefaultValue(0);
            
        builder.Property(u => u.GamesWon)
            .HasDefaultValue(0);
            
        builder.Property(u => u.BestAttempts)
            .IsRequired(false); // Nullable - no attempts initially
            
        builder.Property(u => u.LastUpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        // Computed properties - ignore from database mapping
        builder.Ignore(u => u.WinRate);
        
        // Indexes
        builder.HasIndex(u => u.UserId)
            .IsUnique()
            .HasDatabaseName("IX_UserGameStatistics_UserId");
            
        builder.HasIndex(u => u.TotalScore)
            .HasDatabaseName("IX_UserGameStatistics_TotalScore");
            
        builder.HasIndex(u => u.GamesWon)
            .HasDatabaseName("IX_UserGameStatistics_GamesWon");
        
        // Relationships
        builder.HasOne(u => u.User)
            .WithOne()
            .HasForeignKey<UserGameStatistics>(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(u => u.GameSessions)
            .WithOne(gs => gs.UserGameStatistics)
            .HasForeignKey(gs => gs.UserGameStatisticsId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Table name
        builder.ToTable("UserGameStatistics");
    }
}