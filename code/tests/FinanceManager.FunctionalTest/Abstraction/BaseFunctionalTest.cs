using FinanceManager.Data;
using FinanceManager.Data.Models;
using FinanceManager.FunctionalTest.TestData;
using FinanceManager.Models.Request;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace FinanceManager.FunctionalTest.Abstraction
{
    public class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>, IAsyncLifetime
    {
        protected readonly string UserId = Guid.NewGuid().ToString();
        protected readonly string BaseUrl = "api";
        protected readonly string SavingsEndpoint;

        public HttpClient HttpClient { get; init; }
        public IServiceProvider ServiceProvider { get; init; }
        protected AppDbContext Context { get; init; }

        public BaseFunctionalTest(FunctionalTestWebAppFactory factory)
        {
            SavingsEndpoint = $"{BaseUrl}/savings";

            HttpClient = factory.CreateClient();
            ServiceProvider = factory.Services;
            Context = ServiceProvider.GetService<AppDbContext>() ?? throw new Exception("Database context not found");
            Context.Database.EnsureCreated();

            // Set the UserId in TestFixtureContext so that each test class has its own user registered in TestAuthHandler
            var testFixtureContext = ServiceProvider.GetRequiredService<TestFixtureContext>();
            testFixtureContext.UserId = UserId;
        }

        protected void DumpTable<TEntity>(DbSet<TEntity> dbSet) where TEntity : class
        {
            var entities = dbSet.ToList();
            foreach (var entity in entities)
            {
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(entity));
            }
        }

        private async Task SeedUserData(AppDbContext context)
        {
            context.Users.Add(TestDataGenerator.Generate<User>(faker => faker.RuleFor(u => u.Id, UserId)));
            await context.SaveChangesAsync();
        }

        public async Task InitializeAsync()
        {
            await SeedUserData(Context);
        }

        public async Task DisposeAsync()
        {
            var accountsToDelete = Context.UserBankAccounts.Where(a => a.UserId == UserId);
            var userToDelete = Context.Users.Where(u => u.Id == UserId);

            Context.UserBankAccounts.RemoveRange(accountsToDelete);
            Context.Users.RemoveRange(userToDelete);

            await Context.SaveChangesAsync();
        }

        protected async Task<HttpResponseMessage> PostSavingsGoal(SavingsRequest savingsRequest)
        {
            return await HttpClient.PostAsJsonAsync(SavingsEndpoint, savingsRequest);
        }

        /// <summary>
        /// Builds a properly formatted URI with query parameters.
        /// </summary>
        protected static string BuildUriWithQuery(string basePath, string paramName, string paramValue)
        {
            return QueryHelpers.AddQueryString(basePath, paramName, paramValue);
        }
    }
}
