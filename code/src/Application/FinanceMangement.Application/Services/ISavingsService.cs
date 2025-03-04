using FinanceManager.Domain.Models;

namespace FinanceManager.Application.Services
{
    public interface ISavingsService : IBaseService
    {
        Task<SavingsGoalDomain> AddSavingsAsync(SavingsGoalDomain savingsDomain);
        Task<bool> DeleteSavingsAsync(Guid id, string userId);
        Task<bool> Exists(Guid id);
        Task<SavingsGoalDomain> GetUserSavingsAsync(Guid value, string userId);
        Task<IEnumerable<SavingsGoalDomain>> GetUserSavingsAsync(string userId);
        Task UpdateSavingsAsync(SavingsGoalDomain savingsDomain);
    }
}
