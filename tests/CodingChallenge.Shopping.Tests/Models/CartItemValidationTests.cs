using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Tests.Models;

public class CartItemValidationTests
{
    // ── Price ────────────────────────────────────────────────────────────────

    [Fact]
    public void Price_NegativeValue_ShouldThrowArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CartItem { Price = -0.01m });

        Assert.Contains("Price", ex.Message);
    }

    [Fact]
    public void Price_Zero_ShouldBeAllowed()
    {
        var item = new CartItem { Price = 0m };

        Assert.Equal(0m, item.Price);
    }

    [Fact]
    public void Price_PositiveValue_ShouldBeAllowed()
    {
        var item = new CartItem { Price = 9.99m };

        Assert.Equal(9.99m, item.Price);
    }

    // ── Quantity ─────────────────────────────────────────────────────────────

    [Fact]
    public void Quantity_NegativeValue_ShouldThrowArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CartItem { Quantity = -1 });

        Assert.Contains("Quantity", ex.Message);
    }

    [Fact]
    public void Quantity_Zero_ShouldBeAllowed()
    {
        var item = new CartItem { Quantity = 0 };

        Assert.Equal(0, item.Quantity);
    }

    [Fact]
    public void Quantity_PositiveValue_ShouldBeAllowed()
    {
        var item = new CartItem { Quantity = 5 };

        Assert.Equal(5, item.Quantity);
    }

    // ── Weight ───────────────────────────────────────────────────────────────

    [Fact]
    public void Weight_NegativeValue_ShouldThrowArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CartItem { Weight = -0.5m });

        Assert.Contains("Weight", ex.Message);
    }

    [Fact]
    public void Weight_Zero_ShouldBeAllowed()
    {
        // Zero weight is the sentinel meaning "sold by quantity, not weight".
        var item = new CartItem { Weight = 0m };

        Assert.Equal(0m, item.Weight);
        Assert.False(item.IsSoldByWeight);
    }

    [Fact]
    public void Weight_PositiveValue_ShouldBeAllowed()
    {
        var item = new CartItem { Category = Category.Food, Weight = 1.5m };

        Assert.Equal(1.5m, item.Weight);
        Assert.True(item.IsSoldByWeight);
    }

    // ── BaseCost with boundary values ────────────────────────────────────────

    [Fact]
    public void BaseCost_ZeroPriceAndZeroQuantity_ShouldBeZero()
    {
        var item = new CartItem { Category = Category.Food, Price = 0m, Quantity = 0 };

        Assert.Equal(0m, item.BaseCost);
    }

    [Fact]
    public void BaseCost_ZeroPriceWithWeight_ShouldBeZero()
    {
        var item = new CartItem { Category = Category.Food, Price = 0m, Weight = 2.0m };

        Assert.Equal(0m, item.BaseCost);
    }

    // ── IsSoldByWeight — category guard ──────────────────────────────────────

    [Fact]
    public void IsSoldByWeight_ChristmasItemWithWeight_ShouldBeFalse()
    {
        // Non-Food categories must never be priced by weight even if Weight is set.
        var item = new CartItem { Category = Category.Christmas, Price = 5m, Weight = 1m, Quantity = 2 };

        Assert.False(item.IsSoldByWeight);
        Assert.Equal(10m, item.BaseCost); // Quantity * Price
    }

    [Fact]
    public void IsSoldByWeight_FoodItemWithWeight_ShouldBeTrue()
    {
        var item = new CartItem { Category = Category.Food, Price = 3m, Weight = 1.5m };

        Assert.True(item.IsSoldByWeight);
        Assert.Equal(4.5m, item.BaseCost); // Weight * Price
    }
}
