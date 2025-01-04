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
                // TODO : Handle null ids
                Amount = source.Amount,
                Date = source.Date,
                IsExpense = source.IsExpense,
                Description = source.Description
            })
        {
        }
    }

    public class TransactionDomainToResponseMapper : BaseMapper<TransactionDomain, TransactionResponse>
    {
        public TransactionDomainToResponseMapper()
            : base(source => new()
            {
                TransactionId = source.Id,
                Amount = source.Amount,
                Date = source.Date,
                IsExpense = source.IsExpense,
                Description = source.Description
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
                Id = source.Id,
                UserId = source.UserID,
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
                Id = source.Id,
                UserID = source.UserId,
                IsExpense = source.IsExpense,
                Amount = source.Amount,
                Date = source.Date,
                Description = source.Description
            })
        {
        }
    }
}
