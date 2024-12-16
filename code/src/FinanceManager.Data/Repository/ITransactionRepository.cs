using FinanceManager.Data.Models;

namespace FinanceManager.Data.Repository;

/// <summary>
/// Defines the contract for repository operations related to transactions.
/// </summary>
public interface ITransactionRepository : IGenericRepository<Transaction>
{
    /// <summary>
    /// Retrieves a transaction by its ID asynchronously.
    /// </summary>
    /// <param name="transactionId">The ID of the transaction to retrieve.</param>
    /// <returns>The <see cref="Transaction"/> entity if found; otherwise, <c>null</c>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="transactionId"/> is less than or equal to zero.</exception>
    Task<Transaction?> GetTransactionByIdAsync(int transactionId);

    /// <summary>
    /// Retrieves all transactions asynchronously.
    /// </summary>
    /// <returns>A collection of <see cref="Transaction"/>.</returns>
    Task<IEnumerable<Transaction>> GetAllTransactionsAsync();

    /// <summary>
    /// Retrieves all transactions for a specific user asynchronously.
    /// </summary>
    /// <param name="userID">The user ID to filter transactions.</param>
    /// <returns>A collection of <see cref="Transaction"/> for the specified user.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="userID"/> is less than or equal to zero.</exception>
    Task<IEnumerable<Transaction>> GetAllTransactionsAsync(int userID);

    /// <summary>
    /// Adds a new transaction asynchronously.
    /// </summary>
    /// <param name="transaction">The transaction entity to add.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="transaction"/> is <c>null</c>.</exception>
    Task AddTransactionAsync(Transaction transaction);

    /// <summary>
    /// Updates an existing transaction asynchronously.
    /// </summary>
    /// <param name="transaction">The transaction entity to update.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="transaction"/> is <c>null</c>.</exception>
    Task UpdateTransactionAsync(Transaction transaction);

    /// <summary>
    /// Deletes a transaction asynchronously.
    /// </summary>
    /// <param name="transactionId">The ID of the transaction to delete.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="transactionId"/> is less than or equal to zero.</exception>
    Task DeleteTransactionAsync(int transactionId);
}