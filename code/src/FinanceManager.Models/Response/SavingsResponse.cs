namespace FinanceManager.Models.Response
{
    public class SavingsResponse
    {
        public Guid Id { get; set; }
        public string Goal { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal InitialBalance { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}
