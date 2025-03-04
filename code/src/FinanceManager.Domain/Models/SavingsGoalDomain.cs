using FinanceManager.Domain.Abstraction;

namespace FinanceManager.Domain.Models
{
    public class SavingsGoalDomain : IDomainModel<Guid>
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public string Goal { get; set; }

        public decimal InitialBalance { get; set; }

        public decimal CurrentBalance { get; set; }
        public decimal TargetAmount { get; set; }
    }
}
