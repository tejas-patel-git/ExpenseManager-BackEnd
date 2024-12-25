using FinanceManager.Data;
using FinanceManager.Application.Mappers;
using Microsoft.Extensions.Logging;
using FinanceManager.Models.Response;
using FinanceManager.Models.Request;

namespace FinanceManager.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionService> _logger;

    /// <inheritdoc/>
    public TransactionService(IUnitOfWork unitOfWork, ILogger<TransactionService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<TransactionResponse?> GetTransactionByIdAsync(int transactionId)
    {
        if (transactionId <= 0)
        {
            _logger.LogWarning($"Invalid transaction ID: {transactionId}");
            throw new ArgumentException($"Transaction ID must be greater than zero.", nameof(transactionId));
        }

        // Fetch data from repository
        var transactions = await _unitOfWork.TransactionRepository.GetTransactionByIdAsync(transactionId);

        // Return null if not found
        if (transactions == null) return null;

        // Return dto of fetched data
        return transactions.MapToDto();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TransactionResponse>> GetAllTransactionsAsync(int userId)
    {
        if (userId <= 0)
        {
            _logger.LogWarning($"Invalid user ID: {userId}");
            throw new ArgumentException($"User ID must be greater than zero.", nameof(userId));
        }

        // Fetch data from repository
        var transactions = await _unitOfWork.TransactionRepository.GetAllTransactionsAsync(userId);

        // Return empty collection if not found
        if (!transactions.Any()) return Enumerable.Empty<TransactionResponse>();

        // Return dto of fetched data
        return transactions.MapToDto();
    }

    /// <inheritdoc/>
    public async Task AddTransactionAsync(TransactionRequest transactionDto)
    {
        ArgumentNullException.ThrowIfNull(transactionDto);

        // TODO : Validate userId with the transaction's userId

        // Map dto model to entity model
        var transaction = transactionDto.MapToEntity();

        // Add data to repository
        await _unitOfWork.TransactionRepository.AddTransactionAsync(transaction);
    }

    /// <inheritdoc/>
    public async Task UpdateTransactionAsync(TransactionRequest transactionDto)
    {
        ArgumentNullException.ThrowIfNull(transactionDto);

        // Map dto model to entity model
        var transaction = transactionDto.MapToEntity();

        // Update data to repository
        await _unitOfWork.TransactionRepository.UpdateTransactionAsync(transaction);
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
        await _unitOfWork.TransactionRepository.DeleteTransactionAsync(transactionId);
    }
}
