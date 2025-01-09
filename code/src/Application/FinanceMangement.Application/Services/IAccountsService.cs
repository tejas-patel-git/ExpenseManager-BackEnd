﻿using FinanceManager.Domain.Models;

namespace FinanceManager.Application.Services
{
    public interface IAccountsService
    {
        Task<bool> AddAccount(AccountsDomain accountsDomain);
        Task<bool> DeleteTransactionAsync(Guid id, string userId);
        Task<AccountsDomain?> GetAccounts(Guid accountId, string userId);
        Task<IEnumerable<AccountsDomain>> GetAccounts(string userId);
        Task<bool> UpdateAccountAsync(AccountsDomain accountsDomain);
    }
}