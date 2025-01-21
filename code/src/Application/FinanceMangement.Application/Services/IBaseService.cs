
namespace FinanceManager.Application.Services
{
    public interface IBaseService
    {
        Task<bool> UserExists(string userId);
    }
}