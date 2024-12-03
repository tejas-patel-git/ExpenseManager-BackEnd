using FinanceManager.Models;

namespace FinanceMangement.Application.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetTransactionsAsync();
    }
}