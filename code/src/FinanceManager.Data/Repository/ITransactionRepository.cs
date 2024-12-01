using FinanceManager.Models;

namespace FinanceManager.Data.Repository
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
    }
}