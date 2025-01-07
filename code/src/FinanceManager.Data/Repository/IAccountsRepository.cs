using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Repository;
using FinanceManager.Domain.Models;

namespace FinanceManager.Data.Repository
{
    public interface IAccountsRepository : IRepository<AccountsDomain, UserBankAccounts, Guid>
    {
    }
}
