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
            services.AddSingleton<IMapper<TransactionRequest, TransactionDomain>, TransactionRequestToDomainMapper>();
            // domain to response
            services.AddSingleton<IMapper<TransactionDomain, TransactionResponse>, TransactionDomainToResponseMapper>();
            // domain to entity
            services.AddSingleton<IMapper<TransactionDomain, Transaction>, TransactionDomainToEntityMapper>();
            // entity to domain
            services.AddSingleton<IMapper<Transaction, TransactionDomain>, TransactionEntityToDomainMapper>();


            return services;
        }
    }
}
