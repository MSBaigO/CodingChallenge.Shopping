using CodingChallenge.Shopping.Discounts;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Calculators;

// Uses the Strategy pattern; discount rules live in IDiscountStrategy implementations.
public class GroceryStoreCheckoutCalculator : ICalculator
{
    // IReadOnlyList prevents the injected collection from being mutated after construction.
    private readonly IReadOnlyList<IDiscountStrategy> _strategies;

    public GroceryStoreCheckoutCalculator(IEnumerable<IDiscountStrategy> strategies)
    {
        ArgumentNullException.ThrowIfNull(strategies);
        var list = strategies.ToList();
        if (list.Count == 0)
            throw new ArgumentException("At least one strategy is required. Include DefaultNoDiscountStrategy as a catch-all.", nameof(strategies));
        _strategies = list;
    }

    /// <summary>
    /// Creates a calculator with the default discount strategies.
    /// Strategy order matters: more-specific rules must precede <see cref="DefaultNoDiscountStrategy"/>.
    /// </summary>
    public GroceryStoreCheckoutCalculator()
        : this(
        [
            new ChristmasDiscountStrategy(),
            new SeniorDiscountStrategy(),
            // Fallback: always matches, guarantees every item gets a price.
            new DefaultNoDiscountStrategy(),
        ])
    {
    }

    public decimal Calculate(List<CartItem> cart, DateTime checkoutDate)
    {
        if (cart is null || cart.Count == 0)
            return 0m;

        if (cart.Any(item => item is null))
            throw new ArgumentException("Cart must not contain null items.", nameof(cart));

        // Walk strategies in order; apply the first one that claims the item.
        return cart.Sum(item =>
            _strategies.First(s => s.AppliesTo(item, checkoutDate))
                       .CalculatePrice(item, checkoutDate));
    }
}
