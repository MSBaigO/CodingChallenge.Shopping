using CodingChallenge.Shopping.Calculators;
using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Tests.Extensibility;

// Adds a flat sales tax to whatever the inner calculator returns.
file class TaxAwareCalculator : ICalculator
{
    private readonly ICalculator _inner;
    private readonly decimal _taxRate;

    public TaxAwareCalculator(ICalculator inner, decimal taxRate)
    {
        _inner = inner;
        _taxRate = taxRate;
    }

    public decimal Calculate(List<CartItem> cart, DateTime checkoutDate) =>
        _inner.Calculate(cart, checkoutDate) * (1m + _taxRate);
}

// Rounds the final total to two decimal places using MidpointRounding.AwayFromZero.
file class RoundingCalculator : ICalculator
{
    private readonly ICalculator _inner;

    public RoundingCalculator(ICalculator inner) => _inner = inner;

    public decimal Calculate(List<CartItem> cart, DateTime checkoutDate) =>
        Math.Round(_inner.Calculate(cart, checkoutDate), 2, MidpointRounding.AwayFromZero);
}

public class CalculatorDecoratorTests
{
    [Fact]
    public void TaxAwareCalculator_ShouldAddTaxToInnerTotal()
    {
        ICalculator calculator = new TaxAwareCalculator(
            new GroceryStoreCheckoutCalculator(),
            taxRate: 0.10m);

        var cart = new List<CartItem>
        {
            new() { ProductName = "Widget", Category = Category.Uncategorized, Price = 100.00m, Quantity = 1 },
        };

        var total = calculator.Calculate(cart, new DateTime(2026, 3, 1));

        // 100.00 * 1.10 = 110.00
        Assert.Equal(110.00m, total);
    }

    [Fact]
    public void RoundingCalculator_ShouldRoundToTwoDecimalPlaces()
    {
        ICalculator calculator = new RoundingCalculator(
            new GroceryStoreCheckoutCalculator());

        // 3.27 * 0.79 = 2.5833 → rounds to 2.58
        var cart = new List<CartItem>
        {
            new() { ProductName = "Apple", Category = Category.Food, Price = 3.27m, Weight = 0.79m },
        };

        var total = calculator.Calculate(cart, new DateTime(2026, 3, 1));

        Assert.Equal(Math.Round(3.27m * 0.79m, 2, MidpointRounding.AwayFromZero), total);
    }

    [Fact]
    public void StackedDecorators_DiscountThenTaxThenRounding_ShouldComposeCorrectly()
    {
        ICalculator calculator =
            new RoundingCalculator(
                new TaxAwareCalculator(
                    new GroceryStoreCheckoutCalculator(),
                    taxRate: 0.0925m));

        var cart = new List<CartItem>
        {
            // Dec 30 → 90% off → 10.00 * 0.10 = 1.00
            new() { ProductName = "Ornament", Category = Category.Christmas, Price = 10.00m, Quantity = 1 },
        };

        var total = calculator.Calculate(cart, new DateTime(2026, 12, 30));

        // 1.00 * 1.0925 = 1.0925 → rounds to 1.09
        Assert.Equal(1.09m, total);
    }
}
