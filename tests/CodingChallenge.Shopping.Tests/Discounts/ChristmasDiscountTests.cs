using CodingChallenge.Shopping.Calculators;
using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Tests.Discounts;

public class ChristmasDiscountTests
{
    private readonly ICalculator _calculator = new GroceryStoreCheckoutCalculator();

    private static CartItem ChristmasItem(decimal price = 10m, int quantity = 1) =>
        new() { ProductName = "Ornament", Category = Category.Christmas, Price = price, Quantity = quantity };

    [Fact]
    public void EarlyDecember_ShouldApply20PercentDiscount()
    {
        var cart = new List<CartItem> { ChristmasItem(100m, 1) };
        var date = new DateTime(2020, 12, 5);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(80m, total); // 100 * 0.80
    }

    [Fact]
    public void EarlyDecemberBoundary_Dec14_ShouldApply20PercentDiscount()
    {
        var cart = new List<CartItem> { ChristmasItem(100m, 1) };
        var date = new DateTime(2020, 12, 14);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(80m, total); // last day of early-season tier
    }

    [Fact]
    public void PeakDecemberBoundary_Dec15_ShouldApply60PercentDiscount()
    {
        var cart = new List<CartItem> { ChristmasItem(100m, 1) };
        var date = new DateTime(2020, 12, 15);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(40m, total); // first day of peak-season tier
    }

    [Fact]
    public void PeakDecember_ShouldApply60PercentDiscount()
    {
        var cart = new List<CartItem> { ChristmasItem(100m, 1) };
        var date = new DateTime(2020, 12, 20);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(40m, total); // 100 * 0.40
    }

    [Fact]
    public void PostChristmas_ShouldApply90PercentDiscount()
    {
        var cart = new List<CartItem> { ChristmasItem(100m, 1) };
        var date = new DateTime(2020, 12, 27);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(10m, total); // 100 * 0.10
    }

    [Fact]
    public void OffSeason_ShouldChargeFullPrice()
    {
        var cart = new List<CartItem> { ChristmasItem(100m, 1) };
        var date = new DateTime(2020, 11, 30);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(100m, total);
    }
}
