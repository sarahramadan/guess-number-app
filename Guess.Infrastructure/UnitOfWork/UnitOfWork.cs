using Microsoft.EntityFrameworkCore.Storage;
using Guess.Application.Interfaces;
using Guess.Infrastructure.Data;
using Guess.Infrastructure.Repositories;

namespace Guess.Infrastructure.UnitOfWork;

/// <summary>
/// Unit of Work implementation to manage transactions and coordinate repositories
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed = false;

    // Lazy-loaded repositories
    private IGameSessionRepository? _gameSessionRepository;
    private IGameAttemptRepository? _gameAttemptRepository;
    private IUserGameStatisticsRepository? _userGameStatisticsRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IGameSessionRepository GameSessions => 
        _gameSessionRepository ??= new GameSessionRepository(_context);

    public IGameAttemptRepository GameAttempts => 
        _gameAttemptRepository ??= new GameAttemptRepository(_context);

    public IUserGameStatisticsRepository UserGameStatistics => 
        _userGameStatisticsRepository ??= new UserGameStatisticsRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            await SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _context.Dispose();
            _disposed = true;
        }
    }
}