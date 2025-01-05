using System.Linq.Expressions;

namespace FinanceManager.Domain.Abstraction.Repository
{
    public interface IRepository<TDomain, TEntity, TId>
    where TDomain : class, IDomainModel<TId>
    where TEntity : class, IEntityModel<TId>
    {
        Task<TDomain?> GetByIdAsync(TId id);
        Task AddAsync(TDomain domainModel);
        Task UpdateAsync(TDomain domainModel);
        Task DeleteByIdAsync(TId id);
        Task<bool> ExistsAsync(TId id);
        Task<IEnumerable<TDomain>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> orderBy);
        Task<IEnumerable<TDomain>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> orderBy);
        Task<IEnumerable<TDomain>> GetAllAsync(Expression<Func<TEntity, bool>> filter);
    }
}
