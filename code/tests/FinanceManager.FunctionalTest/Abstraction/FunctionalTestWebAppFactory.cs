using FinanceManager.Data;
using FinanceManager.FunctionalTest.AuthHandler;
using FinanceManager.FunctionalTest.Configuration;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json;

namespace FinanceManager.FunctionalTest.Abstraction
{
    public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(async services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(AppDbContext));
                services.RemoveAll(typeof(IDbContextOptions));

                // Register InMemory database for testing
                services.AddEntityFrameworkInMemoryDatabase().
                AddDbContext<AppDbContext>((sp, options) =>
                {
                    //options.UseInMemoryDatabase("TestDatabase")
                    options.UseSqlServer("Server=\\SQLEXPRESS;Database=ExpenseManagerTest;Trusted_Connection=True;Encrypt=False")
                           .EnableSensitiveDataLogging(true)
                           .EnableDetailedErrors();
                });

                // Build the service provider
                var sp = services.BuildServiceProvider();

                // Create a scope to initialize the database
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureDeleted(); // Ensure a clean state before each test
                db.Database.EnsureCreated();

                // seed
                SeedUserData(db);
            });

            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

                services.AddScoped(_ => new AuthClaimsProvider());
            });
        }

        private async void SeedUserData(AppDbContext db)
        {
            db.Users.Add(new Data.Models.User()
            {
                Id = TestConstants.UserId,
                Email = "fm-svc-functional-test@gmail.com",
                IsEmailVerified = false,
                FamilyName = "Doe",
                GivenName = "John",
                FullName = "John Doe",
                Nickname = "johndoe",
                PhoneNumber = "123-456-7890",
                AppMetadata = JsonSerializer.Serialize("{}"),
                UserMetadata = JsonSerializer.Serialize("{}"),
                PictureUrl = "http://www.gravatar.com/avatar/?d=identicon",
                LastPasswordReset = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });

            db.SaveChanges();
        }
    }
}
