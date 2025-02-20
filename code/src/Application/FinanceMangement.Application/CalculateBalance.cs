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

        public async Task<BalanceDomain?> GetAccountBalance(string userId)
        {
            var accounts = await _unitOfWork.AccountsRepository.GetAllAsync(a => a.UserId == userId);

            if(accounts == null || !accounts.Any()) return null;

            var totalBalance = accounts.Sum(a => a.InitialBalance + a.CurrentBalance);
            var accountsBalance = accounts.ToDictionary(a => a.AccountName, a=> a.InitialBalance + a.CurrentBalance);

            return new() { AccountsBalance = accountsBalance, TotalBalance = totalBalance };
        }
    }
}
