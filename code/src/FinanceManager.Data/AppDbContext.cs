using FinanceManager.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        internal DbSet<Transaction> Transactions { get; set; }
    }
}
