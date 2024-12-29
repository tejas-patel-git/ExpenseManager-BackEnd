using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Data.Repository;

internal class TransactionRepository : Repository<TransactionDomain, Transaction, int>, ITransactionRepository
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
    public TransactionRepository(AppDbContext context,
                                   ILogger<TransactionRepository> logger,
                                   IMapper<TransactionDomain, Transaction> domainEntityMapper,
                                   IMapper<Transaction, TransactionDomain> entityDomainMapper)
        : base(context, logger, domainEntityMapper, entityDomainMapper)
    {
        _logger = logger;
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

            // Update the properties of the existing entity
            existingTransaction.IsExpense = transactionDomain.IsExpense;
            existingTransaction.Date = transactionDomain.Date;
            existingTransaction.Amount = transactionDomain.Amount;
            existingTransaction.Description = transactionDomain.Description;

            _logger.LogInformation($"'{typeof(Transaction).Name}' with id {existingTransaction.Id} updated.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating '{typeof(Transaction).Name}' with id {transactionDomain.Id}.");
            throw;
        }
    }
}