using FinanceManager.Domain.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManager.Data.Models
{
    public class UserBankAccounts : IEntityModel<Guid>, IAuditableEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        [ForeignKey("User")]
        public string UserId { get; set; }
        
        public string? AccountNumber { get; set; }
        
        public string AccountName { get; set; }

        public string BankName { get; set; }
        
        public string AccountType { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Balance { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
}
