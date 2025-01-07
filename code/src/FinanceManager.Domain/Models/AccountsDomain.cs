using FinanceManager.Domain.Abstraction;
using FinanceManager.Domain.Enums;

namespace FinanceManager.Domain.Models
{
    public class AccountsDomain : IDomainModel<Guid>
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public BankName BankName { get; set; }
        public AccountType AccountType { get; set; }
        public decimal Balance { get; set; }
    }
}
