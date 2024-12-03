using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManager.Data.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }
        
        [ForeignKey("User")]
        public int UserID { get; set; }
        
        public bool IsExpense { get; set; }
        
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }
        
        public DateTime Date { get; set; }
        
        [MaxLength(500)]
        public string Description { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
    }
}
