using FinanceManager.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Data.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync(int userID)
        {
            return await _context.Transactions.Where(transaction => transaction.UserID == userID)
                                              .ToListAsync();
        }
    }
}
