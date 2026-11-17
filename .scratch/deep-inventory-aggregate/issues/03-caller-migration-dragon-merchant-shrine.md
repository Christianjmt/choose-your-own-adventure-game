# 03: Caller Migration Across Adversary, Merchant, and Shrine

**What to build:** External callers are migrated to use the deepened `Inventory` and `Armor` seams. `Dragon.cs` replaces hardcoded armor arithmetic with `player.Inventory.MitigateDamage(damage, out int absorbed)`. `Merchant.cs` performs transactions using `player.Inventory.TrySpendGold()`, `CanAddPotion`, `TryAddPotion()`, `EquipArmor()`, and `EquipWeapon()`. `Game.cs` updates `EvaluateShrineTrial()` to delegate the potion award to `player.Inventory.TryAddPotion()`. The entire unit test suite passes 100% green.

**Blocked by:** 02: Player Delegation & Vitality-Inventory Healing Seam

**Status:** completed

- [x] `Dragon.Attack()` calls `player.Inventory.MitigateDamage()` and removes all manual armor mitigation calculations
- [x] `Merchant.BuyArmor()` calls `player.Inventory.TrySpendGold()` and `player.Inventory.EquipArmor()`
- [x] `Merchant.BuyWeapon()` calls `player.Inventory.TrySpendGold()` and `player.Inventory.EquipWeapon()`
- [x] `Merchant.BuyPotion()` verifies `player.Inventory.CanAddPotion`, calls `player.Inventory.TrySpendGold()`, and calls `player.Inventory.TryAddPotion()`
- [x] `Game.EvaluateShrineTrial()` uses `player.Inventory.TryAddPotion()` to enforce capacity and claim state
- [x] All unit tests across `CombatTests.cs`, `MerchantTests.cs`, `GameTests.cs`, and `MessagesTests.cs` pass 100% green
