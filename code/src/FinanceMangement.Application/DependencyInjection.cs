using FinanceMangement.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceMangement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // register services
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
