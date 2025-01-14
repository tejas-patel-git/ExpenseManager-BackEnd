using FinanceManager.Domain.Abstraction;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Abstraction.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace FinanceManager.Data.Repository;

internal class Repository<TDomain, TEntity, TId> : IRepository<TDomain, TEntity, TId>
    where TDomain : class, IDomainModel<TId>
    where TEntity : class, IEntityModel<TId>
{
    protected readonly AppDbContext Context;
    private readonly ILogger<Repository<TDomain, TEntity, TId>> _logger;
    protected readonly IMapper<TEntity, TDomain> entityDomainMapper;
    protected readonly IMapper<TDomain, TEntity> domainEntityMapper;
    protected readonly DbSet<TEntity> dbSet;

    internal Repository(AppDbContext context,
                        ILogger<Repository<TDomain, TEntity, TId>> logger,
                        IMapper<TDomain, TEntity> domainEntityMapper,
                        IMapper<TEntity, TDomain> entityDomainMapper)
    {
        this.Context = context;
        _logger = logger;
        this.entityDomainMapper = entityDomainMapper;
        this.domainEntityMapper = domainEntityMapper;
        dbSet = context.Set<TEntity>();
    }

    protected IQueryable<TEntity> FilterQuery(Expression<Func<TEntity, bool>> filter, IQueryable<TEntity>? query = null)
    {
        query ??= dbSet;
        return query.Where(filter);
    }

    protected IOrderedQueryable<TEntity> OrderQuery<TResult>(Expression<Func<TEntity, TResult>> orderBy,
                                                             IQueryable<TEntity>? query = null)
    {
        query ??= dbSet;
        return query.OrderBy(orderBy);
    }

    public virtual async Task<IEnumerable<TDomain>> GetAllAsync(Expression<Func<TEntity, bool>> filter)
    {
        var entities = await FilterQuery(filter).ToListAsync();
        return entityDomainMapper.Map(entities);
    }

    public virtual async Task<IEnumerable<TDomain>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> orderBy)
    {
        var entities = await OrderQuery(orderBy).ToListAsync();
        return entityDomainMapper.Map(entities);
    }

    public virtual async Task<IEnumerable<TDomain>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>> filter,
                                                                         Expression<Func<TEntity, TResult>> orderBy)
    {
        IQueryable<TEntity> query = dbSet;

        query = FilterQuery(filter, query);
        query = OrderQuery(orderBy, query);

        var entities = await query.ToListAsync();
        return entityDomainMapper.Map(entities);
    }

    public virtual async Task<TDomain?> GetByIdAsync(TId id)
    {
        try
        {
            var entity = await dbSet.FindAsync(id);

            if (entity == null)
            {
                _logger.LogInformation($"'{typeof(TEntity).Name}' having id {id} not found.");
                return null;
            }

            return entityDomainMapper.Map(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while retrieving '{typeof(TEntity).Name}' having id {id}");
            throw;
        }
    }

    protected virtual async Task<TEntity?> GetEntity(TId id)
    {
        try
        {
            var entity = await dbSet.FindAsync(id);

            if (entity == null)
            {
                _logger.LogInformation($"'{typeof(TEntity).Name}' having id {id} not found.");
                return null;
            }

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while retrieving '{typeof(TEntity).Name}' having id {id}");
            throw;
        }
    }

    public virtual async Task<TDomain> AddAsync(TDomain domain)
    {
        try
        {
            var entity = domainEntityMapper.Map(domain);

            if (entity is IAuditableEntity auditableEntity)
            {
                auditableEntity.CreatedAt = DateTime.UtcNow;
                auditableEntity.UpdatedAt = DateTime.UtcNow;
            }

            await AddAsync(entity);

            return entityDomainMapper.Map(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while adding a '{typeof(TEntity).Name}'.");
            throw;
        }
    }

    private async Task AddAsync(TEntity entity)
    {
        await dbSet.AddAsync(entity);
        _logger.LogInformation("'{type}' added successfully with Id {id}.", typeof(TEntity).Name, entity.Id);
    }

    protected virtual async Task AddAsync(TDomain domain, Action<TEntity> updateProperties)
    {
        try
        {
            var entity = domainEntityMapper.Map(domain);
            updateProperties(entity);
            await AddAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while adding a '{typeof(TEntity).Name}'.");
            throw;
        }
    }

    protected void Delete(TEntity entity)
    {
        dbSet.Remove(entity);
    }

    public virtual async Task<bool> DeleteByIdAsync(TId id)
    {
        try
        {
            var entity = await GetEntity(id);

            if (entity == null)
            {
                _logger.LogInformation($"'{typeof(TEntity).Name}' with id {id} not found for deletion.");
                return false;
            }

            Delete(entity);
            _logger.LogInformation($"'{typeof(TEntity).Name}' with id {id} deleted successfully.");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while deleting '{typeof(TEntity).Name}' id {id}.");
            throw;
        }
    }

    public virtual async Task UpdateAsync(TDomain domainToUpdate)
    {
        try
        {
            var entity = domainEntityMapper.Map(domainToUpdate);
            var existingTransaction = await GetByIdAsync(entity.Id);

            if (existingTransaction == null) return;

            dbSet.Update(entity);
            _logger.LogInformation($"'{typeof(TEntity).Name}' with Id {entity.Id} updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating '{typeof(TEntity).Name}'.");
            throw;
        }
    }

    public virtual async Task<bool> ExistsAsync(TId id)
    {
        return await GetByIdAsync(id) != null;
    }

    public virtual async Task<bool> ExistsAsync(ICollection<TId> ids)
    {
        var validIds = await dbSet.Select(x => x.Id).ToListAsync();
        return ids.All(x => validIds.Contains(x));
    }

    public virtual async Task<bool> ExistsAsync(ICollection<TId> ids, Expression<Func<TEntity, bool>> filter)
    {
        var validIds = await FilterQuery(filter).Select(x => x.Id).ToListAsync();
        return ids.All(x => validIds.Contains(x));
    }
}