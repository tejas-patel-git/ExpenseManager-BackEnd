namespace FinanceManager.Domain.Models
{
    public class BalanceDomain
    {
        public decimal TotalBalance { get; set; }
        public decimal TransactionBalance { get; set; }
        public IDictionary<string, decimal> AccountsBalance { get; set; }
    }
}