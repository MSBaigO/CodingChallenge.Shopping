using CodingChallenge.Shopping.Discounts;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Calculators;

// Prices a cart by delegating each item to the first matching IDiscountStrategy.
public class GroceryStoreCheckoutCalculator : ICalculator
{
    private readonly IReadOnlyList<IDiscountStrategy> _strategies;

    public GroceryStoreCheckoutCalculator(IEnumerable<IDiscountStrategy> strategies)
    {
        ArgumentNullException.ThrowIfNull(strategies);
        var list = strategies.ToList();
        if (list.Count == 0)
            throw new ArgumentException("At least one strategy is required. Include DefaultNoDiscountStrategy as a catch-all.", nameof(strategies));
        _strategies = list;
    }

    // Default order: Christmas → Senior → Default (first match wins).
    public GroceryStoreCheckoutCalculator()
        : this(
        [
            new ChristmasDiscountStrategy(),
            new SeniorDiscountStrategy(),
            new DefaultNoDiscountStrategy(),
        ])
    {
    }

    public decimal Calculate(List<CartItem> cart, DateTime checkoutDate)
    {
        if (cart is null || cart.Count == 0)
            return 0m;

        return cart.Sum(item =>
            _strategies.First(s => s.AppliesTo(item, checkoutDate))
                       .CalculatePrice(item, checkoutDate));
    }
}
