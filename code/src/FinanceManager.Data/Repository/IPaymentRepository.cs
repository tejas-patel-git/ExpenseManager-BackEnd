using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Repository;
using FinanceManager.Domain.Models;

namespace FinanceManager.Data.Repository
{
    public interface IPaymentRepository : IRepository<PaymentDomain, TransactionPayment, Guid>
    {
        Task RemovePayment(IEnumerable<TransactionPayment> transactionPayment);
        Task RemovePayment(TransactionPayment transactionPayment);
        Task UpsertPayment(Guid transactionId, IEnumerable<PaymentDomain> transactionPayments);
    }
}