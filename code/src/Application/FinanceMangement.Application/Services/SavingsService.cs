using FinanceManager.Data;
using FinanceManager.Domain.Models;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Application.Services
{
    internal class SavingsService : BaseService, ISavingsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SavingsService> _logger;

        public SavingsService(
            IUnitOfWork unitOfWork,
            ILogger<SavingsService> logger) : base(unitOfWork.UserRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<SavingsGoalDomain?> GetUserSavingsAsync(Guid savingsId, string userId)
        {
            if (savingsId.Equals(Guid.Empty))
            {
                throw new ArgumentException("Invalid savings goal id.", nameof(savingsId));
            }
            ArgumentNullException.ThrowIfNullOrEmpty(userId);

            var savings = await _unitOfWork.SavingsGoalRepository.GetByIdAsync(savingsId);

            if (savings == null || savings.UserId != userId)
                return null;

            return savings;
        }

        public async Task<IEnumerable<SavingsGoalDomain>> GetUserSavingsAsync(string userId)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(userId);

            var savingss = await _unitOfWork.SavingsGoalRepository.GetAllAsync(
                entity => entity.UserId == userId);

            return savingss.Any() ? savingss : [];
        }

        public async Task<SavingsGoalDomain> AddSavingsAsync(SavingsGoalDomain savingsDomain)
        {
            ArgumentNullException.ThrowIfNull(savingsDomain);

            // Assign new GUID for the savings goal
            savingsDomain.Id = Guid.NewGuid();

            // Check for duplicate goal name for the user
            var existing = await _unitOfWork.SavingsGoalRepository.GetByIdAsync(
                sg => sg.UserId == savingsDomain.UserId && sg.Goal == savingsDomain.Goal);

            if (existing != null)
            {
                throw new InvalidOperationException("A savings goal with this name already exists for the user.");
            }

            var created = await _unitOfWork.SavingsGoalRepository.AddAsync(savingsDomain);
            var rowsUpdated = await _unitOfWork.SaveChangesAsync();

            _logger.LogDebug("Added new savings goal with ID {SavingsId}. {RowsUpdated} rows updated",
                created.Id, rowsUpdated);

            return created;
        }

        public async Task UpdateSavingsAsync(SavingsGoalDomain savingsDomain)
        {
            ArgumentNullException.ThrowIfNull(savingsDomain);

            if (savingsDomain.Id.Equals(Guid.Empty)) throw new ArgumentException("Invalid savings goal id.", nameof(savingsDomain.Id));

            var existing = await _unitOfWork.SavingsGoalRepository.GetByIdAsync(sg => sg.UserId == savingsDomain.UserId
                    && sg.Id == savingsDomain.Id)
                ?? throw new KeyNotFoundException("Savings goal not found.");

            // Check if goal name is being changed and if it would create a duplicate
            if (savingsDomain.Goal != existing.Goal)
            {
                var duplicate = await _unitOfWork.SavingsGoalRepository.GetByIdAsync(
                    sg => sg.UserId == savingsDomain.UserId
                    && sg.Goal == savingsDomain.Goal);

                if (duplicate != null)
                {
                    throw new InvalidOperationException("Another savings goal with this name already exists for the user.");
                }
            }

            await _unitOfWork.SavingsGoalRepository.UpdateAsync(savingsDomain);
            var rowsUpdated = await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Updated savings goal with ID {SavingsId}. {RowsUpdated} rows updated",
                savingsDomain.Id, rowsUpdated);
        }

        public async Task<bool> DeleteSavingsAsync(Guid savingsId, string userId)
        {
            if (savingsId.Equals(Guid.Empty)) throw new ArgumentException("Invalid savings goal id.", nameof(savingsId));
            ArgumentNullException.ThrowIfNullOrEmpty(userId);

            var isSuccess = await _unitOfWork.SavingsGoalRepository.DeleteByIdAsync(savingsId);
            if (isSuccess)
            {
                await _unitOfWork.SaveChangesAsync();
                _logger.LogDebug("Deleted savings goal with ID {SavingsId}", savingsId);
            }

            return isSuccess;
        }

        public async Task<bool> Exists(Guid savingsId)
        {
            if (savingsId.Equals(Guid.Empty))
                return false;

            return await _unitOfWork.SavingsGoalRepository.ExistsAsync(savingsId);
        }
    }
}
