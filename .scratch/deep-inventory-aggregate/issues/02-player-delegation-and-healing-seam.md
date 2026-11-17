# 02: Player Delegation & Vitality-Inventory Healing Seam

**What to build:** `Player.cs` wires up an internal `Inventory` instance (`public Inventory Inventory { get; } = new Inventory();`) and delegates its `Gold`, `HealingPotions`, `Weapon`, `Armor`, and `GetInventoryDisplay()` accessors directly to `Inventory`. `Player.HealWithPotion()` coordinates vitality checking (`HealthPoints < MaxHealthPoints`) and delegates consumable consumption to `Inventory.TryConsumePotion()`. All existing tests in `PlayerTests.cs` continue to pass 100% green without test changes.

**Blocked by:** 01: Armor Mitigation & Standalone Inventory Aggregate

**Status:** completed

- [x] `Player.Inventory` exposes the player's underlying inventory aggregate
- [x] `Player.Gold`, `Player.HealingPotions`, `Player.Weapon`, and `Player.Armor` forward getters and setters to `Inventory`
- [x] `Player.HealWithPotion()` verifies `HealthPoints < MaxHealthPoints` before calling `Inventory.TryConsumePotion()`
- [x] `Player.GetInventoryDisplay(messages)` delegates directly to `Inventory.GetInventoryDisplay(messages)`
- [x] `Player.DisplayStats()` continues to display current stats, gold, potions, weapon, and armor seamlessly
- [x] All existing unit tests in `CYOATests/PlayerTests.cs` pass with zero regressions
