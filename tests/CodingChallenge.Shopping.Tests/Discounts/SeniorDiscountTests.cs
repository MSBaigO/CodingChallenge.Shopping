using CodingChallenge.Shopping.Calculators;
using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Tests.Discounts;

public class SeniorDiscountTests
{
    private readonly ICalculator _calculator = new GroceryStoreCheckoutCalculator();

    [Fact]
    public void SeniorHour_ByWeight_ShouldApply10PercentDiscount()
    {
        var cart = new List<CartItem>
        {
            new() { ProductName = "Scallop", Category = Category.Food, Price = 18m, Weight = 1.5m }
        };
        var date = new DateTime(2020, 11, 30, 7, 30, 0);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(18m * 1.5m * 0.9m, total); // 24.30
    }

    [Fact]
    public void SeniorHour_ByUnit_ShouldApply10PercentDiscount()
    {
        var cart = new List<CartItem>
        {
            new() { ProductName = "Salad", Category = Category.Food, Price = 6.99m, Quantity = 1 }
        };
        var date = new DateTime(2020, 11, 30, 7, 30, 0);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(6.99m * 0.9m, total); // 6.291
    }

    [Fact]
    public void SeniorHourBoundary_Exactly6AM_ShouldChargeFullPrice()
    {
        var cart = new List<CartItem>
        {
            new() { ProductName = "Salad", Category = Category.Food, Price = 100m, Quantity = 1 }
        };
        var date = new DateTime(2020, 11, 30, 6, 0, 0);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(100m, total); // 6:00 AM is outside the window; discount starts at 7:00 AM
    }

    [Fact]
    public void SeniorHourBoundary_Exactly7AM_ShouldApply10PercentDiscount()
    {
        var cart = new List<CartItem>
        {
            new() { ProductName = "Salad", Category = Category.Food, Price = 100m, Quantity = 1 }
        };
        var date = new DateTime(2020, 11, 30, 7, 0, 0);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(90m, total); // 7:00 AM is the first hour of the window
    }

    [Fact]
    public void SeniorHourBoundary_Exactly8AM_ShouldApply10PercentDiscount()
    {
        var cart = new List<CartItem>
        {
            new() { ProductName = "Salad", Category = Category.Food, Price = 100m, Quantity = 1 }
        };
        var date = new DateTime(2020, 11, 30, 8, 0, 0);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(90m, total); // 8:xx AM is the last hour of the window
    }

    [Fact]
    public void SeniorHourBoundary_Exactly9AM_ShouldChargeFullPrice()
    {
        var cart = new List<CartItem>
        {
            new() { ProductName = "Salad", Category = Category.Food, Price = 100m, Quantity = 1 }
        };
        var date = new DateTime(2020, 11, 30, 9, 0, 0);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(100m, total); // 9:00 AM is outside the window
    }

}
