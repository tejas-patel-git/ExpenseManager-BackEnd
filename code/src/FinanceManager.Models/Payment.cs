namespace FinanceManager.Models
{
    public class Payment
    {
        // TODO : Validate count of Accounts
        public ICollection<Accounts> Accounts { get; set; }
    }
}
