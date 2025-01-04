using FinanceManager.Domain.Models;

namespace FinanceManager.Application.Services
{
    /// <summary>
    /// Service interface for managing user-related business logic.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The user object to create.</param>
        Task<bool> CreateUserAsync(UserDomain user);
        Task<bool> UserExistsAsync(string userId);
    }
}
