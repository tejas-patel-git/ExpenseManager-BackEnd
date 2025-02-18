using FinanceManager.FunctionalTest.Abstraction;
using FinanceManager.Models.Response;
using Microsoft.AspNetCore.WebUtilities;
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
    }
}
