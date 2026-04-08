using CodingChallenge.Shopping.Enums;

namespace CodingChallenge.Shopping.Models;

public class CartItem
{
    public string ProductName { get; set; } = string.Empty;

    private decimal _price;
    /// <summary>
    /// Unit price or price per weight. Must be zero or greater.
    /// </summary>
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Price cannot be negative.");
            _price = value;
        }
    }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Quantity cannot be negative.");
            _quantity = value;
        }
    }

    public Category Category { get; set; }

    private decimal _weight;
    /// <summary>
    /// If > 0, item is sold by weight. Must be zero or greater.
    /// </summary>
    public decimal Weight
    {
        get => _weight;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Weight cannot be negative.");
            _weight = value;
        }
    }

    /// <summary>
    /// Base cost before discounts.
    /// </summary>
    public decimal BaseCost => IsSoldByWeight ? Weight * Price : Quantity * Price;

    /// <summary>
    /// True if the item is a Food item sold by weight (Weight > 0).
    /// </summary>
    public bool IsSoldByWeight => Category == Category.Food && Weight > 0;
}