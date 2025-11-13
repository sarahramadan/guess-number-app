namespace Guess.Domain.Models;

/// <summary>
/// JWT configuration settings (immutable during runtime)
/// </summary>
public record JwtSettings
{
    public const string Section = "JwtSettings";
    
    public required string SecretKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int AccessTokenExpiryMinutes { get; init; } = 60;
    public int RefreshTokenExpiryDays { get; init; } = 7;
}