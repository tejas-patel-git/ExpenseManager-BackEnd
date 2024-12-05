
using FinanceManager.Data.Models;

namespace FinanceManager.Data.Repository
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync(int userID);
    }
}