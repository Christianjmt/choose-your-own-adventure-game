# Specification: Deep Inventory Aggregate and Encapsulated Equipment Mitigation

**Triage Label:** `ready-for-agent`  
**Governing ADRs:** ADR 0001 (Localization Parity), ADR 0002 (Merchant Encounter & Equipment), ADR 0003 (Healing Potion & Ancient Shrine), ADR 0004 (Inventory Display System), ADR 0005 (Deep Inventory Aggregate & Encapsulated Equipment Mitigation)  
**Domain Vocabulary:** Defined in `GLOSSARY.md` (`Inventory`, `Equipped Gear`, `Carried Items`, `Healing Potion`, `Maximum Health Points`, `Merchant`, `Dragon`, `Ancient Shrine`)

---

## Problem Statement

Following successive feature expansions, the domain logic surrounding player possessions—**Equipped Gear** and **Carried Items**—has become fragmented and anemic across the codebase. `Armor.cs` is a passive 13-line data bag; its core damage mitigation rule ($\max(1, \text{rawDamage} - \text{DamageReduction})$) is hardcoded inside `Dragon.cs`. Potion belt capacity constraints ($\le 3$) and gold transactions are manually enforced by multiple external callers (`Merchant.cs`, `Game.cs`, and `Combat.cs`). 

Furthermore, `Player.cs` directly manages loose item fields and presentation formatting, making equipment and currency rules impossible to test in isolation from character creation and combat. The architecture requires a deep, cohesive **Inventory** aggregate that encapsulates all equipment mitigation, currency rules, and consumable capacity bounds behind a clean, testable seam while keeping `Player` backward compatible.

---

## Solution

Introduce a deep `Inventory` aggregate module that encapsulates all **Equipped Gear** (wielded Weapon, worn Armor) and **Carried Items** (Gold balance, Healing Potions belt), and deepen `Armor.cs` to encapsulate damage absorption:

1. **Encapsulated Damage Mitigation**: `Armor.cs` owns `AbsorbDamage(incomingDamage, out absorbedDamage)`, enforcing the 1-damage floor. `Inventory.cs` exposes `MitigateDamage(incomingDamage, out absorbedDamage)`, seamlessly handling both armored and unarmored combat encounters. Adversaries like `Dragon.cs` interact purely through this seam without knowing armor formulas.
2. **Centralized Currency & Consumable Invariants**: `Inventory.cs` encapsulates transactions through semantic methods (`TrySpendGold`, `AddGold`, `TryAddPotion`, `TryConsumePotion`, `CanAddPotion`), enforcing the 3-potion belt capacity and positive gold balances in one place.
3. **Co-located Presentation**: `GetInventoryDisplay(Messages messages)` lives on `Inventory.cs`, keeping presentation rendering co-located with item data.
4. **Backward-Compatible Seam**: `Player.cs` holds an instance of `Inventory` and forwards `Gold`, `HealingPotions`, `Weapon`, `Armor`, and `GetInventoryDisplay()` to it via delegating accessors, ensuring zero regressions across existing test suites and call sites.
5. **Preserving Value Objects**: `Weapon.cs` remains an immutable value object, maintaining attacker-owned damage rolling via `_die.Roll(Weapon.MaxDamage)`.

---

## User Stories

1. As a Developer maintaining the combat system, I want incoming damage mitigation encapsulated in `Armor` and `Inventory`, so that adversary code like `Dragon.cs` does not contain hardcoded armor math.
2. As a Developer adding future enemies or environmental hazards, I want to call a single `MitigateDamage` method on `Inventory`, so that all damage sources automatically respect armor absorption and the 1-damage floor.
3. As an unarmored Player taking damage, I want `MitigateDamage` to pass through incoming damage with 0 damage absorbed, so that combat remains fair before buying armor.
4. As an armored Player taking dragon attack damage greater than my armor reduction, I want the incoming damage reduced by my armor's damage reduction rating, so that I survive longer in battle.
5. As an armored Player taking attack damage less than or equal to my armor reduction, I want the final damage floored at exactly 1 damage point, so that attacks always deal minimal threat.
6. As a Player visiting the Merchant, I want purchasing items to deduct gold through atomic `Inventory` transaction methods, so that my gold balance cannot enter negative debt.
7. As a Player with full potion belt capacity (3 potions), I want `Inventory.TryAddPotion()` to reject adding a 4th potion, so that the 3-potion capacity invariant is enforced in one central place.
8. As a Player carrying fewer than 3 potions, I want `Inventory.TryAddPotion()` to increment my potion count by 1 and return true, so that acquiring supplies succeeds cleanly.
9. As an injured Player consuming a potion, I want `Inventory.TryConsumePotion()` to decrement my potion count by 1 and return true, so that my emergency draught is expended.
10. As a Player with 0 potions, I want `Inventory.TryConsumePotion()` to return false and leave my inventory unchanged, so that empty belts cannot produce phantom heals.
11. As a Player at full health attempting to heal, I want `Player.HealWithPotion()` to reject consumption before decrementing `Inventory`, so that valuable potions are not wasted when health is already full.
12. As a Player visiting the Ancient Shrine, I want a successful trial roll to attempt `Inventory.TryAddPotion()`, so that the shrine trial cleanly respects my belt capacity without hardcoded literals in `Game.cs`.
13. As a Player purchasing upgraded equipment from the Merchant, I want `EquipWeapon` and `EquipArmor` on `Inventory` to cleanly replace existing gear, so that upgrades apply immediately.
14. As a Player checking my inventory, I want `Inventory.GetInventoryDisplay(messages)` to format my equipped gear and carried items across all four supported languages, so that localization parity (ADR 0001) is preserved.
15. As a Developer writing automated tests, I want to test `Inventory` in complete isolation without instantiating characters, dice, or combat loops, so that unit testing is fast and modular.
16. As a Developer running the existing 182-test suite, I want `Player.Gold`, `Player.HealingPotions`, `Player.Weapon`, and `Player.Armor` to remain accessible via delegating properties, so that existing tests pass with zero regressions.

---

## Implementation Decisions

### Modules Modified & Created
- **`Armor.cs` (Behavioral Deepening)**:
  - Add `public int AbsorbDamage(int incomingDamage, out int absorbedDamage)`.
  - Calculates `absorbedDamage = DamageReduction;`.
  - Returns `Math.Max(1, incomingDamage - absorbedDamage);`.

- **`Weapon.cs` (Value Object Preservation)**:
  - Remains a lightweight value object with `Type`, `MaxDamage`, and `AsciiArt`. No behavioral changes.

- **`Inventory.cs` (New Deep Aggregate)**:
  - Encapsulates `EquippedWeapon` (Weapon?), `EquippedArmor` (Armor?), `Gold` (int, default 50), and `HealingPotions` (int, default 0).
  - Exposes `public int MitigateDamage(int incomingDamage, out int absorbedDamage)`:
    - If `EquippedArmor != null`, delegates to `EquippedArmor.AbsorbDamage`.
    - Else, `absorbedDamage = 0; return incomingDamage;`.
  - Exposes `public bool TrySpendGold(int amount)`:
    - If `amount <= 0` or `Gold < amount`, returns `false`.
    - Else, `Gold -= amount; return true;`.
  - Exposes `public void AddGold(int amount)`:
    - If `amount > 0`, `Gold += amount;`.
  - Exposes `public bool CanAddPotion => HealingPotions < 3;`.
  - Exposes `public bool TryAddPotion()`:
    - If `CanAddPotion`, `HealingPotions++; return true;`.
    - Else, returns `false;`.
  - Exposes `public bool TryConsumePotion()`:
    - If `HealingPotions > 0`, `HealingPotions--; return true;`.
    - Else, returns `false;`.
  - Exposes `public void EquipWeapon(Weapon weapon)` and `public void EquipArmor(Armor? armor)`.
  - Exposes `public void SetGold(int amount)` and `public void SetHealingPotions(int amount)` (for test setup and state initialization).
  - Owns `public string GetInventoryDisplay(Messages messages)` formatting Equipped Gear and Carried Items.

- **`Player.cs` (Seam & Delegation)**:
  - Owns `public Inventory Inventory { get; } = new Inventory();`.
  - Forwarding properties:
    - `public int Gold { get => Inventory.Gold; set => Inventory.SetGold(value); }`
    - `public int HealingPotions { get => Inventory.HealingPotions; set => Inventory.SetHealingPotions(value); }`
    - `public Weapon? Weapon { get => Inventory.EquippedWeapon; set { if (value != null) Inventory.EquipWeapon(value); else Inventory.EquipWeapon(null!); } }`
    - `public Armor? Armor { get => Inventory.EquippedArmor; set => Inventory.EquipArmor(value); }`
  - `HealWithPotion()` coordinates:
    - If `HealthPoints >= MaxHealthPoints`, return `false`.
    - If `!Inventory.TryConsumePotion()`, return `false`.
    - `HealthPoints = MaxHealthPoints; return true;`.
  - `GetInventoryDisplay(Messages messages)` delegates to `Inventory.GetInventoryDisplay(messages)`.

- **`Dragon.cs` (Caller Simplification)**:
  - In `Attack(Player player, Messages messages)`, replaces manual armor mitigation with:
    ```csharp
    int damage = RollDamage();
    damage = player.Inventory.MitigateDamage(damage, out int absorbed);
    if (absorbed > 0)
    {
        output.Add(string.Format(messages.GetMessage("armor_absorbed_damage"), absorbed));
    }
    player.HealthPoints -= damage;
    ```

- **`Merchant.cs` (Semantic Purchases)**:
  - `BuyArmor`: checks `!ArmorSold`, calls `player.Inventory.TrySpendGold(ArmorCost)`, calls `player.Inventory.EquipArmor(...)`.
  - `BuyWeapon`: checks `!WeaponSold`, calls `player.Inventory.TrySpendGold(WeaponCost)`, calls `player.Inventory.EquipWeapon(...)`.
  - `BuyPotion`: checks `player.Inventory.CanAddPotion`, calls `player.Inventory.TrySpendGold(PotionCost)`, calls `player.Inventory.TryAddPotion()`.

- **`Game.cs` (Ancient Shrine Seam)**:
  - In `EvaluateShrineTrial`:
    ```csharp
    if (d20Roll >= 12)
    {
        if (player.Inventory.TryAddPotion())
        {
            ShrineClaimed = true;
            return ShrineTrialResult.SuccessClaimed;
        }
        return ShrineTrialResult.SuccessBeltFull;
    }
    ```

---

## Testing Decisions

### Testing Principles
- Test `Inventory` directly through its public behavioral seam without needing character generation or combat fixtures.
- Retain existing unit tests to prove backward compatibility.

### Modules Under Test
- **`CYOATests/InventoryTests.cs` (New Test Suite)**:
  - Initial state defaults: 50 Gold, 0 Healing Potions, null Armor, null Weapon.
  - Currency operations: `TrySpendGold` (exact, insufficient, excess), `AddGold`.
  - Potion capacity bounds: `TryAddPotion` (succeeds up to 3, fails on 4th), `TryConsumePotion` (decrements to 0, fails at 0).
  - Damage mitigation:
    - Armored: `AbsorbDamage` reduces damage, verifies floor of 1.
    - Unarmored: passes raw damage through, 0 absorbed.
  - Equipment assignment: `EquipWeapon`, `EquipArmor`.
  - Presentation rendering: `GetInventoryDisplay` across English, Spanish, French, Italian.
- **Existing Test Suites**:
  - `CYOATests/PlayerTests.cs`, `CombatTests.cs`, `MerchantTests.cs`, `GameTests.cs` must continue to pass 100% green.

### Prior Art
- `PlayerTests.cs` and `CombatTests.cs` for character stats and damage assertions.
- `MerchantTests.cs` for purchase boundaries and funds checks.

---

## Out of Scope

- Discarding, dropping, or selling items.
- Adding new equipment slots (rings, amulets, helmets).
- Weapon damage variance on `Weapon.cs` (explicitly rejected in design review; kept on attacker).

---

## Further Notes

- Fully conforms to ADRs 0001, 0002, 0003, 0004, and 0005.
