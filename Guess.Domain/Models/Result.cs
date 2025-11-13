namespace Guess.Domain.Models;

/// <summary>
/// Generic result class for service operations
/// </summary>
/// <typeparam name="T">Type of data returned on success</typeparam>
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string Error { get; }
    public List<string> Errors { get; }

    private Result(bool isSuccess, T? data, string error, List<string> errors)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        Errors = errors;
    }

    public static Result<T> Success(T data) => new(true, data, string.Empty, new List<string>());
    public static Result<T> Failure(string error) => new(false, default, error, new List<string>());
    public static Result<T> Failure(List<string> errors) => new(false, default, string.Empty, errors);
}

/// <summary>
/// Result class for operations that don't return data
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }
    public List<string> Errors { get; }

    private Result(bool isSuccess, string error, List<string> errors)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors;
    }

    public static Result Success() => new(true, string.Empty, new List<string>());
    public static Result Failure(string error) => new(false, error, new List<string>());
    public static Result Failure(List<string> errors) => new(false, string.Empty, errors);
}