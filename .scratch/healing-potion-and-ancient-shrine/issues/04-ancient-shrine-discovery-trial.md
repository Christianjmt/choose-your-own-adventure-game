# 04: Ancient Shrine Discovery Trial & Retreat Synergy

**What to build:** The mountain crossroads navigation is expanded to include `[E]ast (Ancient Shrine)`. Traveling East presents a sanctuary with a single hidden Healing Potion. To retrieve it, the player rolls a 20-sided die: rolling 12 or higher succeeds, awarding 1 potion with celebratory ASCII art (or leaving it on the altar if the player's belt is full at 3/3). Rolling below 12 fails, dealing no damage but sealing the shrine vault shut. When the player encounters the Dragon in the North and retreats, any sealed shrine reopens, allowing a fresh attempt. Exiting the shrine safely returns the player to the mountain crossroads.

**Blocked by:** 01: Player Healing Domain & Potion Belt Capacity

**Status:** completed

- [x] Crossroads navigation prompt includes East (`[E]ast (Ancient Shrine)`)
- [x] `Game.IsValidPathChoice()` accepts `"e"` and `"east"`
- [x] Traveling East executes the Ancient Shrine encounter and returns to crossroads
- [x] A d20 roll $\ge 12$ claims the potion and adds 1 to `HealingPotions` if belt $< 3$
- [x] A d20 roll $\ge 12$ while belt is at 3/3 leaves the potion on the altar and keeps shrine available
- [x] A d20 roll $< 12$ seals the shrine vault without dealing damage
- [x] Visiting a sealed shrine displays a locked vault message and returns to crossroads
- [x] Retreating from the Dragon in the North clears the sealed state, unsealing the shrine for future attempts
- [x] Unit tests in `CYOATests` verify shrine trial math, capacity handling, sealing state transitions, and retreat unsealing
