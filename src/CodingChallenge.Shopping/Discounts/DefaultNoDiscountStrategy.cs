using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Discounts;

// Fallback when no other strategy applies.
public class DefaultNoDiscountStrategy : IDiscountStrategy
{
    // Always matches, ensuring every item gets a price.
    public bool AppliesTo(CartItem item, DateTime checkoutDate) => true;

    public decimal CalculatePrice(CartItem item, DateTime checkoutDate) => item.BaseCost;
}
