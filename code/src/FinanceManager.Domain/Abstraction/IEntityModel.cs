namespace FinanceManager.Domain.Abstraction
{
    public interface IEntityModel<TId>
    {
        public TId Id { get; set; }
    }
}
