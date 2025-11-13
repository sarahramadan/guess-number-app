namespace Guess.Domain.Exceptions;

/// <summary>
/// Exception for business logic violations
/// </summary>
public class BusinessLogicException : Exception
{
    public BusinessLogicException(string message) : base(message)
    {
    }

    public BusinessLogicException(string message, Exception innerException) : base(message, innerException)
    {
    }
}