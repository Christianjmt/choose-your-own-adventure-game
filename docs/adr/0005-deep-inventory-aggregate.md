# ADR 0005: Deep Inventory Aggregate and Encapsulated Equipment Mitigation

* **Status:** Accepted
* **Date:** 2026-11-16
* **Deciders:** Development Team
* **Consulted:** Dr. Sean Humpherys (CIDM 4390)
* **Informed:** Development Team

---

## Context and Problem Statement

Following the introduction of the Merchant equipment progression (ADR 0002), Healing Potions (ADR 0003), and the dedicated inventory display command (ADR 0004), player possessions (Equipped Gear and Carried Items) accumulated substantial domain logic. However, this logic was scattered across external callers:
- `Armor.cs` was an anemic 13-line data bag; its core damage mitigation rule ($\max(1, \text{rawDamage} - \text{DamageReduction})$) was hardcoded in `Dragon.cs`.
- Potion belt capacity constraints ($\le 3$) were duplicated across `Merchant.cs`, `Game.cs`, and `Combat.cs`.
- Gold currency mutations (`player.Gold -= cost`) were directly performed by `Merchant.cs`.
- `Player.cs` was burdened with loose item properties and formatting presentation logic.

To eliminate anemic domain models, achieve high locality for equipment invariants, and provide a single deep module for inventory management, we require a dedicated `Inventory` aggregate.

---

## Decision Drivers

1. **Locality & High Cohesion**: Centralize equipment mitigation arithmetic, currency transactions, and potion capacity invariants into one module.
2. **Elimination of Anemic Data Bags**: Transform `Armor.cs` and `Inventory.cs` into rich behavioral domain models.
3. **Decoupled Testability**: Provide clean seams (`TrySpendGold`, `TryAddPotion`, `MitigateDamage`) that can be tested in isolation without instantiating full characters or combat encounters.
4. **Backward Compatibility & Low Blast Radius**: Preserve existing property accessors on `Player.cs` via delegation to prevent breaking existing call sites and unit tests.
5. **Preserving Value Objects Where Appropriate**: Retain `Weapon.cs` as a simple value object, leaving damage variance generation with the attacker's die.

---

## Considered Options

* **Option 1: Deep Inventory Aggregate with Encapsulated Mitigation & Delegating Player Accessors (Chosen)**
  - Introduce `Inventory.cs` encapsulating `EquippedWeapon`, `EquippedArmor`, `Gold`, and `HealingPotions`.
  - Add `AbsorbDamage` to `Armor.cs` and `MitigateDamage` to `Inventory.cs`, removing mitigation math from `Dragon.cs`.
  - Centralize transaction rules (`TrySpendGold`, `AddGold`, `TryAddPotion`, `TryConsumePotion`, `CanAddPotion`) on `Inventory.cs`.
  - Move `GetInventoryDisplay(Messages messages)` to `Inventory.cs`.
  - Provide delegating accessors on `Player.cs` for backward compatibility.
  - Leave `Weapon.cs` as a clean value object.
* **Option 2: Deepening Weapon with Embedded Die Rolling**
  - Make `Weapon.cs` own `RollDamage(Die die)` alongside `Armor.AbsorbDamage`.
  - *Rejected*: The team decided during architectural review that weapon damage variance is an attacker action properly owned by the attacking entity's combat resolution, keeping `Weapon.cs` as a simple value object.
* **Option 3: Maintaining Status Quo with Utility Helper Functions**
  - Keep loose properties on `Player.cs` and create a static helper for mitigation.
  - *Rejected*: Misses the opportunity to aggregate domain invariants into an object-oriented domain model, leaving entities anemic.

---

## Decision Outcome

**Chosen Option:** **Option 1**, because it creates a robust, highly cohesive domain model for inventory and equipment without breaking existing interfaces or test suites.

### Implementation Details:
1. **`Armor.cs`**:
   - Deepened with `public int AbsorbDamage(int incomingDamage, out int absorbedDamage)` enforcing the 1-damage floor.
2. **`Inventory.cs` (New Module)**:
   - Holds `EquippedWeapon`, `EquippedArmor`, `Gold` (default 50), and `HealingPotions` (default 0).
   - Exposes `MitigateDamage(int incomingDamage, out int absorbedDamage)`.
   - Exposes `TrySpendGold(int amount)`, `AddGold(int amount)`.
   - Exposes `TryAddPotion()`, `TryConsumePotion()`, `CanAddPotion`.
   - Exposes `EquipWeapon(Weapon weapon)`, `EquipArmor(Armor? armor)`.
   - Exposes `GetInventoryDisplay(Messages messages)`.
3. **`Player.cs`**:
   - Holds `public Inventory Inventory { get; } = new Inventory();`.
   - Forwards `Gold`, `HealingPotions`, `Weapon`, `Armor`, and `GetInventoryDisplay` to `Inventory`.
   - Coordinates `HealWithPotion()` by verifying vitality (`HealthPoints < MaxHealthPoints`) and delegating consumption to `Inventory.TryConsumePotion()`.
4. **`Dragon.cs`**:
   - Calls `player.Inventory.MitigateDamage(damage, out int absorbed)` instead of calculating mitigation directly.
5. **`Merchant.cs`**:
   - Executes purchases through `player.Inventory` transactional methods.
6. **`Game.cs`**:
   - Uses `player.Inventory.TryAddPotion()` in `EvaluateShrineTrial`.
7. **Automated Verification (`CYOATests`)**:
   - Introduces `InventoryTests.cs` to test currency, capacity caps, mitigation, and display formatting in isolation.

---

## Consequences

### Positive:
* Eliminates anemic model friction: `Armor` and `Inventory` own their business rules.
* Dragon and other adversaries no longer need to know how armor reduces damage.
* Hardcoded capacity literals (`3`) and raw balance mutations are eliminated from `Game.cs` and `Merchant.cs`.
* 100% backward compatible with existing tests via delegating properties on `Player`.

### Negative / Trade-offs:
* Adds a new domain class file (`Inventory.cs`).
