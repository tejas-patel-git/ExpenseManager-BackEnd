using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Data.Repository;

/// <summary>
/// Repository for managing User entities.
/// </summary>
internal class UserRepository : Repository<UserDomain, User, string>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="domainEntityMapper"></param>
    /// <param name="entityDomainMapper"></param>
    public UserRepository(AppDbContext context,
                          ILogger<UserRepository> logger,
                          IMapper<UserDomain, User> domainEntityMapper,
                          IMapper<User, UserDomain> entityDomainMapper)
        : base(context, logger, domainEntityMapper, entityDomainMapper)
    {
        _logger = logger;
    }

    public async Task<string?> GetUserIdAsync(string id)
    {
        try
        {
            var user = await dbSet.FirstOrDefaultAsync(user => user.Id == id);

            if (user is null)
            {
                _logger.LogDebug("{type} not found with Auth Id {auth0Id}", nameof(User), id);
                return null;
            }

            return user.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching User '{type}'.", nameof(Guid));
            throw;
        }
    }

    public async Task<UserDomain?> GetByEmailAsync(string email)
    {
        var user = await dbSet.AsNoTracking().FirstOrDefaultAsync(user => user.Email == email);

        if (user is null)
        {
            _logger.LogDebug("{type} not found with email {email}", nameof(User), email);
            return null;
        }

        return entityDomainMapper.Map(user);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var exists = await dbSet.AsNoTracking().AnyAsync(user => user.Email == email);

        if (!exists)
            _logger.LogDebug("{type} not found with email {email}", nameof(User), email);

        return exists;
    }

    public override async Task<bool> ExistsAsync(string id)
    {
        return await dbSet.AsNoTracking().AnyAsync(user => user.Id == id);
    }
}
