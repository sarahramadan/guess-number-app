namespace Guess.Application.Interfaces;

/// <summary>
/// Unit of Work interface to manage transactions and coordinate repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Game session repository
    /// </summary>
    IGameSessionRepository GameSessions { get; }

    /// <summary>
    /// Game attempt repository
    /// </summary>
    IGameAttemptRepository GameAttempts { get; }

    /// <summary>
    /// User game statistics repository
    /// </summary>
    IUserGameStatisticsRepository UserGameStatistics { get; }

    /// <summary>
    /// Saves all changes to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a database transaction
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    Task CommitAsync();

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    Task RollbackAsync();
}