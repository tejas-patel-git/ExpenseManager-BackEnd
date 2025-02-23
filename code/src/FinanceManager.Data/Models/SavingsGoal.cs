using FinanceManager.Domain.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManager.Data.Models
{
    public class SavingsGoal : IEntityModel<Guid>, IAuditableEntity
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public string Goal { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal InitialBalance { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal CurrentBalance { get; set; }

        /// <summary>
        /// Gets or sets the timestamp for when the transaction was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the timestamp for when the transaction was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
