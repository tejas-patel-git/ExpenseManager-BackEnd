namespace FinanceManager.Models
{
    /// <summary>
    /// Represents a Data Transfer Object (DTO) for a financial transaction.
    /// </summary>
    public class TransactionResponse : BaseResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier for the transaction.
        /// </summary>
        public int TransactionID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the transaction is an expense or income.
        /// </summary>
        /// <value><c>true</c> if the transaction is an expense; <c>false</c>, if transaction is an income.</value>
        public bool IsExpense { get; set; }

        /// <summary>
        /// Gets or sets the monetary amount of the transaction.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the date when the transaction occurred.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets a description of the transaction.
        /// </summary>
        /// <remarks>This field provides additional details about the transaction.</remarks>
        public string? Description { get; set; }
    }
}