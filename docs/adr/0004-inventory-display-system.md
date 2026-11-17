# ADR 0004: Inventory Display Command and In-Combat Potion Visibility

* **Status:** Accepted
* **Date:** 2026-11-16
* **Deciders:** Development Team
* **Consulted:** Dr. Sean Humpherys (CIDM 4390)
* **Informed:** Development Team

---

## Context and Problem Statement

Following the addition of the Merchant equipment system (ADR 0002) and the Healing Potion / Ancient Shrine mechanics (ADR 0003), players accumulate gear (Weapons, Armor), consumables (Healing Potions), and currency (Gold). However, during exploration at the Mountain Crossroads and during active combat against the dragon, there was no dedicated, on-demand command to inspect what the player is carrying or wearing without either checking raw character attributes at creation or waiting for combat initialization banners. Furthermore, players in active combat could not easily discern their remaining potion count when choosing between attacking, healing, and retreating.

To enhance player awareness and tactical decision-making, we need a dedicated inventory display command and dynamic in-combat potion visibility.

---

## Decision Drivers

1. **Information Clarity & Separation of Concerns**: Decouple gear and consumable inspection from character biographical attributes (Strength, Agility, Health Points).
2. **Action Economy & Free-Action Inspection**: Enable players to inspect possessions at the Mountain Crossroads and in combat without penalty, turn consumption, or triggering enemy attacks.
3. **In-Combat Potion Transparency**: Dynamically display remaining potion capacity (`{0}/3 remaining`) directly inside the combat prompt to inform healing decisions.
4. **Decoupled Architecture & Testability**: Structure inventory rendering to return structured text from domain models (`Player.cs`) so formatting can be verified in automated unit tests without console redirection.
5. **Full Localization Parity (ADR 0001)**: Maintain 4-language parity (English, Spanish, French, Italian) for all headers, labels, prompt text, and validation hints in `language_data.json`.

---

## Considered Options

* **Option 1: Two-Section Structured Text Display with In-Combat Free Action & Dynamic Prompt (Chosen)**
  - Group inventory into two canonical domain categories: **Equipped Gear** (Weapon, Armor) and **Carried Items** (Gold, Healing Potions).
  - Provide `'i'`, `'inv'`, and `'inventory'` commands at the Mountain Crossroads and during combat as a non-turn-advancing free action.
  - Dynamically display remaining potion count in the combat prompt (`Enter 'a' to attack, 'h' to heal ({0}/3 remaining), 'i' for inventory, or 'r' to retreat.`).
  - Keep Merchant shop focused on purchasing (options 1–4) and character creation minimal.
* **Option 2: Multi-line Full ASCII Art Inventory Gallery**
  - Render full ASCII art for weapons, armor, and potion bottles simultaneously on every inventory check.
  - *Rejected*: Floods the console buffer with 25+ lines of ASCII graphics, obscuring narrative context and game prompts.
* **Option 3: Overloaded Character Summary Command**
  - Re-use the existing `DisplayStats()` method across exploration and combat.
  - *Rejected*: Clutters item inspection with raw character attributes and lacks dedicated domain categorization.

---

## Decision Outcome

**Chosen Option:** **Option 1**, because it provides clean domain categorization, non-intrusive terminal formatting, seamless accessibility in exploration and battle, and testable decoupled architecture.

### Implementation Details:
1. **Domain Model (`Player.cs`)**:
   - `GetInventoryDisplay(Messages messages)`: Decoupled method returning formatted multi-line inventory text partitioned into `Equipped Gear` and `Carried Items`.
   - `DisplayInventory(Messages messages)`: Thin wrapper invoking `Console.WriteLine(GetInventoryDisplay(messages))`.
2. **Exploration Navigation (`Game.cs`)**:
   - Update `path_prompt_full` and `path_invalid` across all four languages to include `'i' for Inventory`.
   - Update `IsValidPathChoice(string? input)` to accept `i`, `inv`, `inventory`.
   - Handle inventory inspection in crossroads loop by displaying inventory and re-prompting.
3. **Combat Integration (`Combat.cs`, `Game.cs`)**:
   - Update `IsValidCombatChoice(string? input)` to accept `i`, `inv`, `inventory`.
   - Update `attack_prompt` across all four languages to include dynamic `{0}/3 remaining` potion count and `'i' for inventory`.
   - Handle `i`/`inv`/`inventory` in `Combat.StartCombat()` as a free action without incrementing rounds or triggering dragon attack.
4. **Domain Terminology (`GLOSSARY.md`)**:
   - Canonical definitions added for **Inventory**, **Equipped Gear**, and **Carried Items** with explicit `_Avoid_` synonyms.
5. **Localization (`language_data.json`)**:
   - Added `inventory_header`, `inventory_equipped_header`, `inventory_carried_header`, `inventory_footer`, `inventory_weapon_format`, `inventory_armor_format`, `inventory_armor_none`, `inventory_gold_format`, and `inventory_potions_format` across English, Spanish, French, and Italian.
6. **Automated Verification (`CYOATests`)**:
   - Added unit tests in `PlayerTests.cs` for `GetInventoryDisplay` with starting gear, upgraded weapon, magical armor, and potions.
   - Added unit tests in `GameTests.cs` for path validation and combat choice validation.

---

## Consequences

### Positive:
* Clear, instantaneous player awareness of gear and consumable supplies in both exploration and combat.
* Zero turn penalty for inspecting inventory.
* Clean separation between character stat attributes and inventory gear.
* Adheres strictly to ADR 0001 localization invariants across all 4 languages.
* Fully unit testable without console stream redirection.

### Negative / Trade-offs:
* Adds new localized strings and prompt variations to `language_data.json`.
