using FinanceManager.Data.Models;
using FinanceManager.Models;

namespace FinanceMangement.Application.Mappers
{
    public static class TransactionMapperExtension
    {
        public static TransactionDto MapToDto(this Transaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction, nameof(transaction));

            return new()
            {
                TransactionID = transaction.TransactionID,
                IsExpense = transaction.IsExpense,
                Amount = transaction.Amount,
                Date = transaction.Date,
                Description = transaction.Description
            };
        }

        public static IEnumerable<TransactionDto> MapToDto(this IEnumerable<Transaction> transactions)
        {
            ArgumentNullException.ThrowIfNull(transactions, nameof(transactions));

            var transactionsDto = new List<TransactionDto>();

            foreach (var transaction in transactions)
                transactionsDto.Add(transaction.MapToDto());

            return transactionsDto;
        }
    }
}
