using FinanceManager.Models;

namespace FinanceManager.API.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetTransactionsAsync();
    }
}