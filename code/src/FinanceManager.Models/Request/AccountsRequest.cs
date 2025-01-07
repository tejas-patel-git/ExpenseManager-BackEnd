using FinanceManager.Domain.Enums;

namespace FinanceManager.Models.Request
{
    public class AccountsRequest
    {
        public string AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public BankName BankName { get; set; }
        public AccountType AccountType { get; set; }
        public decimal Balance { get; set; } = 0;
    }
}
