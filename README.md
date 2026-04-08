# NContracts Coding Challenge — Grocery Store Checkout

A clean, extensible grocery store checkout calculator built with the **Strategy** and **Chain-of-Responsibility** patterns.

---

## Table of Contents

- [Project Overview](#project-overview)
- [Folder & File Structure](#folder--file-structure)
- [Design Choices](#design-choices)
- [How to Run the Program](#how-to-run-the-program)
- [How to Run the Tests](#how-to-run-the-tests)
- [CI — GitHub Actions](#ci--github-actions)

---

## Project Overview

This solution implements a grocery store checkout calculator that supports multiple discount strategies (Christmas tiers, senior hour, extended post-Christmas clearance, and more) without any `if/else` chains or hard-coded strings. Adding a new discount type requires only a new file — no existing code changes needed.

---

## Folder & File Structure

```
CodingChallenge.Shopping.sln   # Solution file tying src and tests together

src/
└── CodingChallenge.Shopping/
    ├── CodingChallenge.Shopping.csproj   # Console app targeting net9.0
    ├── Program.cs                        # Entry point — demo scenarios
    │
    ├── Calculators/                      # ICalculator implementations
    │   └── GroceryStoreCheckoutCalculator.cs  # Core calculator — strategy pipeline
    │
    ├── Discounts/                        # One file per discount strategy
    │   ├── ChristmasDiscountStrategy.cs  # Data-driven tiered December discounts (20/60/90%)
    │   ├── SeniorDiscountStrategy.cs     # 10% off Food during 6:00–8:59 AM window
    │   └── DefaultNoDiscountStrategy.cs  # Catch-all — full price, always matches
    │
    ├── Enums/
    │   ├── Category.cs                   # Uncategorized | Christmas | Food
    │   ├── PurchaseAmountType.cs         # Quantity | Weight
    │   └── WeightUnit.cs                 # Pound | Ounce | Kilogram | Gram
    │
    ├── Interfaces/
    │   ├── ICalculator.cs                # Calculate(cart, date) → decimal
    │   └── IDiscountStrategy.cs          # AppliesTo + CalculatePrice contract
    │
    └── Models/
        ├── CartItem.cs                   # Product + PurchaseAmount; exposes BaseCost, IsSoldByWeight
        ├── Product.cs                    # Id, Name, Category, Price (validated ≥ 0)
        ├── PurchaseAmount.cs             # ForQuantity(int) / ForWeight(Weight) factory
        └── Weight.cs                     # Amount (validated ≥ 0) + WeightUnit

tests/
└── CodingChallenge.Shopping.Tests/
    ├── CodingChallenge.Shopping.Tests.csproj  # xUnit test project targeting net9.0
    │
    ├── LegacyParity/
    │   └── LegacyParityTests.cs          # Verifies expected output for known scenarios
    ├── Discounts/
    │   ├── ChristmasDiscountTests.cs     # Each December pricing tier
    │   └── SeniorDiscountTests.cs        # Senior hour by-unit, by-weight, and window boundary tests
    ├── BoundaryTests/
    │   └── BoundaryAndEdgeCaseTests.cs   # Edge times, empty/null carts, null/empty strategies, uncategorized items
    ├── Models/
    │   └── CartItemValidationTests.cs    # Product.Price, PurchaseAmount.ForQuantity, Weight ctor, CartItem null guards
    └── Extensibility/
        ├── FirstResponderDiscountTests.cs     # 15% off all items; composition with built-in strategies
        ├── ExtendedChristmasDiscountTests.cs  # 95% off Christmas items Jan 1–15
        └── CalculatorDecoratorTests.cs        # Proof-of-concept ICalculator decorators
                                               #   (TaxAwareCalculator, RoundingCalculator, stacked)
```

---

## Design Choices

### Strategy Pattern
Each discount rule is isolated in its own class that implements `IDiscountStrategy`:

```csharp
public interface IDiscountStrategy
{
    bool AppliesTo(CartItem item, DateTime checkoutDate);
    decimal CalculatePrice(CartItem item, DateTime checkoutDate);
}
```

Adding a new discount (e.g., `FirstResponderDiscountStrategy`) means creating one new file — no existing code changes required.

### Chain of Responsibility
`GroceryStoreCheckoutCalculator` (namespace `CodingChallenge.Shopping.Calculators`) walks an ordered list of strategies and applies the **first** one that matches each cart item. The `DefaultNoDiscountStrategy` is always last and always matches, acting as a safe catch-all.

Default strategy order:
1. `ChristmasDiscountStrategy` — checked first for Christmas items in December
2. `SeniorDiscountStrategy` — checked for Food items during the senior hour
3. `DefaultNoDiscountStrategy` — returns full price for everything else

### Dependency Injection Constructor
The calculator exposes two constructors:

| Constructor | When to use |
|---|---|
| `new GroceryStoreCheckoutCalculator()` | Default — registers all built-in strategies |
| `new GroceryStoreCheckoutCalculator(IEnumerable<IDiscountStrategy>)` | Custom pipeline — inject any set of strategies |

The injection constructor guards against misuse at construction time:
- `null` collection → `ArgumentNullException`
- empty collection → `ArgumentException` (a catch-all strategy is always required)

This satisfies the **Open/Closed Principle**: the calculator is open for extension (inject new strategies) and closed for modification (no `if/else` changes needed).

### Data-driven Tier Table (`ChristmasDiscountStrategy`)
Rather than a chain of named constants and `if`-branches, the Christmas tiers are defined as a `(MaxDay, Rate)[]` array. The table is walked in order and the first entry whose `MaxDay` is ≥ the current day of the month wins. Adding or adjusting a tier means editing one row of data — no new constants or branches needed.

```
(14, 0.20m)  // Dec  1–14: early season
(25, 0.60m)  // Dec 15–25: peak season
(31, 0.90m)  // Dec 26–31: post-Christmas clearance
```

### Extended Post-Christmas Clearance (`ExtendedChristmasDiscountStrategy`)
A test-only strategy (declared `file`-scoped in `ExtendedChristmasDiscountTests.cs`) that demonstrates how the pipeline can be extended without touching production code:

- **Applies to:** `Category.Christmas` items on **January 1–15** only
- **Discount:** 95% off (customer pays 5% of base cost)
- After January 15 the strategy's `AppliesTo` returns `false` and `DefaultNoDiscountStrategy` takes over

### Decorator Pattern (`ICalculator` decorators)
The `ICalculator` interface makes it trivial to wrap the core calculator with additional behaviour using the decorator pattern. Two proof-of-concept decorators live in `CalculatorDecoratorTests.cs` (declared `file`-scoped, test-only):

| Decorator | Behaviour |
|---|---|
| `TaxAwareCalculator` | Multiplies the inner total by `(1 + taxRate)` |
| `RoundingCalculator` | Rounds the inner total to 2 decimal places (`MidpointRounding.AwayFromZero`) |

Decorators compose cleanly and none of the production files need to change:

```csharp
ICalculator calculator =
    new RoundingCalculator(
        new TaxAwareCalculator(
            new GroceryStoreCheckoutCalculator(),
            taxRate: 0.0925m));
```

### `CartItem` model
`CartItem` composes two objects:

| Property | Type | Purpose |
|---|---|---|
| `Product` | `Product` | Id, Name, Category, Price — validated ≥ 0 at assignment |
| `Amount` | `PurchaseAmount` | Created via `ForQuantity(int)` or `ForWeight(Weight)` |

Both setters throw `ArgumentNullException` when assigned `null`. `BaseCost` and `IsSoldByWeight` are computed from the two properties, keeping pricing logic out of each strategy.

```csharp
// By quantity
new CartItem
{
    Product = new Product { Name = "Ornament", Category = Category.Christmas, Price = 8m },
    Amount = PurchaseAmount.ForQuantity(15)
}

// By weight
new CartItem
{
    Product = new Product { Name = "Apple", Category = Category.Food, Price = 3.27m },
    Amount = PurchaseAmount.ForWeight(new Weight(0.79m, WeightUnit.Pound))
}
```

### `PurchaseAmount` factory
Construction is enforced through static factories that validate at the boundary:

| Factory | Validates |
|---|---|
| `PurchaseAmount.ForQuantity(int)` | `quantity >= 0` — throws `ArgumentOutOfRangeException` |
| `PurchaseAmount.ForWeight(Weight)` | `weight != null` — throws `ArgumentNullException` |

The `Weight` constructor similarly validates `amount >= 0`.

### Strongly-typed enums
The legacy code used raw strings (`"Christmas"`, `"food"`) for category comparisons. The refactored code uses enums, eliminating typo-based bugs and enabling compiler-checked exhaustiveness:

| Enum | Values |
|---|---|
| `Category` | `Uncategorized`, `Christmas`, `Food` |
| `PurchaseAmountType` | `Quantity`, `Weight` |
| `WeightUnit` | `Pound`, `Ounce`, `Kilogram`, `Gram` |

---

## How to Run the Program

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### From the terminal

```bash
# From the repo root
dotnet run --project src/CodingChallenge.Shopping/CodingChallenge.Shopping.csproj
```

Or build first, then run:

```bash
dotnet build
dotnet run --project src/CodingChallenge.Shopping
```

### From Visual Studio
Open `CodingChallenge.Shopping.sln`, set `CodingChallenge.Shopping` as the startup project, and press **F5** or **Ctrl+F5**.

### Expected output
`Program.cs` runs two demo scenarios that mirror the original challenge:

1. **Christmas cart** — totals for November 30 (no discount) and December 30 (90% off Christmas items)
2. **Food cart** — totals for a regular checkout and during the senior discount hour (7:11 AM)

---

## How to Run the Tests

### From the terminal

```bash
# Run all tests from the repo root
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run a specific test project
dotnet test tests/CodingChallenge.Shopping.Tests/CodingChallenge.Shopping.Tests.csproj
```

### From Visual Studio
Open **Test Explorer** (`Test → Test Explorer`) and click **Run All Tests**.

### Test categories

| Folder | What it covers |
|---|---|
| `LegacyParity` | Expected output for known checkout scenarios |
| `Discounts` | Each December pricing tier and the senior hour window |
| `BoundaryTests` | Edge cases: boundary times, empty/null carts, null/empty strategy collections, uncategorized items |
| `Models` | Validation: `Product.Price`, `PurchaseAmount.ForQuantity`, `Weight` ctor, `CartItem` null guards |
| `Extensibility` | `FirstResponderDiscountTests` — custom strategy via DI; `ExtendedChristmasDiscountTests` — Jan 1–15 clearance; `CalculatorDecoratorTests` — `ICalculator` decorator pattern |

---

## CI — GitHub Actions

The workflow file is located at [.github/workflows/pr-tests.yml](.github/workflows/pr-tests.yml).

### Trigger
The workflow runs automatically on every **pull request targeting `main`**.

### What it does

| Step | Command |
|---|---|
| Restore | `dotnet restore` |
| Build | `dotnet build --no-restore --configuration Release` |
| Test | `dotnet test --no-build --configuration Release --verbosity normal` |
