# 01: Player Healing Domain & Potion Belt Capacity

**What to build:** The Player tracks their Maximum Health Points established at character creation and carries a potion belt with a capacity of up to 3 Healing Potions (starting at 0). When damaged, consuming a potion restores the player to 100% of their Maximum Health Points. Potion consumption is rejected if the player is already at full health or if their potion belt is empty. Character stats screen displays the current potion belt count (e.g., `Healing Potions: 0/3`).

**Blocked by:** None (can start immediately)

**Status:** completed

- [x] `Player.MaxHealthPoints` is set during character creation to the initial rolled health value
- [x] `Player.HealingPotions` starts at 0 with an invariant range of [0, 3]
- [x] `Player.HealWithPotion()` restores `HealthPoints` to `MaxHealthPoints` and decrements `HealingPotions` by 1 when damaged
- [x] `Player.HealWithPotion()` returns `false` and preserves potion count when `HealthPoints == MaxHealthPoints`
- [x] `Player.HealWithPotion()` returns `false` when `HealingPotions == 0`
- [x] `Player.DisplayStats()` displays localized `Healing Potions: X/3`
- [x] Unit tests in `CYOATests` verify initial state, healing recovery, capacity bounds, and rejection guards
