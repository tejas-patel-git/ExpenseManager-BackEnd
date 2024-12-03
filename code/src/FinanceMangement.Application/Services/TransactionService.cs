using FinanceManager.Data.Repository;
using FinanceManager.Models;

namespace FinanceMangement.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;

        public TransactionService(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsAsync()
        {
            var transactions = await _repository.GetAllTransactionsAsync();

            if (!transactions.Any()) return Enumerable.Empty<TransactionDto>();

            var responseTransactions = new List<TransactionDto>();

            foreach (var transaction in transactions)
                responseTransactions.Add(new()
                {
                    TransactionID = transaction.TransactionID,
                    IsExpense = transaction.IsExpense,
                    Amount = transaction.Amount,
                    Date = transaction.Date,
                    Description = transaction.Description
                });

            return responseTransactions;
        }
    }
}
