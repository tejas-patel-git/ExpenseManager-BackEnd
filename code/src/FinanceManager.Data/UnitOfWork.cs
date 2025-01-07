﻿using FinanceManager.Data.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace FinanceManager.Data;

/// <summary>
/// Implementation of the Unit of Work pattern.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction _transaction;
    private readonly IUserRepository _userRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountsRepository _accountsRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="transactionRepository"></param>
    /// <param name="userRepository"></param>
    /// <param name="accountsRepository"></param>
    public UnitOfWork(AppDbContext context,
                      ITransactionRepository transactionRepository,
                      IUserRepository userRepository,
                      IAccountsRepository accountsRepository)
    {
        _context = context;
        _transactionRepository = transactionRepository;
        _userRepository = userRepository;
        _accountsRepository = accountsRepository;
    }

    /// <inheritdoc/>
    public IUserRepository UserRepository => _userRepository;

    /// <inheritdoc/>
    public ITransactionRepository TransactionRepository => _transactionRepository;

    /// <inheritdoc/>
    public IAccountsRepository AccountsRepository => _accountsRepository;

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public void BeginTransaction()
    {
        _transaction = _context.Database.BeginTransaction();
    }

    /// <inheritdoc/>
    public void CommitTransaction()
    {
        _transaction?.Commit();
        _transaction?.Dispose();
    }

    /// <inheritdoc/>
    public void RollbackTransaction()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();

        GC.SuppressFinalize(this);
    }
}
