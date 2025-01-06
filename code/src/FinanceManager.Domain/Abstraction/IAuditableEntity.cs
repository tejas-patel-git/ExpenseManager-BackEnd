namespace FinanceManager.Domain.Abstraction
{
    public interface IAuditableEntity
    {
        /// <summary>
        /// Gets or sets the timestamp for when the entity was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the timestamp for when the entity was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
