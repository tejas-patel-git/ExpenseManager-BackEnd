using FinanceManager.FunctionalTest.Abstraction;
using FinanceManager.Models.Request;
using System.Net.Http.Json;

namespace FinanceManager.FunctionalTest.Tests.SavingsTests
{
    public class BaseSavingsTest : BaseFunctionalTest
    {
        private readonly string QueryParamId = "id";

        public BaseSavingsTest(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        public async Task<HttpResponseMessage> DeleteSavingsGoalAsync(Guid id)
        {
            var requestUri = BuildUriWithQuery(SavingsEndpoint, QueryParamId, id.ToString());
            return await HttpClient.DeleteAsync(requestUri);
        }

        public async Task<HttpResponseMessage> GetSavingsGoalByIdAsync(Guid id)
        {
            var requestUri = BuildUriWithQuery(SavingsEndpoint, QueryParamId, id.ToString());
            return await HttpClient.GetAsync(requestUri);
        }

        public async Task<HttpResponseMessage> GetSavingsGoalsAsync()
        {
            return await HttpClient.GetAsync(SavingsEndpoint);
        }

        public async Task<HttpResponseMessage> UpdateSavingsGoalAsync(Guid id, SavingsRequest request)
        {
            var requestUri = BuildUriWithQuery(SavingsEndpoint, QueryParamId, id.ToString());
            return await HttpClient.PutAsJsonAsync(requestUri, request);
        }
    }
}