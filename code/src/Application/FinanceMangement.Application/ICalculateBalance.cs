using FinanceManager.Domain.Models;

namespace FinanceManager.Application
{
    internal interface ICalculateBalance
    {
        Task<BalanceDomain?> GetBalance(string userId);
        Task<BalanceDomain?> GetTransactionalBalance(string userId);
    }
}