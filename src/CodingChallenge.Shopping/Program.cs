using CodingChallenge.Shopping.Calculators;
using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

var christmasCart = new List<CartItem>
{
    new() { ProductName = "Lights", Category = Category.Christmas, Price = 5.99m, Quantity = 10 },
    new() { ProductName = "Tree", Category = Category.Christmas, Price = 169m, Quantity = 1 },
    new() { ProductName = "Ornaments", Category = Category.Christmas, Price = 8m, Quantity = 15 },
};

var foodCart = new List<CartItem>
{
    new() { ProductName = "Apple", Category = Category.Food, Price = 3.27m, Weight = 0.79m },
    new() { ProductName = "Scallop", Category = Category.Food, Price = 18m, Weight = 1.5m },
    new() { ProductName = "Salad", Category = Category.Food, Price = 6.99m, Quantity = 1 },
    new() { ProductName = "Ground Beef", Category = Category.Food, Price = 7.99m, Weight = 1.5m },
    new() { ProductName = "Red Wine", Category = Category.Food, Price = 25.99m, Quantity = 1 },
};

// Declared as ICalculator so this entry point depends on the abstraction, not the
// concrete pipeline. Swapping to a different implementation (e.g. tax-aware) only
// requires changing this one line.
ICalculator calculator = new GroceryStoreCheckoutCalculator();

Console.WriteLine("=== Christmas Shopping ===");
Console.WriteLine($"Nov 30 (full price):        {calculator.Calculate(christmasCart, new DateTime(2020, 11, 30))}");
Console.WriteLine($"Dec 30 (post-Christmas):    {calculator.Calculate(christmasCart, new DateTime(2020, 12, 30))}");

Console.WriteLine();
Console.WriteLine("=== Food Shopping ===");
Console.WriteLine($"Regular checkout:           {calculator.Calculate(foodCart, new DateTime(2020, 11, 30))}");
Console.WriteLine($"Senior hour (7:11 AM):      {calculator.Calculate(foodCart, new DateTime(2020, 11, 30, 7, 11, 0))}");
