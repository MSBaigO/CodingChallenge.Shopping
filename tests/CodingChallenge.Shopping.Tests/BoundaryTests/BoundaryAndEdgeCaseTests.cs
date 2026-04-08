using CodingChallenge.Shopping.Calculators;
using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Tests.BoundaryTests;

public class BoundaryAndEdgeCaseTests
{
    private readonly ICalculator _calculator = new GroceryStoreCheckoutCalculator();

    [Fact]
    public void EmptyCart_ShouldReturnZero()
    {
        var total = _calculator.Calculate(new List<CartItem>(), new DateTime(2020, 12, 5));

        Assert.Equal(0m, total);
    }

    [Fact]
    public void NullCart_ShouldReturnZero()
    {
        var total = _calculator.Calculate(null!, new DateTime(2020, 12, 5));

        Assert.Equal(0m, total);
    }

    [Fact]
    public void UnknownCategory_ShouldChargeFullPrice()
    {
        var cart = new List<CartItem>
        {
            new() { ProductName = "Widget", Category = Category.Uncategorized, Price = 49.99m, Quantity = 2 }
        };
        var date = new DateTime(2020, 11, 30);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(99.98m, total); // 49.99 * 2
    }

    [Fact]
    public void CartWithNullItem_ShouldThrowArgumentException()
    {
        var cart = new List<CartItem> { null! };

        var ex = Assert.Throws<ArgumentException>(() =>
            _calculator.Calculate(cart, new DateTime(2020, 12, 5)));

        Assert.Contains("null", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_NullStrategies_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new GroceryStoreCheckoutCalculator(null!));
    }

    [Fact]
    public void Constructor_EmptyStrategies_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new GroceryStoreCheckoutCalculator(Array.Empty<IDiscountStrategy>()));
    }
}
