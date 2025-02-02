using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Data.Repository;

internal class TransactionRepository : Repository<TransactionDomain, Transaction, Guid>, ITransactionRepository
{
    private readonly ILogger<TransactionRepository> _logger;
    private readonly IMapper<TransactionDomain, Transaction> _domainEntityMapper;
    private readonly IMapper<Transaction, TransactionDomain> entityDomainMapper;
    private readonly IPaymentRepository _paymentRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionRepository"/> class.
    /// </summary>
    /// <param name="context">The <see cref="AppDbContext"/> instance used to interact with the database.</param>
    /// <param name="logger">The <see cref="ILogger{TransactionRepository}"/> instance for logging repository activities.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when either <paramref name="context"/> or <paramref name="logger"/> is <c>null</c>.
    /// </exception>
    public TransactionRepository(AppDbContext context,
                                 ILogger<TransactionRepository> logger,
                                 IMapper<TransactionDomain, Transaction> domainEntityMapper,
                                 IMapper<Transaction, TransactionDomain> entityDomainMapper,
                                 IPaymentRepository paymentRepository)
        : base(context, logger, domainEntityMapper, entityDomainMapper)
    {
        _logger = logger;
        _domainEntityMapper = domainEntityMapper;
        this.entityDomainMapper = entityDomainMapper;
        _paymentRepository = paymentRepository;
    }

    public override async Task UpdateAsync(TransactionDomain transactionDomain)
    {
        // TODO : Revisit logic - might need some tweaks related to how to handle FKs
        try
        {
            // map
            var transaction = _domainEntityMapper.Map(transactionDomain);

            // load the transaction with its related payments
            var existingTransaction = await dbSet.Include(t => t.Payments)
                                                 .FirstOrDefaultAsync(t => t.Id == transaction.Id);

            if (existingTransaction == null)
            {
                _logger.LogWarning("'{transaction}' with id {id} not found.", nameof(Transaction), transaction.Id);
                return;
            }

            // update the transaction properties
            existingTransaction.IsExpense = transaction.IsExpense;
            existingTransaction.Date = transaction.Date;
            existingTransaction.Amount = transaction.Amount;
            existingTransaction.Description = transaction.Description;
            existingTransaction.UpdatedAt = DateTime.UtcNow;

            // process payments
            var paymentsBeforeUpdate = transaction.Payments.Select(a => a.UserBankAccountId).ToList();

           
            // remove orphaned payments
            var paymentsToRemove = existingTransaction.Payments
                .Where(p => !paymentsBeforeUpdate.Contains(p.UserBankAccountId))
                .ToList();

            if (paymentsToRemove != null && paymentsToRemove.Count > 0) await _paymentRepository.RemovePayment(paymentsToRemove);

            _logger.LogInformation("'{transaction} with id {id} updated.", nameof(Transaction), existingTransaction.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating '{transaction}' with id {id}.", nameof(Transaction), transactionDomain.Id);
            throw;
        }
    }

    public virtual async Task<bool> DeleteByIdAsync(Guid id, string userId)
    {
        try
        {
            var entity = await dbSet.FirstOrDefaultAsync(entity => entity.UserId == userId && entity.Id == id);

            if (entity == null)
            {
                _logger.LogInformation("'{type}' with id {id} not found for deletion.", nameof(Transaction), id);
                return false;
            }

            Delete(entity);
            _logger.LogInformation("'{type}' with id {id} deleted successfully.", nameof(Transaction), id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting '{type}' id {id}.", nameof(Transaction), id);
            throw;
        }
    }
}