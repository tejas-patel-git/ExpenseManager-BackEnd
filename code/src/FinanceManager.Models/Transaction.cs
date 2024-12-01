namespace FinanceManager.Models
{
    public class Transaction
    {
        public int TransactionID { get; set; } // Unique identifier for the transaction
        public int UserID { get; set; } // Links to the user
        public int CategoryID { get; set; } // Links to the expense category
        public string Type { get; set; } // Type of transaction (Expense, Loan, Borrow, Investment)
        public int? ReferenceID { get; set; } // Links to LoanID, BorrowID, or InvestmentID as applicable
        public decimal Amount { get; set; } // Amount of the transaction
        public DateTime Date { get; set; } // Transaction date
        public string Description { get; set; } // Optional description of the transaction
        public DateTime UpdatedAt { get; set; } // Timestamp of the last update
        public DateTime CreateAt { get; set; } // Timestamp of the creation
    }
}