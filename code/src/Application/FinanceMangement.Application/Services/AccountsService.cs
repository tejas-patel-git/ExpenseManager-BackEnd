using FinanceManager.Data;
using FinanceManager.Domain.Models;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Application.Services
{
    internal class AccountsService : BaseService, IAccountsService
    {
        private readonly ILogger<AccountsService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public AccountsService(ILogger<AccountsService> logger,
                               IUnitOfWork unitOfWork)
            : base(unitOfWork.UserRepository)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc/>
        public async Task<AccountsDomain?> GetAccounts(Guid accountId, string userId)
        {
            if (accountId.Equals(Guid.Empty))
            {
                throw new ArgumentException($"Invalid transaction id.", nameof(accountId));
            }
            ArgumentNullException.ThrowIfNullOrEmpty(userId, nameof(userId));


            // Fetch data from repository
            var accountsDomain = await _unitOfWork.AccountsRepository.GetByIdAsync(accountId);

            // Return null if not found
            if (accountsDomain == null) return null;

            // validate if transaction belongs to the user
            if (accountsDomain.UserId != userId) return null;

            // Return dto of fetched data
            return accountsDomain;
        }

        public async Task<IEnumerable<AccountsDomain>> GetAccounts(string userId)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(userId, nameof(userId));

            // fetch data from repository
            var accounts = await _unitOfWork.AccountsRepository.GetAllAsync(entity => entity.UserId == userId);

            // return empty collection if not found
            if (!accounts.Any()) return [];

            // Return dto of fetched data
            return accounts;
        }

        public async Task<bool> AddAccount(AccountsDomain accountsDomain)
        {
            ArgumentNullException.ThrowIfNull(accountsDomain, nameof(accountsDomain));

            if (!await UserExists(accountsDomain.UserId))
                return false;

            accountsDomain.Id = Guid.NewGuid();

            await _unitOfWork.AccountsRepository.AddAsync(accountsDomain);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateAccountAsync(AccountsDomain accountsDomain)
        {
            ArgumentNullException.ThrowIfNull(accountsDomain);

            // update data to repository
            var isSuccess = await _unitOfWork.AccountsRepository.UpdateAsync(accountsDomain);
            if (!isSuccess) return false;

            var rowsUpdated = await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"{rowsUpdated} rows updated while updating {typeof(TransactionDomain).Name}");

            return true;
        }

        public async Task<bool> DeleteTransactionAsync(Guid id, string userId)
        {
            // Delete data from repository
            var isSuccess = await _unitOfWork.AccountsRepository.DeleteByIdAsync(id, userId);
            await _unitOfWork.SaveChangesAsync();

            return isSuccess;
        }

        public async Task<bool> Exists(ICollection<Guid> ids)
        {
            ArgumentNullException.ThrowIfNull(ids, nameof(ids));
            return await _unitOfWork.AccountsRepository.ExistsAsync(ids);
        }

        public async Task<bool> Exists(ICollection<Guid> ids, string userId)
        {
            ArgumentNullException.ThrowIfNull(ids, nameof(ids));
            return await _unitOfWork.AccountsRepository.ExistsAsync(ids, acc => acc.UserId == userId);
        }

        public async Task<bool> UpdateCurrentBalance(Guid id, decimal amount)
        {
            return await _unitOfWork.AccountsRepository.UpdateBalance(id, amount);
        }
    }
}
