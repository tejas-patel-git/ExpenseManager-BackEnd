using System.ComponentModel.DataAnnotations;

namespace FinanceManager.Data.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        
        [Required]
        [MaxLength(15)]
        public string Phone { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }
        
        public ICollection<Transaction> Transactions { get; set; }
    }
}