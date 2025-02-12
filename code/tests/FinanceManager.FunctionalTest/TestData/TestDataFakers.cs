using Bogus;
using FinanceManager.Data.Models;
using FinanceManager.Domain.Enums;
using FinanceManager.Models;
using FinanceManager.Models.Request;

internal static class TestDataFakers
{
    internal static Faker<Transaction> TransactionFaker()
    {
        return new Faker<Transaction>()
                        .RuleFor(t => t.Id, f => Guid.NewGuid())
                        .RuleFor(t => t.IsExpense, f => f.Random.Bool())
                        .RuleFor(t => t.Amount, f => f.Finance.Amount(10, 500))
                        .RuleFor(t => t.Date, f => f.Date.Recent())
                        .RuleFor(t => t.Description, f => f.Commerce.ProductName());
    }

    internal static Faker<TransactionRequest> TransactionRequestFaker()
    {
        return new Faker<TransactionRequest>()
                        .RuleFor(t => t.IsExpense, f => f.Random.Bool())
                        .RuleFor(t => t.Amount, f => f.Finance.Amount(10, 500))
                        .RuleFor(t => t.Date, f => f.Date.Recent())
                        .RuleFor(t => t.Description, f => f.Commerce.ProductName())
                        .RuleFor(t => t.Payments, new Payment() { Accounts = [] });
    }

    internal static Faker<UserBankAccounts> UserBankAccountsFaker()
    {
        return new Faker<UserBankAccounts>()
                        .RuleFor(a => a.Id, f => Guid.NewGuid())
                        .RuleFor(a => a.UserId, f => f.Random.Guid().ToString())
                        .RuleFor(a => a.AccountNumber, f => f.Finance.Account())
                        .RuleFor(a => a.AccountName, f => f.Finance.AccountName())
                        .RuleFor(a => a.BankName, f => f.PickRandom<BankName>().ToString())
                        .RuleFor(a => a.AccountType, f => f.PickRandom<AccountType>().ToString())
                        .RuleFor(a => a.InitialBalance, f => f.Finance.Amount(100, 10000))
                        .RuleFor(a => a.CurrentBalance, f => f.Finance.Amount(100, 10000))
                        .RuleFor(a => a.CreatedAt, f => f.Date.Past(2))
                        .RuleFor(a => a.UpdatedAt, f => f.Date.Recent());
    }
}