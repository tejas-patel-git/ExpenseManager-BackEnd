﻿using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Repository;
using FinanceManager.Domain.Models;

namespace FinanceManager.Data.Repository
{
    public interface ISavingsGoalRepository : IRepository<SavingsGoalDomain, SavingsGoal, Guid>
    {
        Task<bool> UpdateBalance(Guid id, decimal amount);
    }
}
