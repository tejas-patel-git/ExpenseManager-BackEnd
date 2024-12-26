using System.Linq.Expressions;

namespace FinanceManager.Data.Repository;

// TODO : Add XML comments

/// <summary>
/// Generic repository interface for CRUD operations.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(object id);
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task DeleteByIdAsync(object id);
    Task<bool> ExistsAsync(object id);
    Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "");
    Task SaveChangesAsync();
}
