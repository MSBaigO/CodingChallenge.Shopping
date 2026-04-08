using CodingChallenge.Shopping.Calculators;
using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Tests.Discounts;

public class ChristmasDiscountTests
{
    // ICalculator keeps tests decoupled from the concrete pipeline;
    // a stub can be injected here without changing any test logic.
    private readonly ICalculator _calculator = new GroceryStoreCheckoutCalculator();

    private static CartItem ChristmasItem(decimal price = 10m, int quantity = 1) =>
        new()
        {
            Product = new Product { Name = "Ornament", Category = Category.Christmas, Price = price },
            Amount = PurchaseAmount.ForQuantity(quantity)
        };

    /// <summary>Verifies that Dec 1–14 (early-season tier in the table) applies a 20% discount.</summary>
    [Fact]
    public void EarlyDecember_ShouldApply20PercentDiscount()
    {
        var cart = new List<CartItem> { ChristmasItem(100m, 1) };
        var date = new DateTime(2020, 12, 5);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(80m, total); // 100 * 0.80
    }

    /// <summary>Verifies that Dec 14 (last day of the early-season tier) still applies only 20%, not the peak rate.</summary>
    [Fact]
    public void EarlyDecemberBoundary_Dec14_ShouldApply20PercentDiscount()
    {
        var cart = new List<CartItem> { ChristmasItem(100m, 1) };
        var date = new DateTime(2020, 12, 14);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(80m, total); // last day of early-season tier
    }

    /// <summary>Verifies that Dec 15 (first day of the peak-season tier) applies the 60% discount.</summary>
    [Fact]
    public void PeakDecemberBoundary_Dec15_ShouldApply60PercentDiscount()
    {
        var cart = new List<CartItem> { ChristmasItem(100m, 1) };
        var date = new DateTime(2020, 12, 15);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(40m, total); // first day of peak-season tier
    }

    /// <summary>Verifies that Dec 15–25 (peak-season tier in the table) applies a 60% discount.</summary>
    [Fact]
    public void PeakDecember_ShouldApply60PercentDiscount()
    {
        var cart = new List<CartItem> { ChristmasItem(100m, 1) };
        var date = new DateTime(2020, 12, 20);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(40m, total); // 100 * 0.40
    }

    /// <summary>Verifies that Dec 26–31 (post-Christmas clearance tier in the table) applies a 90% discount.</summary>
    [Fact]
    public void PostChristmas_ShouldApply90PercentDiscount()
    {
        var cart = new List<CartItem> { ChristmasItem(100m, 1) };
        var date = new DateTime(2020, 12, 27);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(10m, total); // 100 * 0.10
    }

    /// <summary>Verifies that a Christmas item purchased outside of December is charged at full price — the tier table is never consulted.</summary>
    [Fact]
    public void OffSeason_ShouldChargeFullPrice()
    {
        var cart = new List<CartItem> { ChristmasItem(100m, 1) };
        var date = new DateTime(2020, 11, 30);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(100m, total);
    }
}
