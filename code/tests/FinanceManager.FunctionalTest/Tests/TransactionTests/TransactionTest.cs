using FinanceManager.Data.Models;
using FinanceManager.FunctionalTest.Abstraction;
using FinanceManager.FunctionalTest.TestData;
using FinanceManager.Models.Request;
using FinanceManager.Models.Response;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace FinanceManager.FunctionalTest.Tests.TransactionTests
{
    public class TransactionTest : BaseTransactionTest
    {
        public TransactionTest(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact(DisplayName = "Add New Transaction - Should Return Correct Data")]
        public async Task AddNewTransaction_ReturnsExpectedTransaction()
        {
            // Arrange
            var fakeAccounts = TestDataGenerator.GenerateMany<UserBankAccounts>(3, faker => faker.RuleFor(a => a.UserId, UserId));
            await Context.UserBankAccounts.AddRangeAsync(fakeAccounts);
            await Context.SaveChangesAsync();

            var newTransaction = TestDataGenerator.Generate<TransactionRequest>();

            // divide the transaction amount equally in all accounts
            int eachAccountTransactionAmount = (int)(newTransaction.Amount / fakeAccounts.Count);
            foreach (var account in fakeAccounts) newTransaction.Payments.Accounts.Add(new() { AccountId = account.Id, Amount = eachAccountTransactionAmount });
            newTransaction.Payments.Accounts.ToList()[0].Amount += newTransaction.Amount - (eachAccountTransactionAmount * fakeAccounts.Count);
            //var currentBalance = Context.UserBankAccounts

            // Act
            var response = await PostTransaction(newTransaction);

            // Assert: Response data
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var transactionResponse = await response.Content.ReadFromJsonAsync<Response<TransactionResponse>>();

            // Assert: Validate transaction properties
            transactionResponse.ValidateTransactionResponseModel(newTransaction);

            // Assert: Validate transaction in the database
            var dbTransaction = await Context.Transactions
                .Include(t => t.Payments) // Include related payments
                .FirstOrDefaultAsync(t => t.Id == transactionResponse.Data.TransactionId);
            dbTransaction.Should().NotBeNull();

            dbTransaction!.IsExpense.Should().Be(newTransaction.IsExpense);
            dbTransaction.Amount.Should().Be(newTransaction.Amount);
            dbTransaction.Date.Should().Be(newTransaction.Date);
            dbTransaction.Description.Should().Be(newTransaction.Description);

            // Assert: Validate payments in the database
            dbTransaction.Payments.Should().NotBeNull();
            dbTransaction.Payments.Should().HaveCount(newTransaction.Payments.Accounts.Count);
            dbTransaction.Payments.Sum(p => p.Amount).Should().Be(newTransaction.Amount);
        }

        //[Fact(DisplayName = "Get Transaction By Id - Should Return a Valid Transaction")]
        //public async Task GetTransactionById_ReturnsValidTransaction()
        //{
        //    // Arrange
        //    var newTransaction = new TransactionRequest
        //    {
        //        IsExpense = false,
        //        Amount = 50M,
        //        Date = DateTime.Parse("2024-11-01T10:00:00"),
        //        Description = "Test Transaction",
        //        Payments = new Payment
        //        {
        //            Accounts =
        //            [
        //                new Accounts { AccountId = Guid.Parse("07250AAA-6E32-4389-837E-2F60CB7FED42"), Amount = 25M },
        //                new Accounts { AccountId = Guid.Parse("D95654CE-DBF3-4C3B-876B-6C3A2B41808A"), Amount = 25M }
        //            ]
        //        }
        //    };

        //    var transactionId = await CreateTestTransactionAsync(newTransaction);

        //    // Act
        //    var transaction = await GetTransactionByIdAsync(transactionId.Value);

        //    // Assert
        //    transaction.Should().NotBeNull();
        //    transaction!.TransactionId.Should().Be(transactionId.Value);
        //    transaction.Amount.Should().Be(50M);
        //}

        //[Fact(DisplayName = "Get All Transactions - Should Return a List with Required Fields")]
        //public async Task GetAllTransactions_ReturnsTransactionsWithRequiredFields()
        //{
        //    // Act
        //    var response = await GetAllTransactionsAsync();


        //    // Assert
        //    foreach (var transaction in response)
        //    {
        //        transaction.TransactionId.Should().NotBeEmpty();
        //        transaction.Description.Should().NotBeNullOrEmpty();
        //        transaction.Amount.Should().BeGreaterOrEqualTo(0);
        //    }
        //}

        //[Fact(DisplayName = "Update Transaction - Should Modify Transaction Data")]
        //public async Task UpdateTransaction_UpdatesExistingTransaction()
        //{
        //    // Arrange
        //    var newTransaction = new TransactionRequest
        //    {
        //        IsExpense = true,
        //        Amount = 10M,
        //        Date = DateTime.Parse("2024-11-01T10:00:00"),
        //        Description = "Update Test Transaction",
        //        Payments = new Payment
        //        {
        //            Accounts =
        //            [
        //                new Accounts { AccountId = Guid.Parse("07250AAA-6E32-4389-837E-2F60CB7FED42"), Amount = 5M },
        //                new Accounts { AccountId = Guid.Parse("D95654CE-DBF3-4C3B-876B-6C3A2B41808A"), Amount = 5M }
        //            ]
        //        }
        //    };

        //    var transactionId = await CreateTestTransactionAsync(newTransaction);

        //    var updatedTransaction = new TransactionRequest
        //    {
        //        IsExpense = false,
        //        Amount = 51M,
        //        Date = DateTime.Parse("2024-11-01T10:00:00"),
        //        Description = "Test Grocery Shopping Updated",
        //        Payments = new Payment
        //        {
        //            Accounts =
        //            [
        //                new Accounts { AccountId = Guid.Parse("07250AAA-6E32-4389-837E-2F60CB7FED42"), Amount = 26M },
        //                new Accounts { AccountId = Guid.Parse("D95654CE-DBF3-4C3B-876B-6C3A2B41808A"), Amount = 25M }
        //            ]
        //        }
        //    };

        //    // Act
        //    await UpdateTransactionAsync(transactionId.Value, updatedTransaction);
        //    var updatedResult = await GetTransactionByIdAsync(transactionId.Value);

        //    // Assert
        //    updatedResult?.IsExpense.Should().BeFalse();
        //    updatedResult!.Amount.Should().Be(51M);
        //    updatedResult.Description.Should().Be("Test Grocery Shopping Updated");
        //}

        //[Fact(DisplayName = "Delete Transaction - Should Remove Transaction")]
        //public async Task DeleteTransaction_RemovesTransaction()
        //{
        //    // Arrange
        //    var newTransaction = new TransactionRequest
        //    {
        //        IsExpense = false,
        //        Amount = 20M,
        //        Date = DateTime.Parse("2024-11-01T10:00:00"),
        //        Description = "Delete Test Transaction",
        //        Payments = new Payment
        //        {
        //            Accounts =
        //            [
        //                new Accounts { AccountId = Guid.Parse("07250AAA-6E32-4389-837E-2F60CB7FED42"), Amount = 10M },
        //                new Accounts { AccountId = Guid.Parse("D95654CE-DBF3-4C3B-876B-6C3A2B41808A"), Amount = 10M }
        //            ]
        //        }
        //    };

        //    var transactionId = await CreateTestTransactionAsync(newTransaction);

        //    // Act
        //    await DeleteTransactionAsync(transactionId.Value);

        //    // Assert
        //}
    }
}
