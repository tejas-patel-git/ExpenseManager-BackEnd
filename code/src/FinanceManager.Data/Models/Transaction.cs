using FinanceManager.Domain.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManager.Data.Models
{
    /// <summary>
    /// Represents a financial transaction in the system.
    /// </summary>
    public class Transaction : IEntityModel<Guid>, IAuditableEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the transaction.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the user associated with the transaction.
        /// </summary>
        [ForeignKey("User")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the transaction is an expense.
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, the transaction is an expense; otherwise, it is income.
        /// </remarks>
        public bool IsExpense { get; set; }
        public byte TransactionType {  get; set; }

        /// <summary>
        /// Gets or sets the monetary amount of the transaction.
        /// </summary>
        /// <remarks>
        /// Stored as a decimal with a precision of 10 and scale of 2.
        /// </remarks>
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the date when the transaction occurred.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets a description of the transaction.
        /// </summary>
        /// <remarks>
        /// Maximum length is 500 characters.
        /// </remarks>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the timestamp for when the transaction was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the timestamp for when the transaction was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the user associated with the transaction.
        /// </summary>
        /// <remarks>
        /// This establishes a navigation property to the <see cref="User"/> entity.
        /// </remarks>
        public virtual User User { get; set; }

        // Navigation property for many-to-many relationship
        public virtual ICollection<TransactionPayment> Payments { get; set; } = [];
    }
}
