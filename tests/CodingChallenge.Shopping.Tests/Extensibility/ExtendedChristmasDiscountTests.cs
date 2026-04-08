using CodingChallenge.Shopping.Discounts;
using CodingChallenge.Shopping.Calculators;
using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Tests.Extensibility;

// 95% off Christmas items Jan 1–15.
file class ExtendedChristmasDiscountStrategy : IDiscountStrategy
{
    private const decimal DiscountRate = 0.95m;

    public bool AppliesTo(CartItem item, DateTime checkoutDate) =>
        item.Category == Category.Christmas && checkoutDate.Month == 1 && checkoutDate.Day <= 15;

    public decimal CalculatePrice(CartItem item, DateTime checkoutDate) =>
        item.BaseCost * (1m - DiscountRate);
}

public class ExtendedChristmasDiscountTests
{
    [Fact]
    public void ExtendedChristmasStrategy_ShouldApply95PercentDiscount_ThroughJanuary15()
    {
        var strategies = new IDiscountStrategy[]
        {
            new ExtendedChristmasDiscountStrategy(),
            new DefaultNoDiscountStrategy(),
        };

        var calculator = new GroceryStoreCheckoutCalculator(strategies);

        var cart = new List<CartItem>
        {
            new() { ProductName = "Ornament", Category = Category.Christmas, Price = 20.00m, Quantity = 1 },
        };

        // Jan 1 — discount applies
        var totalJan1 = calculator.Calculate(cart, new DateTime(2026, 1, 1));
        Assert.Equal(20.00m * 0.05m, totalJan1);

        // Jan 15 — discount still applies on the last eligible day
        var totalJan15 = calculator.Calculate(cart, new DateTime(2026, 1, 15));
        Assert.Equal(20.00m * 0.05m, totalJan15);

        // Jan 16 — discount no longer applies; falls through to DefaultNoDiscountStrategy
        var totalJan16 = calculator.Calculate(cart, new DateTime(2026, 1, 16));
        Assert.Equal(20.00m, totalJan16);
    }
}
