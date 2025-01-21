using FinanceManager.Data.Repository;

namespace FinanceManager.Application.Services
{
    internal class BaseService : IBaseService
    {
        private readonly IUserRepository _userRepository;

        public BaseService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> UserExists(string userId)
        {
            // check if user exists
            return await _userRepository.ExistsAsync(userId);
        }
    }
}
