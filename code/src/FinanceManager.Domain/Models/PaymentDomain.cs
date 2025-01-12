using FinanceManager.Domain.Abstraction;

namespace FinanceManager.Domain.Models
{
    public class PaymentDomain : IDomainModel<Guid>
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
