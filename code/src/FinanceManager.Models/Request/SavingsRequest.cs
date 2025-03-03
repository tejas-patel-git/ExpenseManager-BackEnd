namespace FinanceManager.Models.Request
{
    public class SavingsRequest
    {
        public string Goal { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal InitialBalance { get; set; }
    }
}
