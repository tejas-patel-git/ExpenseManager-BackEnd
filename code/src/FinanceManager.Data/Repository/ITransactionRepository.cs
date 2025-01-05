using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Repository;
using FinanceManager.Domain.Models;

namespace FinanceManager.Data.Repository;

/// <summary>
/// Defines the contract for repository operations related to transactions.
/// </summary>
public interface ITransactionRepository : IRepository<TransactionDomain, Transaction, Guid>
{
    Task<bool> DeleteByIdAsync(Guid id, string userId);
}