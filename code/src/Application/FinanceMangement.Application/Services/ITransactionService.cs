using FinanceManager.Domain.Models;
using FinanceManager.Models.Request;

namespace FinanceManager.Application.Services
{
    /// <summary>
    /// Defines methods for managing transactions in the system.
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Retrieves a transaction by its unique ID.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to retrieve. Must be greater than zero.</param>
        /// <param name="userId"></param>
        /// <returns>
        /// The <see cref="TransactionDomain"/> if found; otherwise, <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="transactionId"/> is less than or equal to zero.</exception>
        Task<TransactionDomain?> GetUserTransactionAsync(Guid transactionId, string userId);

        /// <summary>
        /// Retrieves all transactions for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user. Must be greater than zero.</param>
        /// <returns>
        /// A collection of <see cref="TransactionDomain"/> objects for the specified user. If no transactions exist, returns an empty collection.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="userId"/> is less than or equal to zero.</exception>
        Task<IEnumerable<TransactionDomain>> GetUserTransactionsAsync(string userId);

        /// <summary>
        /// Adds a new transaction.
        /// </summary>
        /// <param name="transaction">The <see cref="TransactionRequest"/> object to add. Cannot be <c>null</c>.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="transaction"/> is <c>null</c>.</exception>
        Task<bool> AddTransactionAsync(TransactionDomain transaction);

        /// <summary>
        /// Updates an existing transaction.
        /// </summary>
        /// <param name="transaction">The updated <see cref="TransactionRequest"/> object. Must not be <c>null</c>.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="transaction"/> is <c>null</c>.</exception>
        Task UpdateTransactionAsync(TransactionDomain transaction);

        /// <summary>
        /// Deletes a transaction from the system by its unique ID.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to delete. Must be greater than zero.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="transactionId"/> is less than or equal to zero.</exception>
        Task DeleteTransactionAsync(Guid transactionId);
    }
}