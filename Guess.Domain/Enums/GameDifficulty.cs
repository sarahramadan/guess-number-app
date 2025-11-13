namespace Guess.Domain.Enums;

/// <summary>
/// Enumeration for game difficulty levels
/// </summary>
public enum GameDifficulty
{
    Easy = 1,     // 1-50, 15 attempts
    Normal = 2,   // 1-100, 10 attempts
    Hard = 3,     // 1-200, 8 attempts
    Expert = 4    // 1-500, 6 attempts
}