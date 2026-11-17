# 01: Armor Mitigation & Standalone Inventory Aggregate

**What to build:** `Armor.cs` is deepened to encapsulate damage absorption (`AbsorbDamage`), calculating damage reduction down to a minimum floor of 1 damage point. A new `Inventory.cs` domain aggregate is created to encapsulate all **Equipped Gear** and **Carried Items**, exposing semantic methods for damage mitigation (`MitigateDamage`), currency transactions (`TrySpendGold`, `AddGold`), potion belt capacity bounds (`TryAddPotion`, `TryConsumePotion`, `CanAddPotion`), equipment swapping (`EquipWeapon`, `EquipArmor`), and localized presentation formatting (`GetInventoryDisplay`). A new test suite `CYOATests/InventoryTests.cs` tests all behavior and edge cases in isolation.

**Blocked by:** None (can start immediately)

**Status:** completed

- [x] `Armor.AbsorbDamage(int incomingDamage, out int absorbedDamage)` calculates damage reduction and enforces the 1-damage floor
- [x] `Inventory.cs` manages `EquippedWeapon`, `EquippedArmor`, `Gold` (default 50), and `HealingPotions` (default 0)
- [x] `Inventory.MitigateDamage(int incomingDamage, out int absorbedDamage)` delegates to `Armor` when equipped, or passes raw damage with 0 absorbed when unarmored
- [x] `Inventory.TrySpendGold(int amount)` deducts gold if sufficient funds exist and returns boolean success
- [x] `Inventory.AddGold(int amount)` increments gold for positive amounts
- [x] `Inventory.TryAddPotion()` adds a potion if `HealingPotions < 3` and returns true, or returns false when full
- [x] `Inventory.TryConsumePotion()` decrements a potion if `HealingPotions > 0` and returns true, or returns false when empty
- [x] `Inventory.GetInventoryDisplay(Messages messages)` formats equipped gear and carried items across English, Spanish, French, and Italian
- [x] `CYOATests/InventoryTests.cs` exercises all methods, boundaries, and localized displays in isolation
