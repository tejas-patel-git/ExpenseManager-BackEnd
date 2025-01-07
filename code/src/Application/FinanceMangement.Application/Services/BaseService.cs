using FinanceManager.Data.Repository;

namespace FinanceManager.Application.Services
{
    internal class BaseService
    {
        private readonly IUserRepository _userRepository;

        public BaseService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        protected async Task<bool> DoesUserExists(string userId)
        {
            // check if user exists
            return await _userRepository.ExistsAsync(userId);
        }
    }
}
