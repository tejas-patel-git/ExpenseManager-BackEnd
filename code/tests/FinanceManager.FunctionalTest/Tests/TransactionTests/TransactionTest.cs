using FinanceManager.FunctionalTest.Abstraction;
using FinanceManager.Models.Response;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace FinanceManager.FunctionalTest.Tests.TransactionTests
{
    public class TransactionTest : BaseTransactionTest
    {
        private const int AccountCount = 3;

        public TransactionTest(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Theory(DisplayName = "Should update account balances correctly when posting new transaction")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ShouldUpdateAccountBalances_WhenPostingNewTransaction(bool isExpense)
        {
            // Arrange
            var (createdAccounts, transaction) = await SetupAccountsAndTransaction(isExpense, AccountCount);
            var initialBalances = createdAccounts.ToDictionary(a => a.AccountId, a => a.CurrentBalance);

            // Act
            var response = await PostTransaction(transaction);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var transactionResponse = await response.Content.ReadFromJsonAsync<Response<TransactionResponse>>();

            transactionResponse.Should().NotBeNull();
            transactionResponse!.Data.Should().NotBeNull();

            transactionResponse.Data.AssertTransactionResponseModel(transaction); // model assertion
            await transactionResponse.Data!.AssertTransactionWithDB(transaction, Context); // db data assertion

            // account balance assertion
            var updatedAccounts = await GetAccountsViaApi(createdAccounts.Select(a => a.AccountId));

            transaction.AssertAccountBalancesAfterTransaction(updatedAccounts, createdAccounts, isExpense);
        }
    }
}
