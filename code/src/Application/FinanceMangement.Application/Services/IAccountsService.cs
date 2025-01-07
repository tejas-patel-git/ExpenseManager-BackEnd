using FinanceManager.Domain.Models;

namespace FinanceManager.Application.Services
{
    public interface IAccountsService
    {
        Task<bool> AddAccount(AccountsDomain accountsDomain);
    }
}
