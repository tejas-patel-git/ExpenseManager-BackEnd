using FinanceManager.Domain.Abstraction;

namespace FinanceManager.Domain.Models
{
    public class SavingsTransactionDomain : IDomainModel<Guid>
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public Guid SavingsGoalId { get; set; }
    }
}
