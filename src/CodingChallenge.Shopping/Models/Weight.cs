using CodingChallenge.Shopping.Enums;

namespace CodingChallenge.Shopping.Models;

/// <summary>
/// Represents a measured weight with a numeric amount and a unit of measurement.
/// </summary>
public class Weight
{
    /// <summary>Gets the weight amount. Must be zero or greater.</summary>
    public decimal Amount { get; }

    /// <summary>Gets the unit of measurement for this weight.</summary>
    public WeightUnit Unit { get; }

    /// <summary>Amount must be zero or greater.</summary>
    public Weight(decimal amount, WeightUnit unit)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Weight amount cannot be negative.");

        Amount = amount;
        Unit = unit;
    }
}
