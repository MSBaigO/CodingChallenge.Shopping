using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Interfaces;

// Defines how discounts are checked and applied.
public interface IDiscountStrategy
{
    // Returns true if this discount applies to the item.
    bool AppliesTo(CartItem item, DateTime checkoutDate);

    // Calculates the price when the discount applies.
    decimal CalculatePrice(CartItem item, DateTime checkoutDate);
}