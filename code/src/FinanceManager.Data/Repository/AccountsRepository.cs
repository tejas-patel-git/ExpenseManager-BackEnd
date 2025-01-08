using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Data.Repository
{
    internal class AccountsRepository : Repository<AccountsDomain, UserBankAccounts, Guid>, IAccountsRepository
    {
        private readonly ILogger<AccountsRepository> _logger;

        public AccountsRepository(AppDbContext context,
                                 ILogger<AccountsRepository> logger,
                                 IMapper<AccountsDomain, UserBankAccounts> domainEntityMapper,
                                 IMapper<UserBankAccounts, AccountsDomain> entityDomainMapper)
        : base(context, logger, domainEntityMapper, entityDomainMapper)
        {
            _logger = logger;
        }


        public override async Task<bool> UpdateAsync(AccountsDomain accountsDomain)
        {
            // TODO : Revisit logic - might need some tweaks related to how to handle FKs
            try
            {
                // Retrieve the existing transaction from the database
                var existingAccount = await dbSet.FindAsync(accountsDomain.Id);

                if (existingAccount == null)
                {
                    _logger.LogWarning("'{type}' with id {id} not found.", nameof(UserBankAccounts), accountsDomain.Id);
                    return false;
                }

                // Update the properties of the existing entity
                existingAccount.AccountNumber = accountsDomain.AccountNumber;
                existingAccount.AccountName = accountsDomain.AccountName;
                existingAccount.BankName = accountsDomain.BankName.ToString();
                existingAccount.AccountType = accountsDomain.AccountType.ToString();
                existingAccount.Balance = accountsDomain.Balance;
                existingAccount.UpdatedAt = DateTime.UtcNow;


                _logger.LogInformation($"'{typeof(Transaction).Name}' with id {existingAccount.Id} updated.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating '{typeof(Transaction).Name}' with id {accountsDomain.Id}.");
                throw;
            }
        }
    }
}