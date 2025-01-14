using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
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
                existingAccount.InitialBalance = accountsDomain.InitialBalance;
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

        public virtual async Task<bool> DeleteByIdAsync(Guid id, string userId)
        {
            try
            {
                var entity = await dbSet.FirstOrDefaultAsync(entity => entity.UserId == userId && entity.Id == id);

                if (entity == null)
                {
                    _logger.LogInformation("'{type}' with id {id} not found for deletion.", nameof(UserBankAccounts), id);
                    return false;
                }

                Delete(entity);
                _logger.LogInformation("'{type}' with id {id} deleted successfully.", nameof(UserBankAccounts), id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting '{type}' id {id}.", nameof(UserBankAccounts), id);
                throw;
            }
        }

        public async Task<bool> UpdateBalance(Guid id, decimal amount)
        {
            try
            {
                // retrieve the existing account from the database
                var existingAccount = await dbSet.FindAsync(id);

                if (existingAccount == null)
                {
                    _logger.LogWarning("'{type}' with id {id} not found.", nameof(UserBankAccounts), id);
                    return false;
                }

                // update current balance
                existingAccount.CurrentBalance += amount;

                _logger.LogInformation("Balance updated for '{type}' id {id}.", nameof(UserBankAccounts), id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating balance of '{type}' id {id}.", nameof(UserBankAccounts), id);
                throw;
            }
        }
    }
}