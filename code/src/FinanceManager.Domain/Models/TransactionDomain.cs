using FinanceManager.Domain.Abstraction;

namespace FinanceManager.Domain.Models;

public class TransactionDomain : IDomainModel<int>
{
    /// <summary>
    /// Gets or sets the unique identifier for the transaction.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the user associated with the transaction.
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the transaction is an expense.
    /// </summary>
    /// <remarks>
    /// If <c>true</c>, the transaction is an expense; otherwise, it is income.
    /// </remarks>
    public bool IsExpense { get; set; }

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
}
