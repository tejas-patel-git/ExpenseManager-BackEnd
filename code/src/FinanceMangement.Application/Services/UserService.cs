using FinanceManager.Data.Models;
using FinanceManager.Data.Repository;

namespace FinanceMangement.Application.Services
{
    // TODO : Use DTOs

    /// <summary>
    /// Service implementation for managing user-related business logic.
    /// </summary>
    /// <inheritdoc/>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepository">The repository for accessing user data.</param>
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <inheritdoc/>
        public async Task<User> GetUserByIdAsync(int id) => await _userRepository.GetUserByIdAsync(id);

        /// <inheritdoc/>
        public async Task<IEnumerable<User>> GetAllUsersAsync() => await _userRepository.GetAllUsersAsync();

        /// <inheritdoc/>
        public async Task CreateUserAsync(User user) => await _userRepository.CreateUserAsync(user);

        /// <inheritdoc/>
        public async Task UpdateUserAsync(User user) => await _userRepository.UpdateUserAsync(user);

        /// <inheritdoc/>
        public async Task DeleteUserAsync(int id) => await _userRepository.DeleteUserAsync(id);
    }

}
