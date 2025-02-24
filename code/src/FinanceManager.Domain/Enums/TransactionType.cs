using System.Text.Json.Serialization;

namespace FinanceManager.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType
    {
        Undefined = 0,
        Income,
        Expense,
        Savings,
        Borrow
    }
}
