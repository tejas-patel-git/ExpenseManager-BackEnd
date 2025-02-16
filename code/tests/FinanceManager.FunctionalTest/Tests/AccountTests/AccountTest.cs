using FinanceManager.FunctionalTest.Abstraction;
using FinanceManager.FunctionalTest.TestData;
using FinanceManager.Models.Request;
using FinanceManager.Models.Response;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace FinanceManager.FunctionalTest.Tests.AccountTests
{
    public class AccountTest : BaseAccountTest
    {
        public AccountTest(FunctionalTestWebAppFactory functionalTestWebAppFactory) : base(functionalTestWebAppFactory)
        {
        }

        [Fact(DisplayName = "Add Account - Should Create New Account")]
        public async Task AddAccount_CreatesNewAccount()
        {
            // Arranges
            var newAccount = TestDataGenerator.Generate<AccountsRequest>();

            // Act
            var response = await HttpClient.PostAsJsonAsync("/api/accounts", newAccount);

            // Assert: Response data
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var createdAccount = await response.Content.ReadFromJsonAsync<Response<AccountsResponse>>();
            createdAccount!.Data.Should().NotBeNull();
            createdAccount.Data!.AccountName.Should().Be(newAccount.AccountName);
            createdAccount.Data.InitialBalance.Should().Be(newAccount.Balance);

            // Assert: Validate database
            var dbAccount = await Context.UserBankAccounts.FirstOrDefaultAsync(a => a.Id == createdAccount.Data.AccountId);
            dbAccount.Should().NotBeNull();
            dbAccount!.AccountName.Should().Be(newAccount.AccountName);
            dbAccount.AccountNumber.Should().Be(newAccount.AccountNumber);
            dbAccount.BankName.Should().Be(newAccount.BankName.ToString());
            dbAccount.InitialBalance.Should().Be(newAccount.Balance);
            dbAccount.AccountType.Should().Be(newAccount.AccountType.ToString());
        }
    }
}
