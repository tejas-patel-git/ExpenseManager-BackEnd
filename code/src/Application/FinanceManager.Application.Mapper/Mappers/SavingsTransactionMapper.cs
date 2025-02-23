using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;

namespace FinanceManager.Application.Mapper.Mappers
{
    public class SavingsTransactionDomainToEntityMapper : BaseMapper<SavingsTransactionDomain, SavingsTransaction>
    {
        public SavingsTransactionDomainToEntityMapper()
            : base(source => new SavingsTransaction
            {
                Id = source.Id,
                SavingsGoalId = source.SavingsGoalId,
                TransactionId = source.TransactionId
            })
        {
        }
    }

    public class SavingsTransactionToDomainMapper : BaseMapper<SavingsTransaction, SavingsTransactionDomain>
    {
        public SavingsTransactionToDomainMapper()
            : base(source => new SavingsTransactionDomain
            {
                Id = source.Id,
                SavingsGoalId = source.SavingsGoalId,
                TransactionId = source.TransactionId
            })
        {
        }
    }
}
