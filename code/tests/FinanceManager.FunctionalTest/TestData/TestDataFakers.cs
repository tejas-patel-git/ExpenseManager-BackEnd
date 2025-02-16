using Bogus;
using FinanceManager.Data.Models;
using FinanceManager.Domain.Enums;
using FinanceManager.Models;
using FinanceManager.Models.Request;
using System.Text.Json;

internal static class TestDataFakers
{
    internal static Faker<User> UserFaker()
    {
        return new Faker<User>()
                        .RuleFor(u => u.Id, f => Guid.NewGuid().ToString())
                        .RuleFor(u => u.Email, f => f.Internet.Email())
                        .RuleFor(u => u.IsEmailVerified, f => f.Random.Bool())
                        .RuleFor(u => u.FamilyName, f => f.Name.LastName())
                        .RuleFor(u => u.GivenName, f => f.Name.FirstName())
                        .RuleFor(u => u.FullName, (f, u) => $"{u.GivenName} {u.FamilyName}")
                        .RuleFor(u => u.Nickname, f => f.Internet.UserName())
                        .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("##########"))
                        .RuleFor(u => u.AppMetadata, f => JsonSerializer.Serialize("{}"))
                        .RuleFor(u => u.UserMetadata, f => JsonSerializer.Serialize("{}"))
                        .RuleFor(u => u.PictureUrl, f => f.Internet.Avatar())
                        .RuleFor(u => u.LastPasswordReset, f => f.Date.Past(1))
                        .RuleFor(u => u.CreatedAt, f => f.Date.Past(5))
                        .RuleFor(u => u.UpdatedAt, (f, u) => f.Date.Between(u.CreatedAt, DateTime.UtcNow));
    }

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
                        .RuleFor(u => u.CreatedAt, f => f.Date.Past(5))
                        .RuleFor(u => u.UpdatedAt, (f, u) => f.Date.Between(u.CreatedAt, DateTime.UtcNow));
    }

    internal static Faker<AccountsRequest> AccountsRequestFaker()
    {
        return new Faker<AccountsRequest>()
                        .RuleFor(a => a.AccountNumber, f => f.Finance.Account())
                        .RuleFor(a => a.AccountName, f => f.Finance.AccountName())
                        .RuleFor(a => a.BankName, f => f.PickRandom<BankName>())
                        .RuleFor(a => a.AccountType, f => f.PickRandom<AccountType>())
                        .RuleFor(a => a.Balance, f => f.Finance.Amount(100, 10000));
    }
}