using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Data.Repository
{
    internal class SavingsTransactionRepository : Repository<SavingsTransactionDomain, SavingsTransaction, Guid>, ISavingsTransactionRepository
    {
        public SavingsTransactionRepository(AppDbContext context,
                                            ILogger<SavingsTransactionRepository> logger,
                                            IMapper<SavingsTransactionDomain, SavingsTransaction> domainEntityMapper,
                                            IMapper<SavingsTransaction, SavingsTransactionDomain> entityDomainMapper) 
            : base(context, logger, domainEntityMapper, entityDomainMapper)
        {

        }
    }
}
