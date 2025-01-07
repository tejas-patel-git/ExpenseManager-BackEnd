using FinanceManager.Data;
using FinanceManager.Domain.Models;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Application.Services
{
    internal class AccountsService : BaseService, IAccountsService
    {
        private readonly ILogger<AccountsService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public AccountsService(ILogger<AccountsService> logger,
                               IUnitOfWork unitOfWork)
            : base(unitOfWork.UserRepository)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddAccount(AccountsDomain accountsDomain)
        {
            ArgumentNullException.ThrowIfNull(accountsDomain, nameof(accountsDomain));

            if(!await DoesUserExists(accountsDomain.UserId))
                return false;
 
            accountsDomain.Id = Guid.NewGuid();
            
            await _unitOfWork.AccountsRepository.AddAsync(accountsDomain);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
