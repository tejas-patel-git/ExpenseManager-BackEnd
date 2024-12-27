using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using FinanceManager.Models.Request;
using FinanceManager.Models.Response;

namespace FinanceManager.Application.Mapper.Mappers
{
    public class TransactionRequestToDomainMapper : BaseMapper<TransactionRequest, TransactionDomain>
    {
        public TransactionRequestToDomainMapper()
            : base(source => new()
            {
                TransactionID = source.TransactionId,
                Amount = source.Amount,
                Date = source.Date,
                IsExpense = source.IsExpense
            })
        {
        }
    }

    public class TransactionDomainToResponseMapper : BaseMapper<TransactionDomain, TransactionResponse>
    {
        public TransactionDomainToResponseMapper()
            : base(source => new()
            {
                TransactionId = source.TransactionID ?? 0,
                Amount = source.Amount,
                Date = source.Date,
                IsExpense = source.IsExpense
            })
        {
        }
    }

    public class TransactionDomainToEntityMapper : BaseMapper<TransactionDomain, Transaction>
    {
        public TransactionDomainToEntityMapper()
            : base(source => new()
            {
                // TODO : Handle Null transaction id.
                TransactionID = source.TransactionID ?? 0,
                IsExpense = source.IsExpense,
                Amount = source.Amount,
                Date = source.Date,
                Description = source.Description,
                UpdatedAt = DateTime.UtcNow
            })
        {
        }
    }

    public class TransactionEntityToDomainMapper : BaseMapper<Transaction, TransactionDomain>
    {
        public TransactionEntityToDomainMapper()
            : base(source => new()
            {
                TransactionID = source.TransactionID,
                IsExpense = source.IsExpense,
                Amount = source.Amount,
                Date = source.Date,
                Description = source.Description
            })
        {
        }
    }
}
