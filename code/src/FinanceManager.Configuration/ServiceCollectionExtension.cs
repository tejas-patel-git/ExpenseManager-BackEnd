using FinanceManager.Configuration.AuthHandlers;
using FinanceManager.Configuration.Models;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManager.Configuration
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAuthenticationSchemes(this IServiceCollection services)
        {
            // Add API Key Authentication Scheme
            services.AddAuthentication(ConfigurationConstants.API_KEY_AUTH_SCHEME)
                    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ConfigurationConstants.API_KEY_AUTH_SCHEME, options =>
                    {
                        options.Scheme = ConfigurationConstants.API_KEY_AUTH_SCHEME;
                    });

            return services;
        }
    }
}
