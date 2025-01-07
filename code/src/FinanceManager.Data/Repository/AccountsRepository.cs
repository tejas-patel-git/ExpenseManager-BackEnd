using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Data.Repository
{
    internal class AccountsRepository : Repository<AccountsDomain, UserBankAccounts, Guid>, IAccountsRepository
    {
        public AccountsRepository(AppDbContext context,
                                 ILogger<AccountsRepository> logger,
                                 IMapper<AccountsDomain, UserBankAccounts> domainEntityMapper,
                                 IMapper<UserBankAccounts, AccountsDomain> entityDomainMapper)
        : base(context, logger, domainEntityMapper, entityDomainMapper)
        {
        }
    }
}
