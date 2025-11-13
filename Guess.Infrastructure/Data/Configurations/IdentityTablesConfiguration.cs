using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Guess.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for ASP.NET Core Identity tables
/// Provides custom table names and configurations for Identity entities
/// </summary>
public class IdentityTablesConfiguration : IEntityTypeConfiguration<IdentityRole>,
                                          IEntityTypeConfiguration<IdentityUserRole<string>>,
                                          IEntityTypeConfiguration<IdentityUserClaim<string>>,
                                          IEntityTypeConfiguration<IdentityUserLogin<string>>,
                                          IEntityTypeConfiguration<IdentityRoleClaim<string>>,
                                          IEntityTypeConfiguration<IdentityUserToken<string>>
{
    /// <summary>
    /// Configures the IdentityRole entity
    /// </summary>
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.ToTable("Roles");
        
        // Add index for better query performance
        builder.HasIndex(r => r.NormalizedName)
            .IsUnique()
            .HasDatabaseName("IX_Roles_NormalizedName");
    }

    /// <summary>
    /// Configures the IdentityUserRole entity
    /// </summary>
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.ToTable("UserRoles");
        
        // Add indexes for better query performance
        builder.HasIndex(ur => ur.UserId)
            .HasDatabaseName("IX_UserRoles_UserId");
            
        builder.HasIndex(ur => ur.RoleId)
            .HasDatabaseName("IX_UserRoles_RoleId");
    }

    /// <summary>
    /// Configures the IdentityUserClaim entity
    /// </summary>
    public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
    {
        builder.ToTable("UserClaims");
        
        // Add index for better query performance
        builder.HasIndex(uc => uc.UserId)
            .HasDatabaseName("IX_UserClaims_UserId");
            
        builder.HasIndex(uc => uc.ClaimType)
            .HasDatabaseName("IX_UserClaims_ClaimType");
    }

    /// <summary>
    /// Configures the IdentityUserLogin entity
    /// </summary>
    public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
    {
        builder.ToTable("UserLogins");
        
        // Add index for better query performance
        builder.HasIndex(ul => ul.UserId)
            .HasDatabaseName("IX_UserLogins_UserId");
    }

    /// <summary>
    /// Configures the IdentityRoleClaim entity
    /// </summary>
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        builder.ToTable("RoleClaims");
        
        // Add index for better query performance
        builder.HasIndex(rc => rc.RoleId)
            .HasDatabaseName("IX_RoleClaims_RoleId");
            
        builder.HasIndex(rc => rc.ClaimType)
            .HasDatabaseName("IX_RoleClaims_ClaimType");
    }

    /// <summary>
    /// Configures the IdentityUserToken entity
    /// </summary>
    public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
    {
        builder.ToTable("UserTokens");
        
        // Add index for better query performance
        builder.HasIndex(ut => ut.UserId)
            .HasDatabaseName("IX_UserTokens_UserId");
    }
}