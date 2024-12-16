using FinanceManager.Data.Repository;

namespace FinanceManager.Data;

/// <summary>
/// Interface for managing database transactions and repositories.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the user repository.
    /// </summary>
    IUserRepository UserRepository { get; }

    /// <summary>
    /// Gets the user repository.
    /// </summary>
    ITransactionRepository TransactionRepository { get; }

    /// <summary>
    /// Saves all pending changes to the database.
    /// </summary>
    /// <returns>The number of affected rows.</returns>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Begins a database transaction.
    /// </summary>
    void BeginTransaction();

    /// <summary>
    /// Commits the current database transaction.
    /// </summary>
    void CommitTransaction();

    /// <summary>
    /// Rolls back the current database transaction.
    /// </summary>
    void RollbackTransaction();
}
