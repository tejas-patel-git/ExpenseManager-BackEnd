using FinanceManager.Data;
using FinanceManager.Domain.Models;

namespace FinanceManager.Application
{
    internal class CalculateBalance : ICalculateBalance
    {
        private readonly IUnitOfWork _unitOfWork;

        public CalculateBalance(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BalanceDomain?> GetTransactionalBalance(string userId)
        {
            var transactions = await _unitOfWork.TransactionRepository.GetAllAsync(t => t.UserId == userId);

            if (transactions == null || !transactions.Any()) return null;

            // get the balance after accounting transactions
            var balance = transactions.Sum(d => d.IsExpense ? -d.Amount : d.Amount);

            return new() { TransactionBalance = balance };
        }

        public async Task<BalanceDomain?> GetBalance(string userId)
        {
            BalanceDomain? balance = new();

            var accounts = await _unitOfWork.AccountsRepository.GetAllAsync(a => a.UserId == userId);
            if (accounts != null && accounts.Any())
            {
                balance.AccountsBalance = accounts.ToDictionary(a => a.AccountName, a => a.InitialBalance + a.CurrentBalance);
                balance.TotalBalance = accounts.Sum(a => a.InitialBalance + a.CurrentBalance);
            }

            var savings = await _unitOfWork.SavingsGoalRepository.GetAllAsync(s => s.UserId == userId);
            if (savings != null && savings.Any())
            {
                balance.SavingsBalance = savings.Select(s => new SavingsBalanceDomain()
                {
                    Goal = s.Goal,
                    CurrentBalance = s.CurrentBalance + s.InitialBalance,
                    TargetAmount = s.TargetAmount,
                });
            }

            return balance;
        }
    }
}
