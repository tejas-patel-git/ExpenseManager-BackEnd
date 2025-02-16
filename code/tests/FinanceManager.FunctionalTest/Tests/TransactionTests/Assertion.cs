using FinanceManager.Models.Request;
using FinanceManager.Models.Response;
using FluentAssertions;

namespace FinanceManager.FunctionalTest.Tests.TransactionTests
{
    internal static class Assertion
    {
        internal static void ValidateTransactionResponseModel(this Response<TransactionResponse>? transactionResponse, TransactionRequest newTransaction)
        {
            transactionResponse.Should().NotBeNull();
            transactionResponse!.Data.Should().NotBeNull();

            transactionResponse.Data!.IsExpense.Should().Be(newTransaction.IsExpense);
            transactionResponse.Data.Amount.Should().Be(newTransaction.Amount);
            transactionResponse.Data.Date.Should().Be(newTransaction.Date);
            transactionResponse.Data.Description.Should().Be(newTransaction.Description);
        }
    }
}
