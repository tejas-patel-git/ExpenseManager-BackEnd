using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Data.Repository;

/// <summary>
/// Repository for managing User entities.
/// </summary>
internal class UserRepository : Repository<UserDomain, User, Guid>, IUserRepository
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

    public override async Task AddAsync(UserDomain domain)
    {
        try
        {
            await AddAsync(domain, entity => entity.Id = Guid.NewGuid());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding a '{type}'.", typeof(User).Name);
            throw;
        }
    }

    public async Task<UserDomain?> GetByEmailAsync(string email)
    {
        var user = await dbSet.AsNoTracking().FirstOrDefaultAsync(user => user.Email == email);

        if (user is default(User)) {
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
}
