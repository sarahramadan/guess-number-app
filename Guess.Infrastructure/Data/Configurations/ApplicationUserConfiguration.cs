using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Guess.Domain.Entities;

namespace Guess.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for ApplicationUser entity
/// </summary>
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // Table configuration
        builder.ToTable("AspNetUsers");

        // Properties configuration
        builder.Property(e => e.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        // Indexes
        builder.HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("IX_AspNetUsers_Email");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_AspNetUsers_CreatedAt");
    }
}