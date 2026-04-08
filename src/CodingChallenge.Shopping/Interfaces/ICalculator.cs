using CodingChallenge.Shopping.Models;

namespace CodingChallenge.Shopping.Interfaces;

// Decouples callers from the concrete calculator implementation.
public interface ICalculator
{
    decimal Calculate(List<CartItem> cart, DateTime checkoutDate);
}
