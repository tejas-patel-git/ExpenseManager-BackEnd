using FinanceManager.Data;
using FinanceManager.Domain.Models;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Application.Services;

internal class TransactionService : BaseService, ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountsService _accountsService;
    private readonly ILogger<TransactionService> _logger;

    /// <inheritdoc/>
    public TransactionService(IUnitOfWork unitOfWork,
                              IAccountsService accountsService,
                              ILogger<TransactionService> logger)
        : base(unitOfWork.UserRepository)
    {
        _unitOfWork = unitOfWork;
        _accountsService = accountsService;
        _logger = logger;
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
    public async Task<TransactionDomain> AddTransactionAsync(TransactionDomain transactionDomain)
    {
        ArgumentNullException.ThrowIfNull(transactionDomain);

        // create new guid for each payments
        foreach (var accounts in transactionDomain.Payments)
            accounts.Id = Guid.NewGuid();

        // create new guid for transaction
        transactionDomain.Id = Guid.NewGuid();

        // add transaction & payment to repository
        var transaction = await _unitOfWork.TransactionRepository.AddAsync(transactionDomain);

        // update current balance in respective accounts
        foreach (var account in transaction.Payments)
            await _accountsService.UpdateCurrentBalance(account.AccountId, transaction.IsExpense ? -account.Amount : account.Amount);

        // save repository changes
        var rowsUpdated = await _unitOfWork.SaveChangesAsync();

        _logger.LogDebug("{rowsUpdated} rows updated", rowsUpdated);

        return transaction;
    }

    /// <inheritdoc/>
    public async Task UpdateTransactionAsync(TransactionDomain transactionDomain)
    {
        ArgumentNullException.ThrowIfNull(transactionDomain);

        var oldPayments = await _unitOfWork.PaymentRepository.GetAllAsync(x => x.TransactionId == transactionDomain.Id);
        var oldTransaction = await _unitOfWork.TransactionRepository.GetByIdAsync(transactionDomain.Id);

        if (oldTransaction == null) return;

        // update transaction
        await _unitOfWork.TransactionRepository.UpdateAsync(transactionDomain);

        // update current balance of old payment accounts
        foreach (var oldPayment in oldPayments)
        {
            // revert old payment account balance - ADD amount if it was an expense
            await _unitOfWork.AccountsRepository.UpdateBalance(oldPayment.AccountId, oldTransaction.IsExpense ? oldPayment.Amount : -oldPayment.Amount);
        }

        // update current balance of new payment accounts
        foreach (var newPayment in transactionDomain.Payments)
        {
            // add amount if not an expense
            await _unitOfWork.AccountsRepository.UpdateBalance(newPayment.AccountId, transactionDomain.IsExpense ? -newPayment.Amount : newPayment.Amount);
        }

        // save changes
        var rowsUpdated = await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("{rowsUpdated} rows updated while {method}", rowsUpdated, nameof(UpdateTransactionAsync));
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
        var transactions = await _unitOfWork.TransactionRepository.GetAllAsync(entity => entity.UserId == userId);

        if (transactions == null || !transactions.Any()) return null;

        // get the balance after accounting transactions
        var balance = transactions.Sum(d => d.IsExpense ? -d.Amount : d.Amount);

        // add the initial balance of the all accounts of user
        var accounts = await _unitOfWork.AccountsRepository.GetAllAsync(acc => acc.UserId == userId);
        if (accounts.Any()) balance += accounts.Sum(acc => acc.InitialBalance);

        return new() { CurrentBalance = balance };
    }
}
