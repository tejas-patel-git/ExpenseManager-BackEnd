using FinanceManager.Domain.Enums;

namespace FinanceManager.Models.Response
{
    public class AccountsResponse
    {
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string BankName { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
    }
}
