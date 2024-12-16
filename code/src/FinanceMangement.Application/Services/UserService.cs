using FinanceManager.Data;
using FinanceManager.Data.Models;
using FinanceManager.Data.Repository;

namespace FinanceManager.Application.Services;

// TODO : Use DTOs

/// <summary>
/// Service implementation for managing user-related business logic.
/// </summary>
/// <inheritdoc/>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The repository for accessing user data.</param>
    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task<User> GetUserByIdAsync(int id) => await _unitOfWork.UserRepository.GetUserByIdAsync(id);

    /// <inheritdoc/>
    public async Task<IEnumerable<User>> GetAllUsersAsync() => await _unitOfWork.UserRepository.GetAllUsersAsync();

    /// <inheritdoc/>
    public async Task CreateUserAsync(User user) => await _unitOfWork.UserRepository.CreateUserAsync(user);

    /// <inheritdoc/>
    public async Task UpdateUserAsync(User user) => await _unitOfWork.UserRepository.UpdateUserAsync(user);

    /// <inheritdoc/>
    public async Task DeleteUserAsync(int id) => await _unitOfWork.UserRepository.DeleteUserAsync(id);
}
