# 02: Merchant Potion Transactions & Capacity Guard

**What to build:** The Merchant in the southern glade sells Healing Potions for 10 Gold each with unlimited stock. The shop menu offers four choices: 1. Buy Armor (30 Gold), 2. Buy Upgraded Weapon (30 Gold), 3. Buy Healing Potion (10 Gold), and 4. Leave Shop. When a player with sufficient gold (< 3 potions) purchases a potion, 10 Gold is deducted and 1 potion is added to their belt. If the player lacks 10 Gold or already carries 3 potions, the Merchant politely refuses the sale and no gold is deducted.

**Blocked by:** 01: Player Healing Domain & Potion Belt Capacity

**Status:** completed

- [x] `Merchant.EnterShop()` menu presents option `3. Buy Healing Potion (10 Gold)` and moves Leave to `4. Leave Shop`
- [x] `Merchant.IsValidChoice()` accepts `"1"`, `"2"`, `"3"`, and `"4"`
- [x] `Merchant.BuyPotion(Player, Messages)` deducts 10 Gold and increments `HealingPotions` when player has $\ge 10$ Gold and $< 3$ potions
- [x] `Merchant.BuyPotion(Player, Messages)` rejects purchase and displays error when player has $< 10$ Gold
- [x] `Merchant.BuyPotion(Player, Messages)` rejects purchase and displays error when player already carries 3 potions
- [x] Unit tests in `CYOATests/MerchantTests.cs` verify successful purchase, insufficient funds rejection, and full-capacity rejection
