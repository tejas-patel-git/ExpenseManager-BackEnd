using FinanceManager.Application.Mapper.Mappers;
using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using FinanceManager.Models.Request;
using FinanceManager.Models.Response;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManager.Application.Mapper
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddObjectMappers(this IServiceCollection services)
        {
            // register mappers

            // transaction model mappers
            // request to domain
            services.AddScoped<IMapper<TransactionRequest, TransactionDomain>, TransactionRequestToDomainMapper>();
            // domain to response
            services.AddScoped<IMapper<TransactionDomain, TransactionResponse>, TransactionDomainToResponseMapper>();
            // domain to entity
            services.AddScoped<IMapper<TransactionDomain, Transaction>, TransactionDomainToEntityMapper>();
            // entity to domain
            services.AddScoped<IMapper<Transaction, TransactionDomain>, TransactionEntityToDomainMapper>();


            return services;
        }
    }
}
