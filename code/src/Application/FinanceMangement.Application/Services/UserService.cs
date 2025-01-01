using FinanceManager.Data;
using FinanceManager.Domain.Models;

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
    public async Task<bool> CreateUserAsync(UserDomain user)
    {
        if (await _unitOfWork.UserRepository.ExistsByEmailAsync(user.Email))
        {
            return false;
        }

        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
