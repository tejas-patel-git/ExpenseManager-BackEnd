namespace FinanceManager.Models
{
    public class TransactionDto
    {
        public int TransactionID { get; set; }
        public bool IsExpense { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}