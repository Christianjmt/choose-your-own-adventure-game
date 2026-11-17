# ADR 0002: Implement Merchant Encounter and Equipment Progression System

* **Status:** Accepted
* **Date:** 2026-11-16
* **Deciders:** Development Team
* **Consulted:** Dr. Sean Humpherys (CIDM 4390)
* **Informed:** Development Team

---

## Context and Problem Statement

In the Choose Your Own Adventure (CYOA) game, traveling South along the mountain path resulted in a static narrative ending ("nothing adventurous happens... you return home, poor and bored") that looped without engaging gameplay. Furthermore, combat against the dragon in the North offered no equipment progression or mitigation mechanics—the player always faced the dragon with their starting class weapon and zero armor.

To deepen gameplay and create meaningful tactical choices, we require an interactive Merchant encounter when traveling South, allowing players to purchase magical armor (increased protection) or a magical weapon (increased damage).

---

## Decision Drivers

1. **Meaningful Strategic Progression**: Provide a tangible choice between offensive capability (increased weapon damage) and defensive survivability (incoming damage absorption).
2. **Modular Architecture & Testability**: Introduce dedicated domain classes (`Armor.cs`, `Merchant.cs`) with decoupled business logic that can be tested in isolation without standard console stream redirection.
3. **Full Localization Parity (ADR 0001)**: Maintain complete 4-language support (English, Spanish, French, Italian) for all narrative prompts, shop menus, item names, and combat messages in `language_data.json`.
4. **Cohesive Game Flow**: Support an adventure loop where players can visit the merchant, purchase equipment, and return to the mountain crossroads to face the dragon in the North, even after retreating from combat.

---

## Considered Options

* **Option 1: Dedicated Modular System with Economy (Chosen)**
  * Introduce `Gold` (starting at 50) and an `Armor` class with flat damage reduction (4 points, minimum 1 damage).
  * Encapsulate shop logic in `Merchant.cs` with persistent stock (30 Gold per item, preventing repeat purchases).
  * Replace starting weapons with occupation-tailored magical weapons (+6 MaxDamage).
  * Integrate full localization across English, Spanish, French, and Italian.
* **Option 2: Free Gift / One-Time Boon**
  * Present a simple binary dialogue prompt to pick one upgrade for free without a currency system.
* **Option 3: Monolithic Implementation in `Game.cs`**
  * Embed all merchant prompts, equipment logic, and mitigation directly within `Game.cs` and `Dragon.cs`.

---

## Decision Outcome

**Chosen Option:** **Option 1**, because it creates a robust architectural foundation for inventory, currency, and defensive equipment while maintaining strict testability and alignment with ADR 0001 localization standards.

### Implementation Details:
1. **Domain Models**:
   - `Armor.cs`: Models protective gear with `Type`, `DamageReduction`, and `AsciiArt`.
   - `Player.cs`: Extended with `Gold = 50` and `Armor? Armor`. `DisplayStats()` updated to display current gold and equipped armor.
   - `Merchant.cs`: Encapsulates transaction logic (`BuyArmor`, `BuyWeapon`, `IsValidChoice`) and UI loop (`EnterShop`).
2. **Combat Integration (`Dragon.cs`, `Combat.cs`)**:
   - Mitigates dragon attack damage using $\text{damage} = \max(1, \text{rawDamage} - \text{player.Armor.DamageReduction})$.
   - Logs armor damage absorption in the combat narrative.
   - Updates `GetCombatStatsDisplay()` to display Player Gold and Armor rating.
3. **Navigation Flow (`Game.cs`)**:
   - South path triggers `merchant.EnterShop(player, messages)`.
   - Completing or leaving the shop returns the player to the mountain crossroads (`PromptForPath`).
   - Retreating from the dragon allows returning South to visit the merchant.
4. **Localization (`language_data.json`, `Messages.cs`)**:
   - Added all merchant dialogue, item names, error messages, and combat absorption text across English, Spanish, French, and Italian.
   - Added `displayArmorMap` and updated `displayWeaponMap`.
5. **Automated Verification (`CYOATests`)**:
   - Added `MerchantTests.cs` for input validation, purchases, stock exhaustion, and insufficient funds.
   - Updated `CombatTests.cs` and `MessagesTests.cs` for combat damage reduction and dictionary verification.

---

## Pros and Cons of the Options

### Option 1: Modular Economy and Equipment (Selected)
* **Positive:** Clear separation of concerns; `Merchant.cs` and `Armor.cs` are easily extensible and testable.
* **Positive:** Meaningful resource management (50 Gold forces a tactical choice between 30 Gold Armor or 30 Gold Weapon).
* **Positive:** Preserves clean OOP design principles and adherence to ADR 0001.
* **Negative:** Requires updating schema and translations across 4 languages in `language_data.json`.

### Option 2: Free Gift / One-Time Boon
* **Positive:** Simpler implementation with no currency tracking.
* **Negative:** Lacks economic depth; fails to support future expansion (e.g., loot drops, selling, multiple shop items).

### Option 3: Monolithic Implementation in `Game.cs`
* **Positive:** Fewer new files.
* **Negative:** Bloats `Game.cs`, tightly couples UI to game mechanics, and severely impairs automated unit testing.

---

## Consequences

* **Positive Consequences:**
  * Players have active agency to customize their strategy (offense vs. defense) before fighting the dragon.
  * The codebase gains reusable, cleanly tested `Armor` and `Merchant` components.
  * Full localization across 4 languages is preserved with zero regression.
* **Negative Consequences & Trade-offs:**
  * Adds new dictionary keys and mapping tables to `language_data.json`.
  * Unit test suite expands to cover merchant transactions and armor mitigation invariants.
