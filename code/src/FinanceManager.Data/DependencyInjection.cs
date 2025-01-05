﻿using FinanceManager.Data.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManager.Data;

// TODO : Add XML Comments
public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // register
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}