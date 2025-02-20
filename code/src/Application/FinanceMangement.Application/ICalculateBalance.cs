using FinanceManager.Domain.Models;

namespace FinanceManager.Application
{
    internal interface ICalculateBalance
    {
        Task<BalanceDomain?> GetAccountBalance(string userId);
        Task<BalanceDomain?> GetTransactionalBalance(string userId);
    }
}