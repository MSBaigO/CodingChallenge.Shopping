using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Interfaces;

// Defines how a discount is evaluated and applied.
public interface IDiscountStrategy
{
    bool AppliesTo(CartItem item, DateTime checkoutDate);
    decimal CalculatePrice(CartItem item, DateTime checkoutDate);
}