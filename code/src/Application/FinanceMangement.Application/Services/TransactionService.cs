﻿using FinanceManager.Data;
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
    public async Task<TransactionDomain?> GetUserTransactionAsync(Guid transactionId, string userId)
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
        if (transactions.UserId != userId) return null;

        // Return dto of fetched data
        return transactions;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TransactionDomain>> GetUserTransactionsAsync(string userId)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(userId, nameof(userId));

        // Fetch data from repository
        var transactions = await _unitOfWork.TransactionRepository.GetAllAsync(entity => entity.UserId == userId);

        // Return empty collection if not found
        if (!transactions.Any()) return [];

        // Return dto of fetched data
        return transactions;
    }

    /// <inheritdoc/>
    public async Task<bool> AddTransactionAsync(TransactionDomain transactionDomain)
    {
        ArgumentNullException.ThrowIfNull(transactionDomain);

        // check if user exists
        if (!await _userService.UserExistsAsync(transactionDomain.UserId))
            return false;

        // Add data to repository
        transactionDomain.Id = Guid.NewGuid();
        await _unitOfWork.TransactionRepository.AddAsync(transactionDomain);
        var rowsUpdated = await _unitOfWork.SaveChangesAsync();
        
        _logger.LogDebug($"{rowsUpdated} rows updated");


        return true;
    }

    /// <inheritdoc/>
    public async Task UpdateTransactionAsync(TransactionDomain transactionDto)
    {
        ArgumentNullException.ThrowIfNull(transactionDto);

        // Update data to repository
        await _unitOfWork.TransactionRepository.UpdateAsync(transactionDto);
        var rowsUpdated = await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"{rowsUpdated} rows updated while updating {typeof(TransactionDomain).Name}");
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteTransactionAsync(Guid transactionId, string userId)
    {
        // Delete data from repository
        var isSuccess = await _unitOfWork.TransactionRepository.DeleteByIdAsync(transactionId, userId);
        await _unitOfWork.SaveChangesAsync();

        return isSuccess;
    }

    /// <inheritdoc/>
    public async Task<BalanceDomain?> GetBalanceAsync(string userId)
    {
        var transactions = await _unitOfWork.TransactionRepository.GetAllAsync(entity =>  entity.UserId == userId);

        if(transactions == null || !transactions.Any()) return null;

        var balance = transactions.Sum(d => d.IsExpense ? -d.Amount : d.Amount);

        return new() { CurrentBalance = balance };
    }
}