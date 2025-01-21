using FinanceManager.Domain.Models;

namespace FinanceManager.Application.Services
{
    public interface IAccountsService
    {
        Task<bool> AddAccount(AccountsDomain accountsDomain);
        Task<bool> UpdateCurrentBalance(Guid id, decimal amount);
        Task<bool> DeleteTransactionAsync(Guid id, string userId);
        Task<bool> Exists(ICollection<Guid> ids);
        Task<bool> Exists(ICollection<Guid> ids, string userId);
        Task<AccountsDomain?> GetAccounts(Guid accountId, string userId);
        Task<IEnumerable<AccountsDomain>> GetAccounts(string userId);
        Task<bool> UpdateAccountAsync(AccountsDomain accountsDomain);
    }
}
