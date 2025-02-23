using FinanceManager.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FinanceManager.Models.Request;

/// <summary>
/// Represents a request to create or update a transaction.
/// </summary>
public class TransactionRequest : BaseRequest
{
    /// <summary>
    /// Gets or sets the amount for the transaction.
    /// </summary>
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the description of the transaction.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the date of the transaction.
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

    /// <summary>
    /// Indicates whether the transaction is an expense.
    /// If <c>true</c>, the transaction is an expense; otherwise, it's an income.
    /// </summary>
    [Required]
    public bool IsExpense { get; set; }

    [Required(ErrorMessage = "Transaction type is required.")]
    //[EnumDataType(typeof(TransactionType), ErrorMessage = "Invalid transaction type.")]
    [JsonConverter(typeof(JsonStringEnumConverter<TransactionType>))]
    public TransactionType Type { get; set; }

    [Required]
    public Payment Payments { get; set; }

    public string SavingGoal { get; set; }
}
