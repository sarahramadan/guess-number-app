namespace Guess.Domain.Exceptions;

/// <summary>
/// Exception for game-related business logic violations
/// </summary>
public class GameException : BusinessLogicException
{
    public GameException(string message) : base(message)
    {
    }

    public GameException(string message, Exception innerException) : base(message, innerException)
    {
    }
}