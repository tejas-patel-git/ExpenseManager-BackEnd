using FinanceManager.Data;
using FinanceManager.FunctionalTest.AuthHandler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FinanceManager.FunctionalTest.Abstraction
{
    public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(AppDbContext));
                services.RemoveAll(typeof(DbContext));

                services.AddDbContext<AppDbContext>(options =>
                {
                    var dbName = $"ExpenseManagerTest";

                    options.UseSqlServer($"Server=DESKTOP-D5245NT\\SQLEXPRESS;Database={dbName};User Id=FunctionalTestUser;Password=FunctionalTestUser;TrustServerCertificate=True")
                           .EnableSensitiveDataLogging(true)
                           .EnableDetailedErrors();
                });
            });

            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(TestAuthHandler.SchemeName)
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

                services.AddSingleton<TestFixtureContext>();
                services.AddScoped<AuthClaimsProvider>();
            });
        }
    }
}
