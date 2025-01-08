using FinanceManager.Domain.Models;

namespace FinanceManager.Application.Services
{
    public interface IAccountsService
    {
        Task<bool> AddAccount(AccountsDomain accountsDomain);
        Task<AccountsDomain?> GetAccounts(Guid accountId, string userId);
        Task<IEnumerable<AccountsDomain>> GetAccounts(string userId);
    }
}
