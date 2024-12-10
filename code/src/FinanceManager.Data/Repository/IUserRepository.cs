using FinanceManager.Data.Models;

namespace FinanceManager.Data.Repository
{
    /// <summary>
    /// Interface for managing User entities in the data store.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user with the specified ID, or null if not found.</returns>
        Task<User> GetUserByIdAsync(int id);

        /// <summary>
        /// Retrieves all users in the data store.
        /// </summary>
        /// <returns>A collection of all users.</returns>
        Task<IEnumerable<User>> GetAllUsersAsync();

        /// <summary>
        /// Creates a new user in the data store.
        /// </summary>
        /// <param name="user">The user entity to create.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CreateUserAsync(User user);

        /// <summary>
        /// Updates an existing user in the data store.
        /// </summary>
        /// <param name="user">The updated user entity.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateUserAsync(User user);

        /// <summary>
        /// Deletes a user from the data store by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteUserAsync(int id);
    }
}