# 03: In-Combat Free-Action Healing & ASCII Visuals

**What to build:** During encounters with the Dragon, the combat action prompt is expanded to `[A]ttack, [H]eal, or [R]etreat`. When an injured player inputs `h` or `heal`, a potion is consumed, health is restored to 100% of `MaxHealthPoints`, celebratory potion flask ASCII art and narrative recovery text are rendered, and the player is immediately returned to the action prompt to choose Attack or Retreat in the same round. The player is restricted to at most one potion per round. Attempting to heal at full health or with 0 potions displays an explanatory warning and re-prompts without wasting an action. The combat summary displays current potion belt count (`Healing Potions: X/3`).

**Blocked by:** 01: Player Healing Domain & Potion Belt Capacity

**Status:** completed

- [x] Combat prompt renders `[A]ttack, [H]eal, or [R]etreat`
- [x] `IsValidCombatChoice` accepts `"h"` and `"heal"`
- [x] Consuming a potion during combat executes a free action that heals the player to full HP and returns to the action prompt
- [x] Combat round restricts consumption to at most 1 potion per round
- [x] Attempting to heal at full health displays a localized warning and does not consume a potion or action
- [x] Attempting to heal with 0 potions displays a localized warning and returns to prompt without wasting an action
- [x] Potion flask ASCII art and recovery message are displayed upon successful heal
- [x] `Combat.GetCombatStatsDisplay()` displays `Healing Potions: X/3`
- [x] Unit tests in `CYOATests/CombatTests.cs` verify prompt validation, display formatting, and combat recovery logic
