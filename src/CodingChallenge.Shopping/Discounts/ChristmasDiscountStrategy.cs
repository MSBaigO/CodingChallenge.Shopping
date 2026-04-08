using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Discounts;

// Tiered Christmas discounts throughout December.
public class ChristmasDiscountStrategy : IDiscountStrategy
{
    // Discount tiers by day of December (first match applies).
    private static readonly (int MaxDay, decimal Rate)[] DecemberTiers =
    [
        (14, 0.20m),  // Dec 1–14: 20% discount
        (25, 0.60m),  // Dec 15–25: 60% discount
        (31, 0.90m),  // Dec 26–31: 90% discount
    ];

    public bool AppliesTo(CartItem item, DateTime checkoutDate) =>
        item.Product.Category == Category.Christmas && checkoutDate.Month == 12;

    public decimal CalculatePrice(CartItem item, DateTime checkoutDate) =>
        item.BaseCost * (1m - GetDiscountRate(checkoutDate.Day));

    private static decimal GetDiscountRate(int day) =>
        DecemberTiers.First(t => day <= t.MaxDay).Rate;
}