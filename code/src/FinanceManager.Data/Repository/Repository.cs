﻿using FinanceManager.Domain.Abstraction;
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

    public virtual async Task<IEnumerable<TDomain>> GetAllAsync(Expression<Func<TDomain, bool>>? filter = null,
                                            Func<IQueryable<TDomain>, IOrderedQueryable<TDomain>>? orderBy = null)
    {
        // TODO : Adding mapping for Func delegates.W

        //IQueryable<TEntity> query = dbSet;

        //if (filter != null)
        //{
        //    query = query.Where(filter);
        //}

        //foreach (var includeProperty in includeProperties.Split
        //    ([','], StringSplitOptions.RemoveEmptyEntries))
        //{
        //    query = query.Include(includeProperty);
        //}

        //if (orderBy != null)
        //{
        //    return await orderBy(query).ToListAsync();
        //}
        //else
        //{
        //    return await query.ToListAsync();
        //}
        throw new NotImplementedException();
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

    public virtual async Task AddAsync(TDomain domain)
    {
        try
        {
            var entity = domainEntityMapper.Map(domain);

            await AddAsync(entity);
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

    private void Delete(TDomain domainToDelete)
    {
        var entityToDelete = domainEntityMapper.Map(domainToDelete);

        dbSet.Remove(entityToDelete);
    }

    public async Task DeleteByIdAsync(TId id)
    {
        try
        {
            var domain = await GetByIdAsync(id);

            if (domain == null)
            {
                _logger.LogInformation($"'{typeof(TEntity).Name}' with id {id} not found for deletion.");
                return;
            }

            Delete(domain);
            _logger.LogInformation($"'{typeof(TEntity).Name}' with id {id} deleted successfully.");
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
}