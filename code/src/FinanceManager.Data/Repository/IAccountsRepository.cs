﻿using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Repository;
using FinanceManager.Domain.Models;

namespace FinanceManager.Data.Repository
{
    public interface IAccountsRepository : IRepository<AccountsDomain, UserBankAccounts, Guid>
    {
        Task<bool> DeleteByIdAsync(Guid id, string userId);
        Task<bool> UpdateAsync(AccountsDomain accountsDomain);
        Task<bool> UpdateBalance(Guid id, decimal amount);
    }
}
