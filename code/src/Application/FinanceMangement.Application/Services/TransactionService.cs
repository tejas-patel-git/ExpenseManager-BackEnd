using FinanceManager.Data;
using FinanceManager.Data.Models;
using FinanceManager.Domain.Enums;
using FinanceManager.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.Reflection.Metadata;

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

        // Assign new GUID for the transaction
        transactionDomain.Id = Guid.NewGuid();


        // Handle Savings transactions
        if (transactionDomain.TransactionType == TransactionType.Savings)
        {
            var savingsGoal = await _unitOfWork.SavingsGoalRepository.GetByIdAsync(s => s.UserId == transactionDomain.UserId
                                                                                  && s.Goal == transactionDomain.SavingsGoal) 
                ?? throw new Exception("Savings goal does not exist");
            var savingsTransaction = new SavingsTransactionDomain
            {
                Id = Guid.NewGuid(),
                TransactionId = transactionDomain.Id,
                SavingsGoalId = savingsGoal.Id
            };

            await _unitOfWork.SavingsTransactionRepository.AddAsync(savingsTransaction);
            _logger.LogDebug("Added SavingsTransaction for Transaction ID {TransactionId} with SavingsGoalId {SavingsGoalId}",
                transactionDomain.Id, savingsTransaction.SavingsGoalId);

            // Do not process Payments for Savings transactions
            if (transactionDomain.Payments.Count != 0)
            {
                _logger.LogWarning("Payments provided for Savings transaction ID {TransactionId} were ignored.", transactionDomain.Id);
                transactionDomain.Payments = [];
            }
        }
        else
        {
            // Assign new GUIDs for payments and process them for non-Savings transactions
            foreach (var payment in transactionDomain.Payments)
            {
                payment.Id = Guid.NewGuid();
                //await _unitOfWork.PaymentRepository.AddAsync(payment); // Explicitly add to PaymentRepository
                await _accountsService.UpdateCurrentBalance(
                    payment.AccountId,
                    transactionDomain.IsExpense ? -payment.Amount : payment.Amount
                );
            }
        }

        // Add the base transaction
        var transaction = await _unitOfWork.TransactionRepository.AddAsync(transactionDomain);
        // Save all changes
        var rowsUpdated = await _unitOfWork.SaveChangesAsync();
        _logger.LogDebug("{RowsUpdated} rows updated", rowsUpdated);

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
}
