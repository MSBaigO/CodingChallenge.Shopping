using CodingChallenge.Shopping.Calculators;
using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Tests.BoundaryTests;

public class BoundaryAndEdgeCaseTests
{
    // ICalculator keeps the tests decoupled from the concrete pipeline,
    // making it straightforward to swap in a stub if pricing logic grows complex.
    private readonly ICalculator _calculator = new GroceryStoreCheckoutCalculator();

    /// <summary>Verifies that an empty cart produces a total of zero without throwing an exception.</summary>
    [Fact]
    public void EmptyCart_ShouldReturnZero()
    {
        var total = _calculator.Calculate(new List<CartItem>(), new DateTime(2020, 12, 5));

        Assert.Equal(0m, total);
    }

    /// <summary>Verifies that a null cart is treated as an empty order, returning zero instead of throwing.</summary>
    [Fact]
    public void NullCart_ShouldReturnZero()
    {
        var total = _calculator.Calculate(null!, new DateTime(2020, 12, 5));

        Assert.Equal(0m, total);
    }

    /// <summary>Verifies that an item with no recognised category falls through to the default strategy and is charged at full price.</summary>
    [Fact]
    public void UnknownCategory_ShouldChargeFullPrice()
    {
        var cart = new List<CartItem>
        {
            new()
            {
                Product = new Product { Name = "Widget", Category = Category.Uncategorized, Price = 49.99m },
                Amount = PurchaseAmount.ForQuantity(2)
            }
        };
        var date = new DateTime(2020, 11, 30);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(99.98m, total); // 49.99 * 2
    }

    /// <summary>Verifies that passing null as the strategy collection throws ArgumentNullException at construction time.</summary>
    [Fact]
    public void Constructor_NullStrategies_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new GroceryStoreCheckoutCalculator(null!));
    }

    /// <summary>Verifies that passing an empty strategy collection throws ArgumentException at construction time,
    /// rather than an unhelpful InvalidOperationException mid-checkout.</summary>
    [Fact]
    public void Constructor_EmptyStrategies_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new GroceryStoreCheckoutCalculator(Array.Empty<IDiscountStrategy>()));
    }
}
