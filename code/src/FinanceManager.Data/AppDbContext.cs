﻿using FinanceManager.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
    }
}
