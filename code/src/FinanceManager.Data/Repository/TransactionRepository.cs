﻿using FinanceManager.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Data.Repository;

internal class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
{
    private readonly ILogger<TransactionRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionRepository"/> class.
    /// </summary>
    /// <param name="context">The <see cref="AppDbContext"/> instance used to interact with the database.</param>
    /// <param name="logger">The <see cref="ILogger{TransactionRepository}"/> instance for logging repository activities.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when either <paramref name="context"/> or <paramref name="logger"/> is <c>null</c>.
    /// </exception>
    internal TransactionRepository(AppDbContext context,
                                 ILogger<TransactionRepository> logger) : base(context, logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Transaction?> GetTransactionByIdAsync(int transactionId)
    {
        try
        {
            var transaction = await dbSet.FindAsync(transactionId);

            if (transaction == null)
            {
                _logger.LogWarning($"Transaction with ID {transactionId} not found.");
            }

            return transaction;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while retrieving transaction ID {transactionId}");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
    {
        try
        {
            return await dbSet.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all transactions.");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync(int userID)
    {
        try
        {
            return await dbSet.Where(t => t.UserID == userID).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while retrieving transactions for UserID {userID}");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task AddTransactionAsync(Transaction transaction)
    {
        try
        {
            await dbSet.AddAsync(transaction);
            _logger.LogInformation($"Transaction added successfully with ID {transaction.TransactionID}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding a transaction.");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateTransactionAsync(Transaction transaction)
    {
        // TODO : Revisit logic - might need some tweaks related to how to handle FKs
        try
        {
            var existingTransaction = await GetTransactionByIdAsync(transaction.TransactionID);

            if (existingTransaction == null) return;

            existingTransaction.IsExpense = transaction.IsExpense;
            existingTransaction.UpdatedAt = DateTime.UtcNow;
            existingTransaction.Date = transaction.Date;
            existingTransaction.Amount = transaction.Amount;
            existingTransaction.Description = transaction.Description;

            _logger.LogInformation($"Transaction with ID {transaction.TransactionID} updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating transaction ID {transaction.TransactionID}.");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteTransactionAsync(int transactionId)
    {
        try
        {
            var transaction = await GetTransactionByIdAsync(transactionId);

            if (transaction == null)
            {
                _logger.LogWarning($"Transaction with ID {transactionId} not found for deletion.");
                return;
            }

            dbSet.Remove(transaction);
            _logger.LogInformation($"Transaction with ID {transactionId} deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while deleting transaction ID {transactionId}.");
            throw;
        }
    }
}