using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Data.Repository
{
    internal class SavingsGoalRepository : Repository<SavingsGoalDomain, SavingsGoal, Guid>, ISavingsGoalRepository
    {
        public SavingsGoalRepository(AppDbContext context,
                                    ILogger<SavingsGoalRepository> logger,
                                    IMapper<SavingsGoalDomain, SavingsGoal> domainEntityMapper,
                                    IMapper<SavingsGoal, SavingsGoalDomain> entityDomainMapper) : base(context, logger, domainEntityMapper, entityDomainMapper)
        {

        }
    }
}
