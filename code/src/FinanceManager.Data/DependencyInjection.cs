using Microsoft.Extensions.DependencyInjection;

namespace FinanceManager.Data;

// TODO : Add XML Comments
public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // register
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
