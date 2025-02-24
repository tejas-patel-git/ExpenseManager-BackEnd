using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Enums;
using FinanceManager.Domain.Models;
using FinanceManager.Models.Request;
using FinanceManager.Models.Response;

namespace FinanceManager.Application.Mapper.Mappers
{
    public class TransactionRequestToDomainMapper : BaseMapper<TransactionRequest, TransactionDomain>
    {
        public TransactionRequestToDomainMapper()
            : base(source =>
            {
                List<PaymentDomain> payments = [];

                if (source.Payments != null)
                {
                    foreach (var account in source.Payments.Accounts)
                        payments.Add(new() { AccountId = account.AccountId, Amount = account.Amount });
                }

                return new()
                {
                    Amount = source.Amount,
                    Date = source.Date,
                    IsExpense = source.IsExpense,
                    Description = source.Description,
                    TransactionType = source.Type,
                    SavingsGoal = source.SavingGoal,
                    Payments = payments
                };
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
                TransactionType = source.TransactionType.ToString(),
                Description = source.Description
            })
        {
        }
    }

    public class TransactionDomainToEntityMapper : BaseMapper<TransactionDomain, Transaction>
    {
        public TransactionDomainToEntityMapper()
            : base(source =>
            {
                List<TransactionPayment> payments = [];

                foreach (var payment in source.Payments)
                    payments.Add(new()
                    {
                        TransactionId = source.Id,
                        UserBankAccountId = payment.AccountId,
                        Amount = payment.Amount,
                    });

                return new()
                {
                    // TODO : Handle Null transaction id.
                    Id = source.Id,
                    UserId = source.UserId,
                    IsExpense = source.IsExpense,
                    TransactionType = (byte)source.TransactionType,
                    Amount = source.Amount,
                    Date = source.Date,
                    Description = source.Description,
                    Payments = payments,
                    UpdatedAt = DateTime.UtcNow
                };
            })
        {
        }
    }

    public class TransactionEntityToDomainMapper : BaseMapper<Transaction, TransactionDomain>
    {
        public TransactionEntityToDomainMapper()
            : base(source =>
            {
                List<PaymentDomain> payments = [];

                foreach (var payment in source.Payments)
                    payments.Add(new()
                    {
                        Id = payment.Id,
                        AccountId = payment.UserBankAccountId,
                        Amount = payment.Amount,
                    });

                return new()
                {
                    Id = source.Id,
                    UserId = source.UserId,
                    IsExpense = source.IsExpense,
                    TransactionType = (TransactionType)source.TransactionType,
                    Amount = source.Amount,
                    Date = source.Date,
                    Description = source.Description,
                    Payments = payments,
                };
            })
        {
        }
    }
}
