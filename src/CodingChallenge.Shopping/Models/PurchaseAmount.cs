using CodingChallenge.Shopping.Enums;

namespace CodingChallenge.Shopping.Models;

/// <summary>Represents how much of a product is being purchased, either by quantity or weight.</summary>
public class PurchaseAmount
{
    /// <summary>Gets the type of purchase measurement.</summary>
    public PurchaseAmountType Type { get; }

    /// <summary>Gets the unit count when <see cref="Type"/> is <see cref="PurchaseAmountType.Quantity"/>; otherwise 0.</summary>
    public int Quantity { get; }

    /// <summary>Gets the weight when <see cref="Type"/> is <see cref="PurchaseAmountType.Weight"/>; otherwise null.</summary>
    public Weight? Weight { get; }

    private PurchaseAmount(PurchaseAmountType type, int quantity, Weight? weight)
    {
        Type = type;
        Quantity = quantity;
        Weight = weight;
    }

    /// <summary>Creates a quantity-based <see cref="PurchaseAmount"/>. Quantity must be zero or greater.</summary>
    public static PurchaseAmount ForQuantity(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative.");

        return new(PurchaseAmountType.Quantity, quantity, null);
    }

    /// <summary>Creates a weight-based <see cref="PurchaseAmount"/>. Weight must not be null.</summary>
    public static PurchaseAmount ForWeight(Weight weight)
    {
        ArgumentNullException.ThrowIfNull(weight);

        return new(PurchaseAmountType.Weight, 0, weight);
    }
}
