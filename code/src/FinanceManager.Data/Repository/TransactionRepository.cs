using FinanceManager.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Data.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TransactionRepository> _logger;

        public TransactionRepository(AppDbContext context, ILogger<TransactionRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves a transaction by its ID asynchronously.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to retrieve.</param>
        /// <returns>The <see cref="Transaction"/> entity if found; otherwise, null.</returns>
        public async Task<Transaction?> GetTransactionByIdAsync(int transactionId)
        {
            if (transactionId <= 0)
            {
                _logger.LogWarning("Invalid transaction ID: {TransactionId}", transactionId);
                throw new ArgumentException("Transaction ID must be greater than zero.", nameof(transactionId));
            }

            try
            {
                var transaction = await _context.Transactions.FindAsync(transactionId);

                if (transaction == null)
                {
                    _logger.LogWarning("Transaction with ID {TransactionId} not found.", transactionId);
                }

                return transaction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving transaction ID {TransactionId}", transactionId);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all transactions asynchronously.
        /// </summary>
        /// <returns>A collection of <see cref="Transaction"/>.</returns>
        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            try
            {
                return await _context.Transactions.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all transactions.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves all transactions for a specific user asynchronously.
        /// </summary>
        /// <param name="userID">The user ID to filter transactions.</param>
        /// <returns>A collection of <see cref="Transaction"/> for the specified user.</returns>
        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync(int userID)
        {
            if (userID <= 0)
            {
                _logger.LogWarning("Invalid user ID: {UserID}", userID);
                throw new ArgumentException("User ID must be greater than zero.", nameof(userID));
            }

            try
            {
                return await _context.Transactions.Where(t => t.UserID == userID).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving transactions for UserID {UserID}", userID);
                throw;
            }
        }

        /// <summary>
        /// Adds a new transaction asynchronously.
        /// </summary>
        /// <param name="transaction">The transaction entity to add.</param>
        public async Task AddTransactionAsync(Transaction transaction)
        {
            if (transaction == null)
            {
                _logger.LogWarning("Cannot add a null transaction.");
                throw new ArgumentNullException(nameof(transaction));
            }

            try
            {
                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Transaction added successfully with ID {transaction.TransactionID}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a transaction.");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing transaction asynchronously.
        /// </summary>
        /// <param name="transaction">The transaction entity to update.</param>
        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            if (transaction == null)
            {
                _logger.LogWarning("Cannot update a null transaction.");
                throw new ArgumentNullException(nameof(transaction));
            }

            try
            {
                _context.Update(transaction);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Transaction with ID {transaction.TransactionID} updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating transaction ID {transaction.TransactionID}.");
                throw;
            }
        }

        /// <summary>
        /// Deletes a transaction asynchronously.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to delete.</param>
        public async Task DeleteTransactionAsync(int transactionId)
        {
            if (transactionId <= 0)
            {
                _logger.LogWarning("Invalid transaction ID: {TransactionId}", transactionId);
                throw new ArgumentException("Transaction ID must be greater than zero.", nameof(transactionId));
            }

            try
            {
                var transaction = await GetTransactionByIdAsync(transactionId);

                if (transaction == null)
                {
                    _logger.LogWarning("Transaction with ID {TransactionId} not found for deletion.", transactionId);
                    return;
                }

                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Transaction with ID {TransactionId} deleted successfully.", transactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting transaction ID {TransactionId}.", transactionId);
                throw;
            }
        }
    }
}
