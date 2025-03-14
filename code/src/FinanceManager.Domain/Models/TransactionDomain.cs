﻿using FinanceManager.Domain.Abstraction;
using FinanceManager.Domain.Enums;

namespace FinanceManager.Domain.Models;

public class TransactionDomain : IDomainModel<Guid>
{
    /// <summary>
    /// Gets or sets the unique identifier for the transaction.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the user associated with the transaction.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the transaction is an expense.
    /// </summary>
    /// <remarks>
    /// If <c>true</c>, the transaction is an expense; otherwise, it is income.
    /// </remarks>
    public bool IsExpense { get; set; }

    public TransactionType TransactionType { get; set; }

    /// <summary>
    /// Gets or sets the monetary amount of the transaction.
    /// </summary>
    /// <remarks>
    /// Stored as a decimal with a precision of 10 and scale of 2.
    /// </remarks>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the date when the transaction occurred.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets a description of the transaction.
    /// </summary>
    public string? Description { get; set; }

    public ICollection<PaymentDomain> Payments { get; set; } = [];
    public string SavingsGoal { get; set; }

    public bool IsAccountable()
    {
        return TransactionType != TransactionType.Savings;
    }

    public bool IsCredit()
    {
        return !IsExpense;
    }

    public bool IsDebit()
    {
        return IsExpense;
    }

    public decimal GetTransactionAmount()
    {
        return IsCredit() ? Amount : -Amount;
    }

    public bool IsSavingsType()
    {
        return TransactionType == TransactionType.Savings;
    }
}
