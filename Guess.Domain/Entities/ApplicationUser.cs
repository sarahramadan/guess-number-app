using Microsoft.AspNetCore.Identity;

namespace Guess.Domain.Entities;

/// <summary>
/// Represents a user in the Guess Number application
/// Extends IdentityUser to leverage ASP.NET Core Identity features
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's full display name
    /// </summary>
    public string DisplayName => $"{FirstName} {LastName}".Trim();
    
    /// <summary>
    /// Date when the user account was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date when the user account was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Indicates if the user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}