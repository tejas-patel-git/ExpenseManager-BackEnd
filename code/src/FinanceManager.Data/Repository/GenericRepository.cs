using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace FinanceManager.Data.Repository;

internal class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly AppDbContext _context;
    private readonly ILogger<Repository.GenericRepository<TEntity>> _logger;
    protected readonly DbSet<TEntity> dbSet;

    internal GenericRepository(AppDbContext context, ILogger<GenericRepository<TEntity>> logger)
    {
        _context = context;
        _logger = logger;
        dbSet = context.Set<TEntity>();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null,
                                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                            string includeProperties = "")
    {
        IQueryable<TEntity> query = dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (var includeProperty in includeProperties.Split
            ([','], StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync();
        }
        else
        {
            return await query.ToListAsync();
        }
    }

    public virtual async Task<TEntity?> GetByIdAsync(object id)
    {
        return await dbSet.FindAsync(id);
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await dbSet.AddAsync(entity);
    }

    public virtual void Delete(TEntity entityToDelete)
    {
        if (_context.Entry(entityToDelete).State == EntityState.Detached)
        {
            dbSet.Attach(entityToDelete);
        }
        dbSet.Remove(entityToDelete);
    }

    public async Task DeleteByIdAsync(object id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            Delete(entity);
        }
    }

    public virtual void Update(TEntity entityToUpdate)
    {
        dbSet.Attach(entityToUpdate);
        _context.Entry(entityToUpdate).State = EntityState.Modified;
    }

    public async Task<bool> ExistsAsync(object id)
    {
        var entity = await GetByIdAsync(id);
        return entity != null;
    }
}