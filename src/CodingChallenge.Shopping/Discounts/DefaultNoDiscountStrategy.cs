using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Discounts;

// Fallback when no discounts apply.
public class DefaultNoDiscountStrategy : IDiscountStrategy
{
    public bool AppliesTo(CartItem item, DateTime checkoutDate) => true;
    public decimal CalculatePrice(CartItem item, DateTime checkoutDate) => item.BaseCost;
}
