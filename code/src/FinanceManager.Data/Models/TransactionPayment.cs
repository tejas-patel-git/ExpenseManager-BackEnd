using FinanceManager.Domain.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManager.Data.Models
{
    public class TransactionPayment : IEntityModel<Guid>
    {
        [Key]
        public Guid Id { get; set; }
        
        [ForeignKey(nameof(Transaction))]
        public Guid TransactionId { get; set; }

        [ForeignKey(nameof(UserBankAccount))]
        public Guid UserBankAccountId { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        public virtual Transaction Transaction { get; set; }

        public virtual UserBankAccounts UserBankAccount { get; set; }
    }
}
