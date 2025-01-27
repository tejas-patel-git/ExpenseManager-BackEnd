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
                                 IMapper<Transaction, TransactionDomain> entityDomainMapper)
        : base(context, logger, domainEntityMapper, entityDomainMapper)
    {
        _logger = logger;
        _domainEntityMapper = domainEntityMapper;
    }

    public override async Task UpdateAsync(TransactionDomain transactionDomain)
    {
        // TODO : Revisit logic - might need some tweaks related to how to handle FKs
        try
        {
            // Retrieve the existing transaction from the database
            var existingTransaction = await dbSet.FindAsync(transactionDomain.Id);

            if (existingTransaction == null)
            {
                _logger.LogWarning($"'{typeof(Transaction).Name}' with id {transactionDomain.Id} not found.");
                return;
            }

            // map
            var transaction = _domainEntityMapper.Map(transactionDomain);

            // Update the properties of the existing entity
            existingTransaction.IsExpense = transaction.IsExpense;
            existingTransaction.Date = transaction.Date;
            existingTransaction.Amount = transaction.Amount;
            existingTransaction.Description = transaction.Description;
            existingTransaction.Payments = transaction.Payments;

            _logger.LogInformation($"'{typeof(Transaction).Name}' with id {existingTransaction.Id} updated.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating '{typeof(Transaction).Name}' with id {transactionDomain.Id}.");
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