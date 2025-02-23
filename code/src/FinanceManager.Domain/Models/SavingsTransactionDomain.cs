namespace FinanceManager.Domain.Models
{
    public class SavingsTransactionDomain
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public Guid SavingsGoalId { get; set; }
    }
}
