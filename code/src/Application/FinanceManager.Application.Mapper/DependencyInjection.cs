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
            services.AddSingleton<IMapper<TransactionRequest, TransactionDomain>, TransactionRequestToDomainMapper>();
            services.AddSingleton<IMapper<TransactionDomain, TransactionResponse>, TransactionDomainToResponseMapper>();
            services.AddSingleton<IMapper<TransactionDomain, Transaction>, TransactionDomainToEntityMapper>();
            services.AddSingleton<IMapper<Transaction, TransactionDomain>, TransactionEntityToDomainMapper>();

            // user model mappers
            services.AddSingleton<IMapper<UserRegistrationRequest, UserDomain>, UserRegistrationRequestToDomainMapper>();
            services.AddSingleton<IMapper<UserDomain, User>, UserDomainToEntityMapper>();
            services.AddSingleton<IMapper<User, UserDomain>, UserEntityToDomainMapper>();

            // accounts model mappers
            services.AddSingleton<IMapper<AccountsRequest, AccountsDomain>, AccountsRequestToDomainMapper>();
            services.AddSingleton<IMapper<AccountsDomain, UserBankAccounts>, AccountsMapper>();
            services.AddSingleton<IMapper<UserBankAccounts, AccountsDomain>, AccountsEntityToDomainMapper>();
            services.AddSingleton<IMapper<AccountsDomain, AccountsResponse>, AccountsDomainToResponseMapper>();


            return services;
        }
    }
}
