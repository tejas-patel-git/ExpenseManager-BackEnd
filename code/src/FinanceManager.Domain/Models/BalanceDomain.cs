namespace FinanceManager.Domain.Models
{
    public class BalanceDomain
    {
        public decimal TotalBalance { get; set; }
        public decimal TransactionBalance { get; set; }
        public IDictionary<string, decimal> AccountsBalance { get; set; } = new Dictionary<string, decimal>();
        public IEnumerable<SavingsBalanceDomain> SavingsBalance { get; set; } = [];
    }

    public class SavingsBalanceDomain
    {
        public string Goal { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}