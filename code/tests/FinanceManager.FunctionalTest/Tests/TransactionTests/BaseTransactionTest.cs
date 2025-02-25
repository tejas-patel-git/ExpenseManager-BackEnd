﻿using FinanceManager.Domain.Enums;
using FinanceManager.FunctionalTest.Abstraction;
using FinanceManager.FunctionalTest.TestData;
using FinanceManager.Models.Request;
using FinanceManager.Models.Response;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Net.Http.Json;

namespace FinanceManager.FunctionalTest.Tests.TransactionTests
{
    public class BaseTransactionTest : BaseFunctionalTest
    {
        private readonly string TransactionEndpoint;
        private readonly string AccountEndpoint;
        private readonly string QueryParamId = "id";

        internal BaseTransactionTest(FunctionalTestWebAppFactory factory) : base(factory)
        {
            TransactionEndpoint = $"{BaseUrl}/transaction";
            AccountEndpoint = $"{BaseUrl}/accounts";
        }

        protected async Task<HttpResponseMessage> PostTransaction(object transactionPayload)
        {
            return await HttpClient.PostAsJsonAsync(TransactionEndpoint, transactionPayload);
        }

        protected async Task<HttpResponseMessage> PostAccount(object accountPayload)
        {
            return await HttpClient.PostAsJsonAsync(AccountEndpoint, accountPayload);
        }

        protected async Task<TransactionResponse?> GetTransactionByIdAsync(Guid transactionId)
        {
            var requestUri = BuildUriWithQuery(TransactionEndpoint, QueryParamId, transactionId.ToString());
            var response = await HttpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TransactionResponse>();
        }

        protected async Task<HttpResponseMessage> GetAccountsById(Guid accountId)
        {
            var requestUri = BuildUriWithQuery(AccountEndpoint, QueryParamId, accountId.ToString());
            return await HttpClient.GetAsync(requestUri);
        }

        protected async Task<List<TransactionResponse>?> GetAllTransactionsAsync()
        {
            var response = await HttpClient.GetAsync(TransactionEndpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<TransactionResponse>>();
        }

        protected async Task DeleteTransactionAsync(Guid transactionId)
        {
            var requestUri = BuildUriWithQuery(TransactionEndpoint, QueryParamId, transactionId.ToString());
            var response = await HttpClient.DeleteAsync(requestUri);
            response.EnsureSuccessStatusCode();
        }

        protected async Task UpdateTransactionAsync(Guid transactionId, object updatedTransaction)
        {
            var requestUri = BuildUriWithQuery(TransactionEndpoint, QueryParamId, transactionId.ToString());
            var response = await HttpClient.PutAsJsonAsync(requestUri, updatedTransaction);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Builds a properly formatted URI with query parameters.
        /// </summary>
        private static string BuildUriWithQuery(string basePath, string paramName, string paramValue)
        {
            return QueryHelpers.AddQueryString(basePath, paramName, paramValue);
        }

        protected void DistributeTransactionAmount(TransactionRequest transaction, IReadOnlyCollection<Guid> accountIds)
        {
            var paymentAmount = (int)transaction.Amount / accountIds.Count;
            var test = transaction.Amount % accountIds.Count;
            var remainingAmount = transaction.Amount - (paymentAmount * accountIds.Count);

            foreach (var accountId in accountIds)
            {
                transaction.Payments.Accounts.Add(new Models.Accounts
                {
                    AccountId = accountId,
                    Amount = paymentAmount
                });
            }

            transaction.Payments.Accounts.First().Amount += remainingAmount;
        }

        protected async Task<(List<AccountsResponse> CreatedAccounts, TransactionRequest Transaction)>
           SetupAccountsAndTransaction(bool isExpense, int numberOfAccounts, bool isSavingsTransaction = false)
        {
            var transaction = TestDataGenerator.Generate<TransactionRequest>(cfg =>
                cfg.RuleFor(t => t.IsExpense, isExpense)
                   .RuleFor(t => t.Type, (f, t) =>
                   {
                       if (isSavingsTransaction)
                       {
                           return TransactionType.Savings;
                       }
                       else
                       {
                           if (t.IsExpense)
                               return f.PickRandomWithout([TransactionType.Undefined, TransactionType.Income, TransactionType.Savings]);
                           else
                               return f.PickRandomWithout([TransactionType.Undefined, TransactionType.Expense, TransactionType.Savings]);
                       }
                   })
            );

            var accountRequests = TestDataGenerator.GenerateMany<AccountsRequest>(numberOfAccounts, faker =>
            {
                faker.RuleFor(a => a.AccountName, f => $"{f.Finance.AccountName()}_{f.UniqueIndex}");
            });
            var createdAccounts = await CreateAccountsViaApi(accountRequests);

            if (isSavingsTransaction)
            {
                transaction.Payments = null;
            }
            else
            {
                DistributeTransactionAmount(transaction, createdAccounts.Select(a => a.AccountId).ToList());
            }
            return (createdAccounts, transaction);
        }

        protected async Task<List<AccountsResponse>> CreateAccountsViaApi(IEnumerable<AccountsRequest> requests)
        {
            var createdAccounts = new List<AccountsResponse>();

            foreach (var request in requests)
            {
                var response = await PostAccount(request);
                response.StatusCode.Should().Be(HttpStatusCode.OK, $"{response.Content.ReadAsStringAsync().Result}");

                var content = await response.Content.ReadFromJsonAsync<Response<AccountsResponse>>();
                createdAccounts.Add(content!.Data!);
            }

            return createdAccounts;
        }

        protected async Task<List<AccountsResponse>> GetAccountsViaApi(IEnumerable<Guid> accountIds)
        {
            var fetchedAccounts = new List<AccountsResponse>();

            foreach (var id in accountIds)
            {
                var response = await GetAccountsById(id);
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var content = await response.Content.ReadFromJsonAsync<Response<AccountsResponse>>();
                content.Should().NotBeNull();
                content!.Data.Should().NotBeNull();

                fetchedAccounts.Add(content.Data!);
            }

            return fetchedAccounts;
        }
    }
}
