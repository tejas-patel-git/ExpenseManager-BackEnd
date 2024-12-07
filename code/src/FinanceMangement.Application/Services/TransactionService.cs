using FinanceManager.Data.Models;
using FinanceManager.Data.Repository;
using FinanceManager.Models;
using FinanceMangement.Application.Mappers;
using Microsoft.Extensions.Logging;

namespace FinanceMangement.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<TransactionService> _logger;

        /// <inheritdoc/>
        public TransactionService(ITransactionRepository transactionRepository, ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<TransactionDto?> GetTransactionByIdAsync(int transactionId)
        {
            if (transactionId <= 0)
            {
                _logger.LogWarning($"Invalid transaction ID: {transactionId}");
                throw new ArgumentException($"Transaction ID must be greater than zero.", nameof(transactionId));
            }

            // Fetch data from repository
            var transactions = await _transactionRepository.GetTransactionByIdAsync(transactionId);

            // Return null if not found
            if (transactions == null) return null;

            // Return dto of fetched data
            return transactions.MapToDto();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning($"Invalid user ID: {userId}");
                throw new ArgumentException($"User ID must be greater than zero.", nameof(userId));
            }

            // Fetch data from repository
            var transactions = await _transactionRepository.GetAllTransactionsAsync(userId);

            // Return empty collection if not found
            if(!transactions.Any()) return Enumerable.Empty<TransactionDto>();

            // Return dto of fetched data
            return transactions.MapToDto();
        }

        /// <inheritdoc/>
        public async Task AddTransactionAsync(TransactionDto transactionDto)
        {
            ArgumentNullException.ThrowIfNull(transactionDto);

            // TODO : Validate userId witht the transaction's userId

            // Map dto model to entity model
            var transaction = transactionDto.MapToEntity();

            // Add data to repository
            await _transactionRepository.AddTransactionAsync(transaction);
        }

        /// <inheritdoc/>
        public async Task UpdateTransactionAsync(TransactionDto transactionDto)
        {
            ArgumentNullException.ThrowIfNull(transactionDto);

            // Map dto model to entity model
            var transaction = transactionDto.MapToEntity();

            // Update data to repository
            await _transactionRepository.UpdateTransactionAsync(transaction);
        }

        /// <inheritdoc/>
        public async Task DeleteTransactionAsync(int transactionId)
        {
            if (transactionId <= 0)
            {
                _logger.LogWarning($"Invalid transaction ID: {transactionId}");
                throw new ArgumentException($"Transaction ID must be greater than zero.", nameof(transactionId));
            }

            // Delete data from repository
            await _transactionRepository.DeleteTransactionAsync(transactionId);
        }
    }
}
