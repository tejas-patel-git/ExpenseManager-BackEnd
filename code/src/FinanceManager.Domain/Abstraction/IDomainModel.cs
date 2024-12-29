namespace FinanceManager.Domain.Abstraction
{
    public interface IDomainModel<TId>
    {
        public TId Id { get; set; }
    }
}
