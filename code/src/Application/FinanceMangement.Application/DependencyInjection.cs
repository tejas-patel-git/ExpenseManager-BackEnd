using FinanceManager.Application.Services;
using FinanceManager.Application.Validator;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManager.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // register services
            services.AddScoped<ICalculateBalance, CalculateBalance>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccountsService, AccountsService>();

            services.AddValidatorsFromAssemblyContaining<TransactionRequestValidator>();
            services.AddFluentValidationAutoValidation();

            return services;
        }
    }
}
