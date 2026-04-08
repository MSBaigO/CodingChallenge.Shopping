using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Tests.Models;

public class CartItemValidationTests
{
    // ── Product.Price ────────────────────────────────────────────────────────

    [Fact]
    public void Price_NegativeValue_ShouldThrowArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Product { Price = -0.01m });

        Assert.Contains("Price", ex.Message);
    }

    [Fact]
    public void Price_Zero_ShouldBeAllowed()
    {
        var product = new Product { Price = 0m };

        Assert.Equal(0m, product.Price);
    }

    [Fact]
    public void Price_PositiveValue_ShouldBeAllowed()
    {
        var product = new Product { Price = 9.99m };

        Assert.Equal(9.99m, product.Price);
    }

    // ── PurchaseAmount.ForQuantity ───────────────────────────────────────────

    [Fact]
    public void Quantity_NegativeValue_ShouldThrowArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            PurchaseAmount.ForQuantity(-1));

        Assert.Contains("quantity", ex.Message);
    }

    [Fact]
    public void Quantity_Zero_ShouldBeAllowed()
    {
        var amount = PurchaseAmount.ForQuantity(0);

        Assert.Equal(0, amount.Quantity);
    }

    [Fact]
    public void Quantity_PositiveValue_ShouldBeAllowed()
    {
        var amount = PurchaseAmount.ForQuantity(5);

        Assert.Equal(5, amount.Quantity);
    }

    // ── Weight constructor ───────────────────────────────────────────────────

    [Fact]
    public void Weight_NegativeAmount_ShouldThrowArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Weight(-0.5m, WeightUnit.Pound));

        Assert.Contains("amount", ex.Message);
    }

    [Fact]
    public void Weight_ZeroAmount_ShouldBeAllowed()
    {
        var weight = new Weight(0m, WeightUnit.Pound);

        Assert.Equal(0m, weight.Amount);
    }

    [Fact]
    public void Weight_PositiveAmount_ShouldBeAllowed()
    {
        var weight = new Weight(1.5m, WeightUnit.Pound);

        Assert.Equal(1.5m, weight.Amount);
    }

    // ── CartItem null guards ─────────────────────────────────────────────────

    [Fact]
    public void CartItem_NullProduct_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new CartItem { Product = null!, Amount = PurchaseAmount.ForQuantity(1) });
    }

    [Fact]
    public void CartItem_NullAmount_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new CartItem { Product = new Product { Price = 1m }, Amount = null! });
    }

    // ── BaseCost with boundary values ────────────────────────────────────────

    [Fact]
    public void BaseCost_ZeroPriceAndZeroQuantity_ShouldBeZero()
    {
        var item = new CartItem
        {
            Product = new Product { Category = Category.Food, Price = 0m },
            Amount = PurchaseAmount.ForQuantity(0)
        };

        Assert.Equal(0m, item.BaseCost);
    }

    [Fact]
    public void BaseCost_ZeroPriceWithWeight_ShouldBeZero()
    {
        var item = new CartItem
        {
            Product = new Product { Category = Category.Food, Price = 0m },
            Amount = PurchaseAmount.ForWeight(new Weight(2.0m, WeightUnit.Pound))
        };

        Assert.Equal(0m, item.BaseCost);
    }

    // ── IsSoldByWeight ───────────────────────────────────────────────────────

    [Fact]
    public void IsSoldByWeight_QuantityAmount_ShouldBeFalse()
    {
        // ForQuantity means the item is counted by units, never by weight.
        var item = new CartItem
        {
            Product = new Product { Category = Category.Christmas, Price = 5m },
            Amount = PurchaseAmount.ForQuantity(2)
        };

        Assert.False(item.IsSoldByWeight);
        Assert.Equal(10m, item.BaseCost); // Quantity * Price
    }

    [Fact]
    public void IsSoldByWeight_WeightAmount_ShouldBeTrue()
    {
        var item = new CartItem
        {
            Product = new Product { Category = Category.Food, Price = 3m },
            Amount = PurchaseAmount.ForWeight(new Weight(1.5m, WeightUnit.Pound))
        };

        Assert.True(item.IsSoldByWeight);
        Assert.Equal(4.5m, item.BaseCost); // Weight.Amount * Price
    }
}
