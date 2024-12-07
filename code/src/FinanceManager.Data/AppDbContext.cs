using FinanceManager.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Data
{
    /// <summary>
    /// Represents the application's database context, enabling interaction with the database.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to configure the database context.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Gets or sets the table for managing transactions.
        /// </summary>
        /// <value>A set of <see cref="Transaction"/> entities in the database.</value>
        internal DbSet<Transaction> Transactions { get; set; }
    }
}
