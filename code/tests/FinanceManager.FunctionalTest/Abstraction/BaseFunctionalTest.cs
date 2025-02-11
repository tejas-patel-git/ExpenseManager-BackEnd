using FinanceManager.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace FinanceManager.FunctionalTest.Abstraction
{
    public class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public HttpClient HttpClient { get; init; }
        protected AppDbContext Context { get; }

        internal BaseFunctionalTest(FunctionalTestWebAppFactory factory)
        {
            HttpClient = factory.CreateClient();

            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
            Context = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
        }

        protected void DumpTable<TEntity>(DbSet<TEntity> dbSet) where TEntity : class
        {
            var entities = dbSet.ToList();
            foreach (var entity in entities)
            {
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(entity));
            }
        }
    }
}
