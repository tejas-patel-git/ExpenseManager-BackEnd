using FinanceManager.Data;
using FinanceManager.Models.Request;
using FinanceManager.Models.Response;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.FunctionalTest.Tests.TransactionTests
{
    internal static class TransactionAssertion
    {
        internal static void AssertTransactionResponseModel(this TransactionResponse? transactionResponse, TransactionRequest transaction)
        {
            transactionResponse.Should().NotBeNull();
            transactionResponse!.IsExpense.Should().Be(transaction.IsExpense);
            transactionResponse.TransactionType.Should().Be(transaction.Type.ToString());
            transactionResponse.Amount.Should().Be(transaction.Amount);
            transactionResponse.Date.Should().Be(transaction.Date);
            transactionResponse.Description.Should().Be(transaction.Description);
        }

        internal static async Task AssertTransactionWithDB(this TransactionResponse transactionResponse, TransactionRequest expectedTransaction, AppDbContext context)
        {
            var dbTransaction = await context.Transactions
                .Include(t => t.Payments)
                .FirstOrDefaultAsync(t => t.Id == transactionResponse.TransactionId);

            dbTransaction.Should().NotBeNull();
            dbTransaction!.Amount.Should().Be(expectedTransaction.Amount);
            dbTransaction.IsExpense.Should().Be(expectedTransaction.IsExpense);
            dbTransaction.TransactionType.Should().Be((byte)expectedTransaction.Type);
            dbTransaction.Date.Should().Be(expectedTransaction.Date);
            dbTransaction.Description.Should().Be(expectedTransaction.Description);

            dbTransaction.Payments.Should()
                .HaveCount(expectedTransaction.Payments.Accounts.Count);
            dbTransaction.Payments.Sum(p => p.Amount).Should().Be(expectedTransaction.Amount);
        }

        internal static void AssertAccountBalancesAfterTransaction(
            this TransactionRequest transaction,
            List<AccountsResponse> updatedAccounts,
            List<AccountsResponse> originalAccounts,
            bool isExpense)
        {

            var expectedTotalChange = transaction.Amount * (isExpense ? -1 : 1);
            var initialBalances = originalAccounts.ToDictionary(a => a.AccountId, a => a.CurrentBalance);

            updatedAccounts.Sum(a => a.CurrentBalance).Should()
                .Be(originalAccounts.Sum(a => a.CurrentBalance) + expectedTotalChange);

            foreach (var updatedAccount in updatedAccounts)
            {
                var originalBalance = initialBalances[updatedAccount.AccountId];
                var expectedPayment = transaction.Payments.Accounts
                    .First(p => p.AccountId == updatedAccount.AccountId).Amount;

                var expectedBalance = originalBalance + (isExpense ? -expectedPayment : expectedPayment);
                updatedAccount.CurrentBalance.Should().Be(expectedBalance);
            }
        }
    }
}
