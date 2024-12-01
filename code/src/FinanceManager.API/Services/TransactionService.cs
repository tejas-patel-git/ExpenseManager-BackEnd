using FinanceManager.Data.Repository;
using FinanceManager.Models;

namespace FinanceManager.API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;

        public TransactionService(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync()
        {
            return await _repository.GetAllTransactionsAsync();
        }
    }
}
