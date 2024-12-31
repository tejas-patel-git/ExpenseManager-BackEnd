using FinanceManager.Configuration.AuthHandlers;
using FinanceManager.Configuration.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManager.Configuration
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAuthenticationSchemes(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            // OAuth 2.0 JWT Token Validation
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.Audience = configuration["OAuth:Audience"] ?? throw new ArgumentNullException("Audience is null");
                        options.Authority = configuration["OAuth:Authority"] ?? throw new ArgumentNullException("Issuer is null");
                    })
            // Add API Key Authentication Scheme
                    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ConfigurationConstants.API_KEY_AUTH_SCHEME, options =>
                    {
                        options.Scheme = ConfigurationConstants.API_KEY_AUTH_SCHEME;
                    });

            // Add Authorization Services
            services.AddAuthorization(options =>
            {
                // Add a policy for the ApiKey scheme
                options.AddPolicy(ConfigurationConstants.API_KEY__AUTH_POLICY, policy =>
                {
                    policy.AddAuthenticationSchemes(ConfigurationConstants.API_KEY_AUTH_SCHEME);
                    policy.RequireAuthenticatedUser();
                });
            });

            return services;
        }
    }
}
