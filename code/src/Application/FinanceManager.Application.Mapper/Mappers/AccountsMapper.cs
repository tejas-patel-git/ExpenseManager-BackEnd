using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Enums;
using FinanceManager.Domain.Models;
using FinanceManager.Domain.Util;
using FinanceManager.Models.Request;
using FinanceManager.Models.Response;

namespace FinanceManager.Application.Mapper.Mappers
{
    public class AccountsRequestToDomainMapper : BaseMapper<AccountsRequest, AccountsDomain>
    {
        public AccountsRequestToDomainMapper()
            : base(source => new()
            {
                AccountName = source.AccountName,
                AccountNumber = source.AccountNumber,
                AccountType = source.AccountType,
                BankName = source.BankName,
                InitialBalance = source.Balance,
            })
        {
        }
    }

    public class AccountsMapper : BaseMapper<AccountsDomain, UserBankAccounts>
    {
        public AccountsMapper()
            : base(source => new()
            {
                Id = source.Id,
                UserId = source.UserId,
                AccountName = source.AccountName,
                AccountNumber = source.AccountNumber,
                AccountType = source.AccountType.ToString(),
                BankName = source.BankName.ToString(),
                InitialBalance = source.InitialBalance,
            })
        {
        }
    }

    public class AccountsEntityToDomainMapper : BaseMapper<UserBankAccounts, AccountsDomain>
    {
        public AccountsEntityToDomainMapper()
            : base(source => new()
            {
                Id = source.Id,
                UserId = source.UserId,
                AccountName = source.AccountName,
                AccountNumber = source.AccountNumber,
                AccountType = source.AccountType.ToEnum<AccountType>(),
                BankName = source.BankName.ToEnum<BankName>(),
                InitialBalance = source.InitialBalance,
                CurrentBalance = source.CurrentBalance
            })
        {
        }
    }

    public class AccountsDomainToResponseMapper : BaseMapper<AccountsDomain, AccountsResponse>
    {
        public AccountsDomainToResponseMapper()
            : base(source => new()
            {
                AccountId = source.Id,
                AccountName = source.AccountName,
                AccountNumber = source.AccountNumber,
                AccountType = source.AccountType.ToString(),
                BankName = source.BankName.ToString(),
                InitialBalance = source.InitialBalance,
                CurrentBalance = source.CurrentBalance
            })
        {
        }
    }
}
