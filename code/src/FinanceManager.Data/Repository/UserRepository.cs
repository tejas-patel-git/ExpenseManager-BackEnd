using FinanceManager.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Data.Repository;

/// <summary>
/// Repository for managing User entities.
/// </summary>
internal class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    internal UserRepository(AppDbContext context, ILogger<UserRepository> logger) : base(context, logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<User?> GetUserByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("GetUserByIdAsync called with invalid ID: {Id}", id);
            return null;
        }

        return await dbSet.FindAsync(id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await dbSet.AsNoTracking().ToListAsync();
    }

    /// <inheritdoc/>
    public async Task CreateUserAsync(User user)
    {
        if (user == null)
        {
            _logger.LogError("CreateUserAsync called with null user");
            throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        try
        {
            await dbSet.AddAsync(user);
            _logger.LogInformation("User created successfully with ID: {Id}", user.UserID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateUserAsync(User user)
    {
        if (user == null)
        {
            _logger.LogError("UpdateUserAsync called with null user");
            throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        var existingUser = await GetUserByIdAsync(user.UserID);
        if (existingUser == null)
        {
            _logger.LogWarning("UpdateUserAsync called for non-existing user with ID: {Id}", user.UserID);
            throw new KeyNotFoundException($"User with ID {user.UserID} not found");
        }

        existingUser.FirstName = user.FirstName ?? existingUser.FirstName;
        existingUser.LastName = user.LastName ?? existingUser.LastName;
        existingUser.Email = user.Email ?? existingUser.Email;

        try
        {
            dbSet.Update(existingUser);
            _logger.LogInformation("User updated successfully with ID: {Id}", user.UserID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user with ID: {Id}", user.UserID);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteUserAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("DeleteUserAsync called with invalid ID: {Id}", id);
            throw new ArgumentException("ID must be greater than zero", nameof(id));
        }

        var user = await GetUserByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("DeleteUserAsync called for non-existing user with ID: {Id}", id);
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        try
        {
            dbSet.Remove(user);
            _logger.LogInformation("User deleted successfully with ID: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user with ID: {Id}", id);
            throw;
        }
    }
}
