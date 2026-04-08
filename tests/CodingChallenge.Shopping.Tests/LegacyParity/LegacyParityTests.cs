using CodingChallenge.Shopping.Calculators;
using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Tests.LegacyParity;

public class LegacyParityTests
{
    private readonly ICalculator _calculator = new GroceryStoreCheckoutCalculator();

    [Fact]
    public void ChristmasCart_FullPrice_Nov30_MatchesLegacy()
    {
        var cart = new List<CartItem>
        {
            new() { ProductName = "Lights", Category = Category.Christmas, Price = 5.99m, Quantity = 10 },
            new() { ProductName = "Tree", Category = Category.Christmas, Price = 169m, Quantity = 1 },
            new() { ProductName = "Ornaments", Category = Category.Christmas, Price = 8m, Quantity = 15 },
        };

        var total = _calculator.Calculate(cart, new DateTime(2020, 11, 30));

        // 5.99*10 + 169*1 + 8*15 = 59.90 + 169 + 120 = 348.90
        Assert.Equal(348.90m, total);
    }

    [Fact]
    public void ChristmasCart_PostChristmas_Dec30_MatchesLegacy()
    {
        var cart = new List<CartItem>
        {
            new() { ProductName = "Lights", Category = Category.Christmas, Price = 5.99m, Quantity = 10 },
            new() { ProductName = "Tree", Category = Category.Christmas, Price = 169m, Quantity = 1 },
            new() { ProductName = "Ornaments", Category = Category.Christmas, Price = 8m, Quantity = 15 },
        };

        var total = _calculator.Calculate(cart, new DateTime(2020, 12, 30));

        // 348.90 * 0.10 (90% off post-Christmas)
        Assert.Equal(348.90m * 0.10m, total);
    }

    [Fact]
    public void FoodCart_RegularCheckout_MatchesLegacy()
    {
        var cart = new List<CartItem>
        {
            new() { ProductName = "Apple", Category = Category.Food, Price = 3.27m, Weight = 0.79m },
            new() { ProductName = "Scallop", Category = Category.Food, Price = 18m, Weight = 1.5m },
            new() { ProductName = "Salad", Category = Category.Food, Price = 6.99m, Quantity = 1 },
            new() { ProductName = "Ground Beef", Category = Category.Food, Price = 7.99m, Weight = 1.5m },
            new() { ProductName = "Red Wine", Category = Category.Food, Price = 25.99m, Quantity = 1 },
        };

        var total = _calculator.Calculate(cart, new DateTime(2020, 11, 30));

        var expected = 3.27m * 0.79m + 18m * 1.5m + 6.99m + 7.99m * 1.5m + 25.99m;
        Assert.Equal(expected, total);
    }

    [Fact]
    public void FoodCart_SeniorHour_MatchesLegacy()
    {
        var cart = new List<CartItem>
        {
            new() { ProductName = "Apple", Category = Category.Food, Price = 3.27m, Weight = 0.79m },
            new() { ProductName = "Scallop", Category = Category.Food, Price = 18m, Weight = 1.5m },
            new() { ProductName = "Salad", Category = Category.Food, Price = 6.99m, Quantity = 1 },
            new() { ProductName = "Ground Beef", Category = Category.Food, Price = 7.99m, Weight = 1.5m },
            new() { ProductName = "Red Wine", Category = Category.Food, Price = 25.99m, Quantity = 1 },
        };

        var total = _calculator.Calculate(cart, new DateTime(2020, 11, 30, 7, 11, 0));

        var expected = (3.27m * 0.79m + 18m * 1.5m + 6.99m + 7.99m * 1.5m + 25.99m) * 0.9m;
        Assert.Equal(expected, total);
    }
}
