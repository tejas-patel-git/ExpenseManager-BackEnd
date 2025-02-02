using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;

namespace FinanceManager.Application.Mapper.Mappers
{
    public class PaymentDomainToEntityMapper : BaseMapper<PaymentDomain, TransactionPayment>
    {
        public PaymentDomainToEntityMapper()
            : base(source => new TransactionPayment
            {
                Id = source.Id,
                UserBankAccountId = source.AccountId,
                Amount = source.Amount,
                TransactionId = source.TransactionId
            })
        {
        }
    }

    public class PaymentEntityToDomainMapper : BaseMapper<TransactionPayment, PaymentDomain>
    {
        public PaymentEntityToDomainMapper()
            : base(source => new PaymentDomain
            {
                Id = source.Id,
                AccountId = source.UserBankAccountId,
                Amount = source.Amount,
                TransactionId = source.TransactionId
            })
        {
        }
    }
}