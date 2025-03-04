using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Data.Repository
{
    internal class SavingsGoalRepository : Repository<SavingsGoalDomain, SavingsGoal, Guid>, ISavingsGoalRepository
    {
        private readonly ILogger<SavingsGoalRepository> _logger;
        private readonly IMapper<SavingsGoalDomain, SavingsGoal> _domainEntityMapper;

        public SavingsGoalRepository(AppDbContext context,
                                    ILogger<SavingsGoalRepository> logger,
                                    IMapper<SavingsGoalDomain, SavingsGoal> domainEntityMapper,
                                    IMapper<SavingsGoal, SavingsGoalDomain> entityDomainMapper) : base(context, logger, domainEntityMapper, entityDomainMapper)
        {
            _logger = logger;
            _domainEntityMapper = domainEntityMapper;
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

        public override async Task UpdateAsync(SavingsGoalDomain savingsGoalDomain)
        {
            // TODO : Revisit logic - might need some tweaks related to how to handle FKs
            try
            {
                // map
                var savingsGoal = _domainEntityMapper.Map(savingsGoalDomain);

                // load the savings goal
                var existingSavingsGoal = await dbSet.FirstOrDefaultAsync(t => t.Id == savingsGoal.Id);

                if (existingSavingsGoal == null)
                {
                    _logger.LogWarning("'{savingsGoal}' with id {id} not found.", nameof(SavingsGoal), savingsGoal.Id);
                    return;
                }

                // update the savings goal properties
                existingSavingsGoal.Goal = savingsGoal.Goal;
                existingSavingsGoal.TargetAmount = savingsGoal.TargetAmount;
                existingSavingsGoal.InitialBalance = savingsGoal.InitialBalance;
                existingSavingsGoal.CurrentBalance = savingsGoal.CurrentBalance;
                existingSavingsGoal.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("'{savingsGoal} with id {id} updated.", nameof(SavingsGoal), existingSavingsGoal.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating '{savingsGoal}' with id {id}.", nameof(SavingsGoal), savingsGoalDomain.Id);
                throw;
            }
        }
    }
}
