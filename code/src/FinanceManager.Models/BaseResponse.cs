namespace FinanceManager.Models;

/// <summary>
/// Represents a base response model.
/// </summary>
public abstract class BaseResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets an optional error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
