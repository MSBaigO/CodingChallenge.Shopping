using CodingChallenge.Shopping.Discounts;
using CodingChallenge.Shopping.Calculators;
using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Tests.Extensibility;

// 15% discount applied to all items; demonstrates Open/Closed Principle.
file class FirstResponderDiscountStrategy : IDiscountStrategy
{
    private const decimal DiscountRate = 0.15m;

    public bool AppliesTo(CartItem item, DateTime checkoutDate) => true;

    public decimal CalculatePrice(CartItem item, DateTime checkoutDate) =>
        item.BaseCost * (1m - DiscountRate);
}

public class FirstResponderDiscountTests
{
    [Fact]
    public void FirstResponderStrategy_ShouldApply15PercentDiscount_WithoutModifyingCalculator()
    {
        var strategies = new IDiscountStrategy[]
        {
            new FirstResponderDiscountStrategy(),
            new DefaultNoDiscountStrategy(),
        };

        // Strategy-injection constructor is only on GroceryStoreCheckoutCalculator, not ICalculator.
        var calculator = new GroceryStoreCheckoutCalculator(strategies);

        var cart = new List<CartItem>
        {
            new() { ProductName = "Bread", Category = Category.Food, Price = 4.00m, Quantity = 1 },
            new() { ProductName = "Ornament", Category = Category.Christmas, Price = 10.00m, Quantity = 2 },
        };

        var total = calculator.Calculate(cart, new DateTime(2020, 11, 30));

        // Bread: 4.00 * 0.85 = 3.40
        // Ornament: 10.00 * 2 * 0.85 = 17.00
        Assert.Equal(3.40m + 17.00m, total);
    }

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

        // Strategy-injection constructor is only on GroceryStoreCheckoutCalculator, not ICalculator.
        var calculator = new GroceryStoreCheckoutCalculator(strategies);

        var cart = new List<CartItem>
        {
            // Christmas item on Dec 5 -> 20% off
            new() { ProductName = "Tree", Category = Category.Christmas, Price = 100m, Quantity = 1 },
            // Uncategorized item -> First Responder 15% off
            new() { ProductName = "Radio", Category = Category.Uncategorized, Price = 50m, Quantity = 1 },
        };

        var total = calculator.Calculate(cart, new DateTime(2020, 12, 5));

        // Tree: 100 * 0.80 = 80
        // Radio: 50 * 0.85 = 42.50
        Assert.Equal(80m + 42.50m, total);
    }
}
