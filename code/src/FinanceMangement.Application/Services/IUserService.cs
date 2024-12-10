using FinanceManager.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceMangement.Application.Services
{
    /// <summary>
    /// Service interface for managing user-related business logic.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Retrieves a user by their unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>The user object if found; otherwise, null.</returns>
        Task<User> GetUserByIdAsync(int id);

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A list of all users.</returns>
        Task<IEnumerable<User>> GetAllUsersAsync();

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The user object to create.</param>
        Task CreateUserAsync(User user);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">The updated user object.</param>
        Task UpdateUserAsync(User user);

        /// <summary>
        /// Deletes a user by their unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        Task DeleteUserAsync(int id);
    }

}
