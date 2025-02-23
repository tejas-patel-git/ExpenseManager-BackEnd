using FinanceManager.Data.Models;
using FinanceManager.Data.Repository;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Abstraction.Repository;
using FinanceManager.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Data;

// TODO : Add XML Comments
public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // register
        services.AddScoped<IPaymentRepository, PaymentsRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAccountsRepository, AccountsRepository>();
        services.AddScoped<ISavingsTransactionRepository, SavingsTransactionRepository>();
        services.AddScoped<ISavingsGoalRepository, SavingsGoalRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
