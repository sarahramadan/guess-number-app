namespace Guess.Domain.Exceptions;

/// <summary>
/// Exception for authentication-related failures
/// </summary>
public class AuthenticationException : Exception
{
    public AuthenticationException(string message) : base(message)
    {
    }

    public AuthenticationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}