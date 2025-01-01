using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Repository;
using FinanceManager.Domain.Models;

namespace FinanceManager.Data.Repository
{
    /// <summary>
    /// Interface for managing User entities in the data store.
    /// </summary>
    public interface IUserRepository : IRepository<UserDomain, User, Guid>
    {
        Task<bool> ExistsByEmailAsync(string email);
        Task<UserDomain?> GetByEmailAsync(string email);
    }
}