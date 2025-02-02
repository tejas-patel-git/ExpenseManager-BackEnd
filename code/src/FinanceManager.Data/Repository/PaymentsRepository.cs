using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using Microsoft.Extensions.Logging;

namespace FinanceManager.Data.Repository
{
    internal class PaymentsRepository : Repository<PaymentDomain, TransactionPayment, Guid>, IPaymentRepository
    {
        private readonly ILogger<Repository<PaymentDomain, TransactionPayment, Guid>> _logger;
        private readonly IMapper<PaymentDomain, TransactionPayment> _domainEntityMapper;

        public PaymentsRepository(AppDbContext context,
                                    ILogger<PaymentsRepository> logger,
                                    IMapper<PaymentDomain, TransactionPayment> domainEntityMapper,
                                    IMapper<TransactionPayment, PaymentDomain> entityDomainMapper) : base(context, logger, domainEntityMapper, entityDomainMapper)
        {
            _logger = logger;
            _domainEntityMapper = domainEntityMapper;
        }

        public async Task RemovePayment(TransactionPayment transactionPayment)
        {
            ArgumentNullException.ThrowIfNull(transactionPayment, nameof(transactionPayment));

            dbSet.Remove(transactionPayment);
        }

        public async Task RemovePayment(IEnumerable<TransactionPayment> transactionPayments)
        {
            ArgumentNullException.ThrowIfNull(transactionPayments, nameof(transactionPayments));

            dbSet.RemoveRange(transactionPayments);
        }

        public async Task UpsertPayment(Guid transactionId, IEnumerable<TransactionPayment> transactionPayments)
        {
            try
            {
                foreach (var payment in transactionPayments)
                {
                    await UpsertPayment(transactionId, payment);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating {count} '{entity}'.", transactionPayments.Count(), nameof(TransactionPayment));
                throw;
            }
        }

        public async Task UpsertPayment(Guid transactionId, IEnumerable<PaymentDomain> transactionPayments)
        {
            try
            {
                foreach (var payment in transactionPayments)
                {
                    await UpsertPayment(transactionId, _domainEntityMapper.Map(payment));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating {count} '{entity}'.", transactionPayments.Count(), nameof(PaymentDomain));
                throw;
            }
        }

        private async Task UpsertPayment(Guid transactionId, TransactionPayment payment)
        {
            var existingPayment = dbSet.FirstOrDefault(p => p.UserBankAccountId == payment.UserBankAccountId && p.TransactionId == transactionId);

            if (existingPayment != null)
            {
                existingPayment.Amount = payment.Amount;
            }
            else
            {
                await AddAsync(new TransactionPayment
                {
                    UserBankAccountId = payment.UserBankAccountId,
                    Amount = payment.Amount,
                    TransactionId = transactionId
                });
            }
        }
    }
}
