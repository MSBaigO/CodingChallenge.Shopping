using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Discounts;

// Seniors get a 10% discount on food items purchased during the early morning hours.
public class SeniorDiscountStrategy : IDiscountStrategy
{
    private const int SeniorHourStart = 6;
    private const int SeniorHourEnd = 8;
    private const decimal SeniorDiscountRate = 0.10m;

    public bool AppliesTo(CartItem item, DateTime checkoutDate)
    {
        int hour = checkoutDate.TimeOfDay.Hours;

        // Food only, 6–8 AM.
        return item.Product.Category == Category.Food
            && hour >= SeniorHourStart
            && hour <= SeniorHourEnd;
    }

    public decimal CalculatePrice(CartItem item, DateTime checkoutDate) =>
        item.BaseCost * (1m - SeniorDiscountRate);
}
