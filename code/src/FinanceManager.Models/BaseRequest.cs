namespace FinanceManager.Models;

/// <summary>
/// Represents a base request model.
/// </summary>
public abstract class BaseRequest
{
    /// <summary>
    /// Gets or sets the unique request identifier.
    /// </summary>
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
}
