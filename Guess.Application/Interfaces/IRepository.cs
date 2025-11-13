using System.Linq.Expressions;

namespace Guess.Application.Interfaces;

/// <summary>
/// Generic repository interface for common CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets entity by ID
    /// </summary>
    Task<T?> GetByIdAsync(object id);

    /// <summary>
    /// Gets all entities
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Finds entities matching the given expression
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);

    /// <summary>
    /// Gets first entity matching the expression
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression);

    /// <summary>
    /// Adds a new entity
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Adds multiple entities
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Updates an entity
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Removes an entity
    /// </summary>
    void Remove(T entity);

    /// <summary>
    /// Removes multiple entities
    /// </summary>
    void RemoveRange(IEnumerable<T> entities);

    /// <summary>
    /// Checks if any entity matches the expression
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> expression);

    /// <summary>
    /// Counts entities matching the expression
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>> expression);

    /// <summary>
    /// Gets paged results
    /// </summary>
    Task<(IEnumerable<T> items, int totalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, object>>? orderBy = null,
        bool ascending = true);
}