using CodingChallenge.Shopping.Discounts;
using CodingChallenge.Shopping.Calculators;
using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Tests.Extensibility;

/// <summary>
/// Extended post-Christmas clearance: 95% off Christmas items Jan 1–15.
/// Declared file-scoped so it lives only in the test project.
/// </summary>
file class ExtendedChristmasDiscountStrategy : IDiscountStrategy
{
    private const decimal DiscountRate = 0.95m;

    public bool AppliesTo(CartItem item, DateTime checkoutDate) =>
        item.Product.Category == Category.Christmas && checkoutDate.Month == 1 && checkoutDate.Day <= 15;

    public decimal CalculatePrice(CartItem item, DateTime checkoutDate) =>
        item.BaseCost * (1m - DiscountRate);
}

public class ExtendedChristmasDiscountTests
{
    /// <summary>Verifies that ExtendedChristmasDiscountStrategy applies 95% off Christmas items
    /// Jan 1–15 and does not apply after the 15th.</summary>
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
            new()
            {
                Product = new Product { Name = "Ornament", Category = Category.Christmas, Price = 20.00m },
                Amount = PurchaseAmount.ForQuantity(1)
            },
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
