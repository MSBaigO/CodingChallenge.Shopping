using CodingChallenge.Shopping.Calculators;
using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Tests.Discounts;

public class SeniorDiscountTests
{
    // ICalculator keeps tests decoupled from the concrete pipeline;
    // a stub can be injected here without changing any test logic.
    private readonly ICalculator _calculator = new GroceryStoreCheckoutCalculator();

    /// <summary>Verifies that a by-weight food item purchased during senior hours (6:00–8:59 AM) receives a 10% discount
    /// calculated against the weight-based base cost (Weight * Price).</summary>
    [Fact]
    public void SeniorHour_ByWeight_ShouldApply10PercentDiscount()
    {
        var cart = new List<CartItem>
        {
            new()
            {
                Product = new Product { Name = "Scallop", Category = Category.Food, Price = 18m },
                Amount = PurchaseAmount.ForWeight(new Weight(1.5m, WeightUnit.Pound))
            }
        };
        var date = new DateTime(2020, 11, 30, 7, 30, 0);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(18m * 1.5m * 0.9m, total); // 24.30
    }

    /// <summary>Verifies that a by-unit food item purchased during senior hours (6:00–8:59 AM) receives a 10% discount
    /// calculated against the quantity-based base cost (Quantity * Price).</summary>
    [Fact]
    public void SeniorHour_ByUnit_ShouldApply10PercentDiscount()
    {
        var cart = new List<CartItem>
        {
            new()
            {
                Product = new Product { Name = "Salad", Category = Category.Food, Price = 6.99m },
                Amount = PurchaseAmount.ForQuantity(1)
            }
        };
        var date = new DateTime(2020, 11, 30, 7, 30, 0);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(6.99m * 0.9m, total); // 6.291
    }

    /// <summary>Verifies that exactly 6:00:00 AM is included — the window opens at 6 AM, so the 10% discount applies.</summary>
    [Fact]
    public void SeniorHourBoundary_Exactly6AM_ShouldApply10PercentDiscount()
    {
        var cart = new List<CartItem>
        {
            new()
            {
                Product = new Product { Name = "Salad", Category = Category.Food, Price = 100m },
                Amount = PurchaseAmount.ForQuantity(1)
            }
        };
        var date = new DateTime(2020, 11, 30, 6, 0, 0);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(90m, total); // 6:00 AM is inside the window
    }

    /// <summary>Verifies that 7:00:00 AM is the first minute inside the window and receives the 10% discount.</summary>
    [Fact]
    public void SeniorHourBoundary_Exactly7AM_ShouldApply10PercentDiscount()
    {
        var cart = new List<CartItem>
        {
            new()
            {
                Product = new Product { Name = "Salad", Category = Category.Food, Price = 100m },
                Amount = PurchaseAmount.ForQuantity(1)
            }
        };
        var date = new DateTime(2020, 11, 30, 7, 0, 0);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(90m, total); // first hour inside the window
    }

    /// <summary>Verifies that 8:00:00 AM is still inside the window (Hours == 8 passes the <= 8 check) and receives the discount.</summary>
    [Fact]
    public void SeniorHourBoundary_Exactly8AM_ShouldApply10PercentDiscount()
    {
        var cart = new List<CartItem>
        {
            new()
            {
                Product = new Product { Name = "Salad", Category = Category.Food, Price = 100m },
                Amount = PurchaseAmount.ForQuantity(1)
            }
        };
        var date = new DateTime(2020, 11, 30, 8, 0, 0);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(90m, total); // 8:xx AM is the last hour of the window
    }

    /// <summary>Verifies that 9:00:00 AM is outside the window (Hours == 9 fails the <= 8 check) and full price applies.</summary>
    [Fact]
    public void SeniorHourBoundary_Exactly9AM_ShouldChargeFullPrice()
    {
        var cart = new List<CartItem>
        {
            new()
            {
                Product = new Product { Name = "Salad", Category = Category.Food, Price = 100m },
                Amount = PurchaseAmount.ForQuantity(1)
            }
        };
        var date = new DateTime(2020, 11, 30, 9, 0, 0);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(100m, total); // 9:00 AM is outside the window
    }

    /// <summary>Verifies that 6:01 AM triggers the senior discount.
    /// TimeOfDay.Hours returns 6 for any minute within that hour, which passes the Hours >= 6 check.</summary>
    [Fact]
    public void SeniorHour_At0601_ShouldApplyDiscount()
    {
        var cart = new List<CartItem>
        {
            new()
            {
                Product = new Product { Name = "Apple", Category = Category.Food, Price = 3.00m },
                Amount = PurchaseAmount.ForQuantity(1)
            }
        };
        // 06:01 has Hours == 6; the rule requires Hours >= 6, so discount applies.
        var date = new DateTime(2020, 11, 30, 6, 1, 0);

        var total = _calculator.Calculate(cart, date);

        Assert.Equal(2.70m, total); // 10% off
    }
}
