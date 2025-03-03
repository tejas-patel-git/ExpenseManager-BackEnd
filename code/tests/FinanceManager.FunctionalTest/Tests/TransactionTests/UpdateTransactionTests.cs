using FinanceManager.FunctionalTest.Abstraction;
using FinanceManager.FunctionalTest.TestData;
using FinanceManager.Models;
using FinanceManager.Models.Request;
using FinanceManager.Models.Response;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace FinanceManager.FunctionalTest.Tests.TransactionTests
{
    public class UpdateTransactionTest : BaseTransactionTest
    {
        private const int AccountCount = 3;

        public UpdateTransactionTest(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Theory(DisplayName = "Should correctly update transaction details when PUTting valid transaction")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ShouldUpdateTransactionDetails_WhenPuttingValidTransaction(bool isExpense)
        {
            // Arrange
            var (createdAccounts, transaction) = await SetupAccountsAndTransaction(isExpense, AccountCount);

            // First create a transaction
            var createResponse = await PostTransaction(transaction);
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var createdTransaction = await createResponse.Content.ReadFromJsonAsync<Response<TransactionResponse>>();
            createdTransaction.Should().NotBeNull();
            createdTransaction!.Data.Should().NotBeNull();

            // Get the created transaction ID
            var transactionId = createdTransaction.Data!.TransactionId;

            // Create an updated transaction
            var updatedTransaction = transaction.Clone();
            updatedTransaction.Description = "Updated Transaction";
            
            // Act
            var updateResponse = await UpdateTransactionAsync(transactionId, updatedTransaction);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify the transaction was updated in DB
            var getResponse = await GetTransactionByIdAsync(transactionId);
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedTransaction = await getResponse.Content.ReadFromJsonAsync<Response<TransactionResponse>>();
            retrievedTransaction.Should().NotBeNull();
            retrievedTransaction!.Data.Should().NotBeNull();

            // Assert updated fields
            retrievedTransaction.Data!.Description.Should().Be(updatedTransaction.Description);
            retrievedTransaction.Data!.Amount.Should().Be(updatedTransaction.Amount);
        }

        [Theory(DisplayName = "Should correctly update account balances when changing transaction amount")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ShouldUpdateAccountBalances_WhenChangingTransactionAmount(bool isExpense)
        {
            // Arrange
            var (createdAccounts, transaction) = await SetupAccountsAndTransaction(isExpense, AccountCount);

            // Track initial balances
            var initialBalances = createdAccounts.ToDictionary(a => a.AccountId, a => a.CurrentBalance);

            // First create a transaction
            var createResponse = await PostTransaction(transaction);
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var createdTransaction = await createResponse.Content.ReadFromJsonAsync<Response<TransactionResponse>>();

            // Get account balances after initial transaction
            var accountsAfterCreate = await GetAccountsViaApi(createdAccounts.Select(a => a.AccountId));

            // Create an updated transaction with different amount
            var updatedTransaction = transaction.Clone();
            updatedTransaction.Amount = transaction.Amount * 2; // Double the amount

            // Update payment amounts proportionally
            foreach (var payment in updatedTransaction.Payments!.Accounts!)
            {
                payment.Amount *= 2;
            }

            // Act
            var updateResponse = await UpdateTransactionAsync(createdTransaction.Data!.TransactionId, updatedTransaction);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Get updated account balances
            var accountsAfterUpdate = await GetAccountsViaApi(createdAccounts.Select(a => a.AccountId));

            // Verify account balances were correctly adjusted
            foreach (var account in accountsAfterUpdate)
            {
                var originalAccount = createdAccounts.First(a => a.AccountId == account.AccountId);
                var initialBalance = initialBalances[account.AccountId];
                var expectedBalance = CalculateExpectedBalance(
                    initialBalance,
                    updatedTransaction,
                    originalAccount,
                    isExpense);

                account.CurrentBalance.Should().Be(expectedBalance);
            }
        }

        [Fact(DisplayName = "Should return 404 when updating non-existent transaction")]
        public async Task ShouldReturn404_WhenUpdatingNonExistentTransaction()
        {
            // Arrange
            var (createdAccounts, transaction) = await SetupAccountsAndTransaction(true, AccountCount);
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await UpdateTransactionAsync(nonExistentId, transaction);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "Should return 400 when transaction has invalid payment accounts")]
        public async Task ShouldReturn400_WhenTransactionHasInvalidPaymentAccounts()
        {
            // Arrange
            var (createdAccounts, transaction) = await SetupAccountsAndTransaction(true, AccountCount);

            // Create transaction first
            var createResponse = await PostTransaction(transaction);
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var createdTransaction = await createResponse.Content.ReadFromJsonAsync<Response<TransactionResponse>>();

            // Update with invalid account IDs
            var updatedTransaction = transaction.Clone();
            updatedTransaction.Payments!.Accounts!.ToList()[0].AccountId = Guid.NewGuid(); // Non-existent account

            // Act
            var updateResponse = await UpdateTransactionAsync(createdTransaction.Data!.TransactionId, updatedTransaction);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var errorResponse = await updateResponse.Content.ReadFromJsonAsync<Response>();
            errorResponse.Should().NotBeNull();
            errorResponse!.ErrorMessage.Should().NotBeNull();
            errorResponse!.ErrorMessage.Should().Contain("Not all payment account exists.");
        }

        [Fact(DisplayName = "Should correctly handle payment account changes when updating transaction")]
        public async Task ShouldHandlePaymentAccountChanges_WhenUpdatingTransaction()
        {
            // Arrange
            var (createdAccounts, transaction) = await SetupAccountsAndTransaction(true, AccountCount);

            // Create transaction first
            var createResponse = await PostTransaction(transaction);
            var createdTransaction = await createResponse.Content.ReadFromJsonAsync<Response<TransactionResponse>>();
            createdTransaction.Should().NotBeNull("because it is created in Arrange phase");

            // Create additional test account
            var newAccount = await CreateTestAccount();
            var initialNewAccountBalance = newAccount.CurrentBalance;

            // Get account balances after initial transaction
            var accountsAfterCreate = await GetAccountsViaApi(createdAccounts.Select(a => a.AccountId));

            // Update transaction to use different accounts
            var updatedTransaction = transaction.Clone();
            // Remove one account and add the new one
            var removedAccount = updatedTransaction.Payments!.Accounts.First();
            updatedTransaction.Payments!.Accounts!.Remove(removedAccount);
            updatedTransaction.Payments!.Accounts!.Add(new()
            {
                AccountId = newAccount.AccountId,
                Amount = removedAccount.Amount
            });

            // Act
            var updateResponse = await UpdateTransactionAsync(createdTransaction!.Data!.TransactionId, updatedTransaction);

            // Assert
            var result = await updateResponse.Content.ReadAsStringAsync();
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Get updated account balances of all accounts
            var allAccountIds = createdAccounts.Select(a => a.AccountId).Append(newAccount.AccountId);
            var accountsAfterUpdate = await GetAccountsViaApi(allAccountIds);

            // Verify the removed account had its balance restored
            var removedAccountDB = accountsAfterUpdate.FirstOrDefault(a => a.AccountId == removedAccount.AccountId);
            removedAccountDB.Should().NotBeNull();
            var balanceOfRemovedAccount = createdAccounts.FirstOrDefault(a => a.AccountId == removedAccountDB!.AccountId);
            balanceOfRemovedAccount.Should().NotBeNull();

            removedAccountDB!.CurrentBalance.Should().Be(balanceOfRemovedAccount!.CurrentBalance);

            // Verify the new account had its balance updated
            var newAccountBalance = accountsAfterUpdate.First(a => a.AccountId == newAccount.AccountId).CurrentBalance;

            newAccountBalance.Should().Be(transaction.IsExpense ? initialNewAccountBalance - removedAccount.Amount
                                                                : initialNewAccountBalance + removedAccount.Amount);

            // Verify payment transaction counts
            Context.Payments.Where(p => p.TransactionId == createdTransaction.Data.TransactionId).Should().HaveCount(AccountCount);
        }

        [Theory(DisplayName = "Should correctly update savings balance when changing savings' transaction amount")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ShouldUpdateAccountBalances_WhenChangingSavingsTransactionAmount(bool isExpense)
        {
            // Arrange
            var (createdAccounts, transaction) = await SetupAccountsAndTransaction(isExpense, AccountCount, true);

            // Track savings current balances
            var savingsGoalResponse = await SetUpSavingsGoalUsingApi(transaction.SavingGoal);
            var initialSavingsCurrentBalance = savingsGoalResponse.CurrentBalance;

            // First create a transaction
            var createResponse = await PostTransaction(transaction);
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var createdTransaction = await createResponse.Content.ReadFromJsonAsync<Response<TransactionResponse>>();
            createdTransaction.Should().NotBeNull();

            // Get account balances after initial transaction
            var accountsAfterCreate = await GetAccountsViaApi(createdAccounts.Select(a => a.AccountId));

            // Create an updated transaction with different amount
            var updatedTransaction = transaction.Clone();
            updatedTransaction.Amount = transaction.Amount * 2; // Double the amount

            // Act
            var updateResponse = await UpdateTransactionAsync(createdTransaction!.Data!.TransactionId, updatedTransaction);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Get updated account balances
            var accountsAfterUpdate = await GetAccountsViaApi(createdAccounts.Select(a => a.AccountId));

            // Verify savings balances were correctly adjusted
            var savingsGoalDB = Context.SavingsGoals.AsNoTracking().FirstOrDefault(s => s.Id == savingsGoalResponse.Id);
            savingsGoalDB.Should().NotBeNull();
            savingsGoalDB!.CurrentBalance.Should().Be(transaction.IsExpense ? initialSavingsCurrentBalance - updatedTransaction.Amount
                                                                                : initialSavingsCurrentBalance + updatedTransaction.Amount,
                                                       $"because transaction amount was update from {transaction.Amount} to {updatedTransaction.Amount} when Savings Balance was {initialSavingsCurrentBalance}");

            // Assert no other records are created
            Context.Payments.AsNoTracking().Where(p => p.TransactionId == createdTransaction.Data.TransactionId)
                .Should().HaveCount(0);
            Context.SavingsTransactions.AsNoTracking().Where(s => s.TransactionId == createdTransaction.Data.TransactionId)
                .Should().HaveCount(1);

        }

        // Helper method to calculate expected balance after update
        private decimal CalculateExpectedBalance(
            decimal initialBalance,
            TransactionRequest updatedTransaction,
            AccountsResponse originalAccount,
            bool isExpense)
        {
            var accountPayment = updatedTransaction.Payments!.Accounts!
                .FirstOrDefault(a => a.AccountId == originalAccount.AccountId);

            if (accountPayment == null)
                return initialBalance;

            return isExpense
                ? initialBalance - accountPayment.Amount
                : initialBalance + accountPayment.Amount;
        }

        // Helper method to create a test account
        private async Task<AccountsResponse> CreateTestAccount()
        {
            var accountRequest = TestDataGenerator.Generate<AccountsRequest>();
            var response = await PostAccount(accountRequest);
            response.EnsureSuccessStatusCode();
            var account = await response.Content.ReadFromJsonAsync<Response<AccountsResponse>>();
            return account!.Data!;
        }
    }

    // Extension method to clone a transaction
    public static class TransactionExtensions
    {
        public static TransactionRequest Clone(this TransactionRequest transaction)
        {
            var clone = new TransactionRequest
            {
                Amount = transaction.Amount,
                Date = transaction.Date,
                Description = transaction.Description,
                IsExpense = transaction.IsExpense,
                SavingGoal = transaction.SavingGoal,
                Type = transaction.Type,
            };

            if (transaction.Payments != null && transaction.Payments!.Accounts != null && transaction.Payments.Accounts!.Count != 0) {
                clone.Payments = new Payment
                {
                    Accounts = transaction.Payments!.Accounts!.Select(a => new Accounts
                    {
                        AccountId = a.AccountId,
                        Amount = a.Amount
                    }).ToList()
                };
                }

            return clone;
        }
    }
}