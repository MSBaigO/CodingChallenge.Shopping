using CodingChallenge.Shopping.Discounts;
using CodingChallenge.Shopping.Calculators;
using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Tests.Extensibility;

/// <summary>
/// A new discount strategy added WITHOUT modifying GroceryStoreCheckoutCalculator,
/// proving the Open/Closed Principle holds.
/// </summary>
file class FirstResponderDiscountStrategy : IDiscountStrategy
{
    private const decimal DiscountRate = 0.15m;

    public bool AppliesTo(CartItem item, DateTime checkoutDate) => true;

    public decimal CalculatePrice(CartItem item, DateTime checkoutDate) =>
        item.BaseCost * (1m - DiscountRate);
}

public class FirstResponderDiscountTests
{
    /// <summary>Verifies that injecting a brand-new strategy applies its discount to all items
    /// without any changes to GroceryStoreCheckoutCalculator, demonstrating the Open/Closed Principle.</summary>
    [Fact]
    public void FirstResponderStrategy_ShouldApply15PercentDiscount_WithoutModifyingCalculator()
    {
        var strategies = new IDiscountStrategy[]
        {
            new FirstResponderDiscountStrategy(),
            new DefaultNoDiscountStrategy(),
        };

        // The concrete type is intentional here: the strategy-injection constructor is only
        // available on GroceryStoreCheckoutCalculator, not on ICalculator.
        var calculator = new GroceryStoreCheckoutCalculator(strategies);

        var cart = new List<CartItem>
        {
            new()
            {
                Product = new Product { Name = "Bread", Category = Category.Food, Price = 4.00m },
                Amount = PurchaseAmount.ForQuantity(1)
            },
            new()
            {
                Product = new Product { Name = "Ornament", Category = Category.Christmas, Price = 10.00m },
                Amount = PurchaseAmount.ForQuantity(2)
            },
        };

        var total = calculator.Calculate(cart, new DateTime(2020, 11, 30));

        // Bread: 4.00 * 0.85 = 3.40
        // Ornament: 10.00 * 2 * 0.85 = 17.00
        Assert.Equal(3.40m + 17.00m, total);
    }

    /// <summary>Verifies that a custom strategy can be inserted between built-in strategies so that
    /// items not matched by earlier rules are caught by the custom strategy rather than the default.</summary>
    [Fact]
    public void CustomStrategies_CanBeComposedWithBuiltIn()
    {
        var strategies = new IDiscountStrategy[]
        {
            new ChristmasDiscountStrategy(),
            new SeniorDiscountStrategy(),
            new FirstResponderDiscountStrategy(), // catch-all before default
            new DefaultNoDiscountStrategy(),
        };

        // The concrete type is intentional here: the strategy-injection constructor is only
        // available on GroceryStoreCheckoutCalculator, not on ICalculator.
        var calculator = new GroceryStoreCheckoutCalculator(strategies);

        var cart = new List<CartItem>
        {
            // Christmas item on Dec 5 -> 20% off
            new()
            {
                Product = new Product { Name = "Tree", Category = Category.Christmas, Price = 100m },
                Amount = PurchaseAmount.ForQuantity(1)
            },
            // Uncategorized item -> First Responder 15% off
            new()
            {
                Product = new Product { Name = "Radio", Category = Category.Uncategorized, Price = 50m },
                Amount = PurchaseAmount.ForQuantity(1)
            },
        };

        var total = calculator.Calculate(cart, new DateTime(2020, 12, 5));

        // Tree: 100 * 0.80 = 80
        // Radio: 50 * 0.85 = 42.50
        Assert.Equal(80m + 42.50m, total);
    }
}
