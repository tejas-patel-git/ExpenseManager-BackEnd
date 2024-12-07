using System.ComponentModel.DataAnnotations;

namespace FinanceManager.Data.Models
{
    /// <summary>
    /// Represents a user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [Key]
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        /// <remarks>
        /// The maximum length is 50 characters.
        /// </remarks>
        [Required]
        [MaxLength(50)]
        public required string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        /// <remarks>
        /// The maximum length is 50 characters.
        /// </remarks>
        [Required]
        [MaxLength(50)]
        public required string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        /// <remarks>
        /// The maximum length is 100 characters.
        /// </remarks>
        [Required]
        [MaxLength(100)]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        /// <remarks>
        /// The maximum length is 15 characters.
        /// </remarks>
        [Required]
        [MaxLength(15)]
        public required string Phone { get; set; }

        /// <summary>
        /// Gets or sets the hashed password for the user.
        /// </summary>
        [Required]
        public required string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the collection of transactions associated with the user.
        /// </summary>
        /// <remarks>
        /// This establishes a one-to-many relationship with the <see cref="Transaction"/> entity.
        /// </remarks>
        public virtual ICollection<Transaction>? Transactions { get; set; }
    }
}
