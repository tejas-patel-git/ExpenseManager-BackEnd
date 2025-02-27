using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Data.Repository
{
    internal class SavingsGoalRepository : Repository<SavingsGoalDomain, SavingsGoal, Guid>, ISavingsGoalRepository
    {
        private readonly ILogger<SavingsGoalRepository> _logger;

        public SavingsGoalRepository(AppDbContext context,
                                    ILogger<SavingsGoalRepository> logger,
                                    IMapper<SavingsGoalDomain, SavingsGoal> domainEntityMapper,
                                    IMapper<SavingsGoal, SavingsGoalDomain> entityDomainMapper) : base(context, logger, domainEntityMapper, entityDomainMapper)
        {
            _logger = logger;
        }

        public async Task<bool> UpdateBalance(Guid id, decimal amount)
        {
            try
            {
                // retrieve the existing account from the database
                var existingSavingsGoal = await dbSet.FindAsync(id);

                if (existingSavingsGoal == null)
                {
                    _logger.LogWarning("'{type}' with id {id} not found.", nameof(SavingsGoal), id);
                    return false;
                }

                // update current balance
                existingSavingsGoal.CurrentBalance += amount;

                _logger.LogInformation("Balance updated for '{type}' id {id}.", nameof(SavingsGoal), id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating balance of '{type}' id {id}.", nameof(SavingsGoal), id);
                throw;
            }
        }
    }
}
