# ADR 0003: Healing Potion Consumable and Ancient Shrine System

* **Status:** Accepted
* **Date:** 2026-11-16
* **Deciders:** Development Team
* **Consulted:** Dr. Sean Humpherys (CIDM 4390)
* **Informed:** Development Team

---

## Context and Problem Statement

Following the introduction of the Merchant encounter and equipment system (ADR 0002), the player can enhance offensive and defensive stats. However, combat remains an attrition battle with zero recovery mechanics: damage suffered during battle against the dragon permanently depletes `HealthPoints` until defeat or retreat. Furthermore, the mountain crossroads offers only North (Dragon) and South (Merchant), leaving map exploration restricted.

To expand player tactical agency, introduce in-battle survivability recovery, and provide exploration diversity, we require a consumable healing item (Healing Potion) that can be purchased from the Merchant or retrieved from an Ancient Shrine along a new East path.

---

## Decision Drivers

1. **In-Battle Recovery with Meaningful Resource Management**: Provide high-impact survivability (100% full health recovery) balanced by strict inventory capacity (maximum 3 potions) and single-use consumption.
2. **Dual Acquisition Channels (Economy vs. Exploration)**: Offer deterministic purchase from the Merchant (10 Gold each) alongside a risk-free skill check discovery trial at an Ancient Shrine in the East.
3. **Exploration Pacing & Retreat Synergy**: Create a reset mechanic where a sealed shrine only reopens after the player dares to confront the dragon and retreat, incentivizing cyclical gameplay.
4. **Action Economy Clarity**: Make drinking a potion in combat a free action that returns to the attack prompt, preventing wasted turns while enforcing a strict 1-potion-per-round limit and full-health consumption guards.
5. **Architectural Consistency & Localization Parity (ADR 0001)**: Extend existing domain classes (`Player`, `Combat`, `Merchant`, `Game`) and maintain full 4-language parity (English, Spanish, French, Italian) in `language_data.json`.

---

## Considered Options

* **Option 1: Full-Heal Consumable with Free Action & Ancient Shrine Reset Loop (Chosen)**
  * Track `MaxHealthPoints` at character creation; potion restores 100% HP up to `MaxHealthPoints`.
  * Player can carry up to 3 potions (`HealingPotions`).
  * Consuming a potion is a free action during combat, returning to `[A]ttack` or `[R]etreat` (capped at 1 potion/round). Guard against consumption when at full health or with 0 potions.
  * Merchant sells potions for 10 Gold (unlimited stock up to belt capacity).
  * East path introduces Ancient Shrine: requires a d20 roll $\ge 12$ to retrieve 1 potion (1-time claim). On failure, shrine seals until the player retreats from the dragon.
* **Option 2: Incremental Flat Heal with Combat Turn Consumption**
  * Potion restores a flat 10 HP and consumes the player's turn, allowing the dragon to retaliate immediately.
* **Option 3: Out-of-Combat Only Healing**
  * Potions can only be consumed at the crossroads between battles, not during active combat.

---

## Decision Outcome

**Chosen Option:** **Option 1**, because full restoration creates a decisive tactical reset during high-damage dragon encounters, and the shrine reset mechanic directly rewards the retreat-and-prepare loop established in ADR 0002.

### Implementation Details:
1. **Domain Model (`Player.cs`)**:
   - Add `MaxHealthPoints` recorded at character creation (`_die.Roll(20) + strengthModifier`).
   - Add `HealingPotions` counter (starts at 0, max 3).
   - Add `HealWithPotion()` method returning boolean (false if 0 potions or already at `MaxHealthPoints`).
   - Update `DisplayStats()` to show `Healing Potions: X/3`.
2. **Combat Integration (`Combat.cs`)**:
   - Expand combat prompt to `[A]ttack, [H]eal, or [R]etreat`.
   - Selecting `[H]eal` executes `HealWithPotion()`, displays ASCII art, and returns to prompt. Capped at 1 potion per round.
   - Update `GetCombatStatsDisplay()` to display `Healing Potions: X/3`.
3. **Merchant Shop (`Merchant.cs`)**:
   - Add Option 3: `Buy Healing Potion (10 Gold)`.
   - Enforce capacity guard: reject transaction if player already holds 3 potions.
4. **Crossroads & Ancient Shrine (`Game.cs`)**:
   - Add `[E]ast` to crossroads navigation (`PromptForPath`).
   - Implement `HandleShrineEncounter(Player player, Messages messages)`: roll d20 $\ge 12$ for success.
   - Track `_shrineClaimed` and `_shrineSealed`. If failed, `_shrineSealed = true`.
   - Update `HandleDragonEncounter`: upon retreat, set `_shrineSealed = false`, reopening the shrine trial.
5. **Localization (`language_data.json`, `Messages.cs`)**:
   - Add shrine narrative, success/fail/sealed messages, merchant potion strings, combat heal logs, and full-health warnings across English, Spanish, French, and Italian.

---

## Consequences

### Positive:
* Provides deep tactical flexibility when confronting the dragon.
* Integrates the new East path seamlessly into the existing crossroads world model.
* Clean separation of concerns with testable methods on `Player`, `Merchant`, and `Combat`.
* Adheres strictly to ADR 0001 localization and ADR 0002 economy standards.

### Negative / Trade-offs:
* Full heal in combat increases player win rates unless dragon attack output remains threatening.
* Adds state tracking variables to `Game.cs` (`_shrineClaimed`, `_shrineSealed`).
