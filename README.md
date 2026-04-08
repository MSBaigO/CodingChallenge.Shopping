# NContracts Coding Challenge — Grocery Store Checkout

A clean, extensible grocery store checkout calculator built with the **Strategy** and **Chain-of-Responsibility** patterns.

---

## Table of Contents

- [Project Overview](#project-overview)
- [Architecture Summary](#architecture-summary)
- [Folder & File Structure](#folder--file-structure)
- [Design Choices](#design-choices)
- [Assumptions](#assumptions)
- [How to Run the Program](#how-to-run-the-program)
- [How to Run the Tests](#how-to-run-the-tests)
- [CI — GitHub Actions](#ci--github-actions)

---

## Project Overview

The original code packed every pricing rule into a single method using deeply nested `if/else` chains with hard-coded string comparisons. Adding a new discount (e.g., first responder, extended January clearance) would require touching that monolithic method every time — exactly the problem the challenge prompt calls out.

This refactored solution replaces the monolith with a **strategy pipeline**: each discount rule lives in its own class behind a shared `IDiscountStrategy` interface. The calculator walks an ordered list of strategies and applies the first match. Adding a new discount means creating one new file — no existing code changes required.

---

## Architecture Summary

```
                    ┌────────────────────┐
                    │     Program.cs     │  Entry point — uses ICalculator abstraction
                    └────────┬───────────┘
                             │
                             ▼
                  ┌──────────────────────┐
                  │     ICalculator      │  Calculate(cart, date) → decimal
                  └──────────┬───────────┘
                             │
                             ▼
          ┌──────────────────────────────────────┐
          │  GroceryStoreCheckoutCalculator      │
          │  ─────────────────────────────────── │
          │  Walks an ordered list of strategies │
          │  and applies the FIRST match per item│
          └──────────────────┬───────────────────┘
                             │
            ┌────────────────┼────────────────┐
            ▼                ▼                ▼
   ┌─────────────┐  ┌──────────────┐  ┌──────────────────┐
   │  Christmas  │  │   Senior     │  │   Default (No    │
   │  Discount   │  │   Discount   │  │   Discount)      │
   │  Strategy   │  │   Strategy   │  │   Strategy       │
   └─────────────┘  └──────────────┘  └──────────────────┘
   Dec tiers:        Food items,       Always matches.
   20/60/90%         7–8 AM: 10% off   Returns BaseCost.
```

**Key patterns:**

| Pattern | Where | Purpose |
|---|---|---|
| Strategy | `IDiscountStrategy` implementations | Isolate each discount rule in its own class |
| Chain of Responsibility | `GroceryStoreCheckoutCalculator` | Walk strategies in order, apply first match |
| Decorator | `ICalculator` wrappers (test-only) | Layer cross-cutting concerns (tax, rounding) without modifying the core |
| Dependency Injection | Calculator constructor | Swap or compose strategy pipelines at construction time |

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
    │   ├── SeniorDiscountStrategy.cs     # 10% off Food during 7:00–8:59 AM window
    │   └── DefaultNoDiscountStrategy.cs  # Catch-all — full price, always matches
    │
    ├── Enums/
    │   └── Category.cs                   # Uncategorized | Christmas | Food
    │
    ├── Interfaces/
    │   ├── ICalculator.cs                # Calculate(cart, date) → decimal
    │   └── IDiscountStrategy.cs          # AppliesTo + CalculatePrice contract
    │
    └── Models/
        └── CartItem.cs                   # Product, price, quantity, weight, category
                                          #   with validation and computed BaseCost / IsSoldByWeight

tests/
└── CodingChallenge.Shopping.Tests/
    ├── CodingChallenge.Shopping.Tests.csproj  # xUnit test project targeting net9.0
    │
    ├── LegacyParity/
    │   └── LegacyParityTests.cs          # Proves refactored output matches original code exactly
    ├── Discounts/
    │   ├── ChristmasDiscountTests.cs     # Each December pricing tier + tier boundaries
    │   └── SeniorDiscountTests.cs        # Senior hour by-unit, by-weight, and window boundary tests
    ├── BoundaryTests/
    │   └── BoundaryAndEdgeCaseTests.cs   # Empty/null carts, null/empty strategies, uncategorized items
    ├── Models/
    │   └── CartItemValidationTests.cs    # Property validation (negative price/qty/weight), BaseCost, IsSoldByWeight
    └── Extensibility/
        ├── FirstResponderDiscountTests.cs     # 15% off all items; composition with built-in strategies
        ├── ExtendedChristmasDiscountTests.cs  # 95% off Christmas items Jan 1–15
        └── CalculatorDecoratorTests.cs        # ICalculator decorators (TaxAwareCalculator, RoundingCalculator, stacked)
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
`GroceryStoreCheckoutCalculator` walks an ordered list of strategies and applies the **first** one that matches each cart item. The `DefaultNoDiscountStrategy` is always last and always matches, acting as a safe catch-all.

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

### `CartItem` computed properties
`CartItem` exposes `BaseCost` and `IsSoldByWeight` as computed properties, removing the need for each strategy to repeat the `weight > 0` pricing logic. Property setters enforce non-negative values, catching invalid data at the model boundary.

### Strongly-typed `Category` enum
The legacy code used raw strings (`"Christmas"`, `"Food"`) for category comparisons. The refactored code uses a `Category` enum (`Uncategorized`, `Christmas`, `Food`), eliminating typo-based bugs and enabling compiler-checked exhaustiveness.

---

## Assumptions

1. **Discount exclusivity** — Only one discount applies per item. The first matching strategy wins; discounts do not stack (e.g., a Christmas food item in senior hour gets only the Christmas discount, not both).
2. **Senior hour window** — `SeniorDiscountStrategy` uses `hour > 6 && hour <= 8`, matching the original code's condition of `Hours > 6 && Hours <= 8`. This covers 7:00–8:59 AM (hours 7 and 8 inclusive). The 6:00–6:59 AM slot is intentionally excluded.
3. **Weight-based pricing applies only to Food** — `IsSoldByWeight` returns `true` only when `Category == Food && Weight > 0`. Christmas or uncategorized items with a non-zero weight are still priced by quantity, matching the original behavior.
4. **Christmas discounts apply only in December** — The original code only discounted Christmas items during December. Other months charge full price. Extending this (e.g., January clearance) is demonstrated in the extensibility tests but is not part of the default pipeline.
5. **No persistence or authentication** — This is a pure calculation library. There is no database, user session, or payment processing.
6. **Negative values are invalid** — Price, Quantity, and Weight reject negative values via setter validation. The original code had no guards; this was added as a minimal safety net.

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

---

## CI — GitHub Actions

The workflow file is located at [.github/workflows/pr-tests.yml](.github/workflows/pr-tests.yml).

### Trigger
The workflow runs automatically on every **pull request targeting `master`**.

### What it does

| Step | Command |
|---|---|
| Restore | `dotnet restore` |
| Build | `dotnet build --no-restore --configuration Release` |
| Test | `dotnet test --no-build --configuration Release --verbosity normal` |
