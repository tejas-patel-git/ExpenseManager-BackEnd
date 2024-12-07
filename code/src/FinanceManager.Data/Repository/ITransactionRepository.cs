
using FinanceManager.Data.Models;

namespace FinanceManager.Data.Repository
{
    public interface ITransactionRepository
    {
        Task AddTransactionAsync(Transaction transaction);
        Task DeleteTransactionAsync(int transactionId);
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync(int userID);
        Task<Transaction?> GetTransactionByIdAsync(int transactionId);
        Task UpdateTransactionAsync(Transaction transaction);
    }
}