using FinanceManager.Data.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Data;

/// <summary>
/// Implementation of the Unit of Work pattern.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<UnitOfWork> _logger;
    private IDbContextTransaction _transaction;

    private IUserRepository _userRepository;
    private ITransactionRepository _transactionRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="loggerFactory"></param>
    /// <param name="transactionRepository"></param>
    public UnitOfWork(AppDbContext context, ILoggerFactory loggerFactory, ITransactionRepository transactionRepository)
    {
        _context = context;
        _loggerFactory = loggerFactory;
        _transactionRepository = transactionRepository;
        _logger = loggerFactory.CreateLogger<UnitOfWork>();
    }

    /// <inheritdoc/>
    public IUserRepository UserRepository =>
        _userRepository ??= new UserRepository(_context, _loggerFactory.CreateLogger<UserRepository>());

    /// <inheritdoc/>
    public ITransactionRepository TransactionRepository => _transactionRepository;

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public void BeginTransaction()
    {
        _transaction = _context.Database.BeginTransaction();
    }

    /// <inheritdoc/>
    public void CommitTransaction()
    {
        _transaction?.Commit();
        _transaction?.Dispose();
    }

    /// <inheritdoc/>
    public void RollbackTransaction()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();

        GC.SuppressFinalize(this);
    }
}
