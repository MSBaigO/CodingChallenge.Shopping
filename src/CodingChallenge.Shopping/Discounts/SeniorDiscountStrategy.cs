using CodingChallenge.Shopping.Enums;
using CodingChallenge.Shopping.Interfaces;
using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Discounts;

// Seniors get a 10% discount on food items purchased during the early morning hours.
public class SeniorDiscountStrategy : IDiscountStrategy
{
    private const int SeniorHourStart = 7;
    private const int SeniorHourEnd = 9;
    private const decimal SeniorDiscountRate = 0.10m;

    public bool AppliesTo(CartItem item, DateTime checkoutDate)
    {
        int hour = checkoutDate.TimeOfDay.Hours;

        // Food items only, between 7:00 AM and 8:59 AM (hour strictly after 6).
        return item.Category == Category.Food
            && hour >= SeniorHourStart
            && hour < SeniorHourEnd;
    }

    public decimal CalculatePrice(CartItem item, DateTime checkoutDate) =>
        item.BaseCost * (1m - SeniorDiscountRate);
}
