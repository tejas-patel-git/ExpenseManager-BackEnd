using FinanceManager.Data;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using FinanceManager.Models.Request;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionService> _logger;
    private readonly IMapper<TransactionRequest, TransactionDomain> _requestDomainMapper;
    private readonly IUserService _userService;

    /// <inheritdoc/>
    public TransactionService(IUnitOfWork unitOfWork,
                              ILogger<TransactionService> logger,
                              IMapper<TransactionRequest, TransactionDomain> requestDomainMapper,
                              IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _requestDomainMapper = requestDomainMapper;
        _userService = userService;
    }

    /// <inheritdoc/>
    public async Task<TransactionDomain?> GetUserTransactionByIdAsync(Guid transactionId, string userId)
    {
        if (transactionId.Equals(Guid.Empty))
        {
            throw new ArgumentException($"Invalid transaction id.", nameof(transactionId));
        }
        ArgumentNullException.ThrowIfNullOrEmpty(userId);


        // Fetch data from repository
        var transactions = await _unitOfWork.TransactionRepository.GetByIdAsync(transactionId);

        // Return null if not found
        if (transactions == null) return null;

        // validate if transaction belongs to the user
        if (transactions.UserID != userId) return null;

        // Return dto of fetched data
        return transactions;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TransactionDomain>> GetAllTransactionsAsync(string userId)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(userId, nameof(userId));

        // Fetch data from repository
        var transactions = await _unitOfWork.TransactionRepository.GetAllAsync(filter: entity => entity.UserID == userId);

        // Return empty collection if not found
        if (!transactions.Any()) return Enumerable.Empty<TransactionDomain>();

        // Return dto of fetched data
        return transactions;
    }

    /// <inheritdoc/>
    public async Task<bool> AddTransactionAsync(TransactionDomain transactionDomain)
    {
        ArgumentNullException.ThrowIfNull(transactionDomain);

        // check if user exists
        if (!await _userService.UserExistsAsync(transactionDomain.UserID))
            return false;

        // Add data to repository
        transactionDomain.Id = Guid.NewGuid();
        await _unitOfWork.TransactionRepository.AddAsync(transactionDomain);
        var rowsUpdated = await _unitOfWork.SaveChangesAsync();
        
        _logger.LogDebug($"{rowsUpdated} rows updated");


        return true;
    }

    /// <inheritdoc/>
    public async Task UpdateTransactionAsync(TransactionRequest transactionDto)
    {
        ArgumentNullException.ThrowIfNull(transactionDto);

        // Map dto model to domain model
        var transaction = _requestDomainMapper.Map(transactionDto);

        // Update data to repository
        await _unitOfWork.TransactionRepository.UpdateAsync(transaction);
        var rowsUpdated = await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"{rowsUpdated} rows updated while updating {typeof(TransactionDomain).Name}");
    }

    /// <inheritdoc/>
    public async Task DeleteTransactionAsync(Guid transactionId)
    {
        //if (transactionId <= 0)
        //{
        //    _logger.LogWarning($"Invalid transaction ID: {transactionId}");
        //    throw new ArgumentException($"Transaction ID must be greater than zero.", nameof(transactionId));
        //}

        // Delete data from repository
        await _unitOfWork.TransactionRepository.DeleteByIdAsync(transactionId);
        await _unitOfWork.SaveChangesAsync();

    }
}
