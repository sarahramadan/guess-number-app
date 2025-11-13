namespace Guess.Domain.Models;

/// <summary>
/// Database configuration settings (immutable during runtime)
/// </summary>
public record DatabaseSettings
{
    public const string Section = "ConnectionStrings";
    
    public required string DefaultConnection { get; init; }
}