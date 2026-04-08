using CodingChallenge.Shopping.Enums;

namespace CodingChallenge.Shopping.Models;

/// <summary>A cart line item pairing a product with how much of it is being purchased.</summary>
public class CartItem
{
    private Product _product = null!;
    private PurchaseAmount _amount = null!;

    /// <summary>Gets or sets the product being purchased. Must not be null.</summary>
    public Product Product
    {
        get => _product;
        set => _product = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>Gets or sets the purchase amount (by quantity or weight). Must not be null.</summary>
    public PurchaseAmount Amount
    {
        get => _amount;
        set => _amount = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>Base cost before discounts (Weight.Amount × Price or Quantity × Price).</summary>
    public decimal BaseCost => IsSoldByWeight
        ? Amount.Weight!.Amount * Product.Price
        : Amount.Quantity * Product.Price;

    /// <summary>True when the item is purchased by weight.</summary>
    public bool IsSoldByWeight => Amount.Type == PurchaseAmountType.Weight;
}