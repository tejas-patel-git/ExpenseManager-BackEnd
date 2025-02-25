using FinanceManager.Data.Models;
using FinanceManager.FunctionalTest.Abstraction;
using FinanceManager.FunctionalTest.TestData;
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

        [Theory(DisplayName = "Should create savings transaction correctly without affecting accounts")]
        [InlineData(true)]    // Savings transaction (expense)
        [InlineData(false)]   // Savings transaction (income)
        public async Task ShouldCreateSavingsTransaction_WhenPostingNewSavingsTransaction(bool isExpense)
        {
            // Arrange
            var (createdAccounts, transaction) = await SetupAccountsAndTransaction(isExpense, AccountCount, true);
            var initialBalances = createdAccounts.ToDictionary(a => a.AccountId, a => a.CurrentBalance);
            var savingsGoals = TestDataGenerator.Generate<SavingsGoal>(cfg =>
            {
                cfg.RuleFor(s => s.UserId, UserId)
                   .RuleFor(s => s.Goal, transaction.SavingGoal);
            });
            Context.SavingsGoals.Add(savingsGoals);
            await Context.SaveChangesAsync();

            // Act
            var response = await PostTransaction(transaction);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var transactionResponse = await response.Content.ReadFromJsonAsync<Response<TransactionResponse>>();

            transactionResponse.Should().NotBeNull();
            transactionResponse!.Data.Should().NotBeNull();

            transactionResponse.Data.AssertTransactionResponseModel(transaction);
            await transactionResponse.Data!.AssertSavingsTransactionWithDB(transaction, Context);

            var updatedAccounts = await GetAccountsViaApi(createdAccounts.Select(a => a.AccountId));
            updatedAccounts.AssertAccountBalancesUnchanged(createdAccounts);

            //transactionResponse.Data!.Type.Should().Be(TransactionType.Savings);
        }
    }
}
