using CodingChallenge.Shopping.Enums;

namespace CodingChallenge.Shopping.Models;

/// <summary>Represents a purchasable product.</summary>
public class Product
{
    /// <summary>Gets or sets the product identifier (SKU or GUID).</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the product name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the product category.</summary>
    public Category Category { get; set; }

    private decimal _price;

    /// <summary>Gets or sets the unit price. Must be zero or greater.</summary>
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
}
