using FinanceManager.Domain.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManager.Data.Models
{
    /// <summary>
    /// Represents a user.
    /// </summary>
    public class User : IEntityModel<string>, IAuditableEntity
    {
        [Key]
        [Required]
        [MaxLength(50)]
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; }

        [Phone]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public bool IsEmailVerified { get; set; }

        [MaxLength(50)]
        public string FamilyName { get; set; }

        [MaxLength(50)]
        public string GivenName { get; set; }

        public DateTime? LastPasswordReset { get; set; }

        [MaxLength(200)]
        public string FullName { get; set; }

        [MaxLength(100)]
        public string Nickname { get; set; }

        [Required]
        public bool IsPhoneVerified { get; set; }

        [Url]
        public string PictureUrl { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Column(TypeName = "NVARCHAR(MAX)")] // Use NVARCHAR(MAX) for JSON data
        public string AppMetadata { get; set; }

        [Column(TypeName = "NVARCHAR(MAX)")]
        public string UserMetadata { get; set; }

        public virtual IList<Transaction> Transactions { get; set; }
    }
}
