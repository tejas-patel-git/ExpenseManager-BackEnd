using FinanceManager.Domain.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManager.Data.Models
{
    public class SavingsTransaction : IEntityModel<Guid>
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Transaction))]
        public Guid TransactionId { get; set; }
        
        [ForeignKey(nameof(SavingsGoal))]
        public Guid SavingsGoalId { get; set; }

        // Navigation property for one to one relationship
        public Transaction Transaction { get; set; }
    }
}
