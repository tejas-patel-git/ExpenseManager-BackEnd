using FinanceManager.Data.Repository;
using FinanceManager.Models;
using FinanceMangement.Application.Mappers;

namespace FinanceMangement.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;

        public TransactionService(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
        {
            var transactions = await _repository.GetAllTransactionsAsync();

            if (!transactions.Any()) return Enumerable.Empty<TransactionDto>();

            return transactions.MapToDto();
        }

        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync(int userID)
        {
            var transactions = await _repository.GetAllTransactionsAsync(userID);

            if (!transactions.Any()) return Enumerable.Empty<TransactionDto>();

            return transactions.MapToDto();
        }
    }
}
