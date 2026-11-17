# Specification: Inventory Display Command and In-Combat Potion Visibility

**Triage Label:** `ready-for-agent`  
**Governing ADRs:** ADR 0001 (Localization Parity), ADR 0002 (Merchant Encounter & Equipment Progression), ADR 0003 (Healing Potion & Ancient Shrine), ADR 0004 (Inventory Display System)  
**Domain Vocabulary:** Defined in `GLOSSARY.md` (`Inventory`, `Equipped Gear`, `Carried Items`, `Healing Potion`, `Maximum Health Points`, `Merchant`, `Dragon`, `Ancient Shrine`)

---

## Problem Statement

As players explore the world and acquire items through commerce at the Merchant's Glade or fortune at the Ancient Shrine, there is no direct, on-demand way to view what their character is currently carrying or wearing during exploration or active combat. Currently, gear is only shown mixed alongside raw biographical attributes (Strength, Agility, Health Points) during character creation, or in the initial combat summary header when entering battle. 

Furthermore, during active encounters with the Dragon, the player has no visibility into how many Healing Potions remain on their belt when choosing between attacking, healing, or retreating, forcing players to guess their remaining survivability. Players need a dedicated, non-intrusive inventory display command during exploration and active combat, along with real-time potion transparency during life-or-death battles.

---

## Solution

Introduce a dedicated **Inventory** inspection command that clearly distinguishes between **Equipped Gear** (offensive Weapon and defensive Armor) and **Carried Items** (Gold currency and Healing Potions). 

1. **Mountain Crossroads Exploration**: Players navigating the mountain crossroads can enter `i`, `inv`, or `inventory` to display their complete inventory in a clean, structured text format without advancing time or consuming a travel action, immediately re-prompting for a direction.
2. **In-Combat Potion Visibility & Free Action**: The combat action prompt dynamically displays the player's remaining potions (`{0}/3 remaining`) on each round. Additionally, players can input `i`, `inv`, or `inventory` in combat as a free action to inspect their inventory without spending their turn or incurring Dragon counterattacks.
3. **Full Localization Parity**: In accordance with ADR 0001, all headers, labels, formatted rows, and prompt options are fully translated across English, Spanish, French, and Italian.

---

## User Stories

1. As an exploring Player standing at the Mountain Crossroads, I want to type `i`, `inv`, or `inventory`, so that I can inspect what my character is wearing and carrying without leaving the crossroads.
2. As an exploring Player viewing my inventory, I want to see an "Equipped Gear" section displaying my current Weapon name and Maximum Damage Points, so that I understand my offensive potential.
3. As an exploring Player viewing my inventory, I want to see my equipped Armor name and Damage Reduction value, so that I understand my defensive mitigation.
4. As an unarmored Player viewing my inventory, I want the Armor row to state "None" in my chosen language, so that I clearly know I have not yet bought protective gear from the Merchant.
5. As an exploring Player viewing my inventory, I want to see a "Carried Items" section displaying my current Gold amount, so that I know my purchasing power before visiting the Merchant.
6. As an exploring Player viewing my inventory, I want to see my current Healing Potions count out of capacity (`X/3`), so that I know how many emergency draughts I have before challenging the Dragon.
7. As an exploring Player after viewing my inventory at the Crossroads, I want the crossroads navigation prompt to immediately reappear without advancing game state or triggering events, so that inspecting my gear has zero negative penalty.
8. As an exploring Player reading the Mountain Crossroads navigation prompt, I want the prompt text to explicitly state that `i` opens the inventory, so that I am clearly informed of the command.
9. As an exploring Player entering an invalid choice at the crossroads, I want the error message to list `i` among valid options, so that I know the proper commands to use.
10. As a Player locked in combat with the Dragon, I want the round prompt to display my remaining Healing Potions count (e.g., `[h]eal (2/3 remaining)`), so that I do not need to guess if I have potions left to heal.
11. As a Player in combat who has used all Healing Potions, I want the prompt to display `0/3 remaining`, so that I know not to waste an input attempting to heal.
12. As a Player in combat, I want the action prompt to explicitly include `i` for inventory alongside `a`, `h`, and `r`, so that I know I can check my gear during battle.
13. As a Player in combat who inputs `i`, `inv`, or `inventory`, I want the game to display my complete inventory and return directly to the combat prompt without passing my turn, so that checking my gear does not allow the Dragon to attack me.
14. As a Player in combat who has already used a potion in the current round, I want the inventory command to remain available as a free action, so that I can still inspect my stats before committing to an attack or retreat.
15. As a Player viewing my inventory, I want the output formatted as clean, compact text rather than multi-line ASCII art, so that it does not push previous dialogue off my terminal screen.
16. As an English-speaking Player, I want all inventory titles, section headers, labels, and prompts displayed in English, so that the game is natural to read.
17. As a Spanish-speaking Player, I want all inventory titles, section headers, labels, and prompts displayed in Spanish, so that the game conforms to ADR 0001.
18. As a French-speaking Player, I want all inventory titles, section headers, labels, and prompts displayed in French, so that the game conforms to ADR 0001.
19. As an Italian-speaking Player, I want all inventory titles, section headers, labels, and prompts displayed in Italian, so that the game conforms to ADR 0001.
20. As a Player creating a new character, I want the character creation menu to remain concise with options 1 (List stats) and 2 (Proceed), so that character generation remains quick and focused.
21. As a Player visiting the Merchant in the southern glade, I want the shop menu to remain focused on purchasing transactions (options 1 to 4), so that buying items remains fast and uncluttered.
22. As a Player who bought upgraded armor and an upgraded weapon from the Merchant, I want the inventory command to accurately reflect my new gear names and increased combat statistics, so that I can verify my upgrade investments.

---

## Implementation Decisions

### Modules Modified & Architecture
- **Player Domain Module**:
  - Implement a decoupled presentation method that builds and returns the multi-line inventory string without performing console I/O, allowing complete assertion in automated tests.
  - Implement a console display wrapper that outputs the decoupled string to the standard console.
  - Structure the output strictly into two domain groupings:
    1. Header banner
    2. `Equipped Gear`: Wielded Weapon (with translated name and maximum damage), worn Armor (with translated name and damage reduction, or translated "None" indicator if unarmored).
    3. `Carried Items`: Gold currency balance, Healing Potions count out of maximum belt capacity (`X/3`).
    4. Footer banner

- **Exploration Navigation Module**:
  - Update path choice validation to recognize `i`, `inv`, and `inventory` (case-insensitive and trimmed) as valid navigation commands.
  - Update the crossroads narrative prompt in all four languages to include `Enter 'i' for Inventory.`
  - Update the invalid path error message in all four languages to include `i`.
  - In the crossroads exploration loop, intercept the inventory selection, execute the player's inventory display, and loop back to the path prompt without altering world state, advancing rounds, or triggering travel events.

- **Combat Interaction Module**:
  - Update combat choice validation to recognize `i`, `inv`, and `inventory` as valid battle inputs.
  - Update the combat round prompt in all four languages to accept a dynamic parameter representing current potion count and include `i` for inventory.
  - In the battle action loop, handle the inventory input as a non-turn-advancing free action: print the player inventory and continue the input loop without evaluating Dragon retaliation or round-scoped potion limits.

- **Localization Data Schema**:
  - Add dictionary keys across English, Spanish, French, and Italian dictionaries:
    - `inventory_header`: "===== Inventory ====="
    - `inventory_equipped_header`: "--- Equipped Gear ---"
    - `inventory_carried_header`: "--- Carried Items ---"
    - `inventory_footer`: "====================="
    - `inventory_weapon_format`: "  Weapon: {0} (Max Damage: {1})"
    - `inventory_armor_format`: "  Armor: {0} (Damage Reduction: {1})"
    - `inventory_armor_none`: "  Armor: {0}"
    - `inventory_gold_format`: "  Gold: {0}"
    - `inventory_potions_format`: "  Healing Potions: {0}/3"
  - Update existing dictionary keys across all four languages:
    - `path_prompt_full`: Appended with localized `Enter 'i' for Inventory.`
    - `path_invalid`: Localized with `'i'` included.
    - `attack_prompt`: Updated to format dynamically with `{0}/3 remaining` and `i` for inventory.

---

## Testing Decisions

### Testing Principles
- Test exclusively at public behavioral seams and contracts, avoiding internal implementation details, private helpers, or console stream redirection.
- Assert full localized output generation and input parsing across multiple player states and all supported languages.

### Modules Under Test
- **Player Presentation Seam**:
  - Test `GetInventoryDisplay` with freshly initialized character (starting weapon, 50 Gold, 0 potions, null Armor).
  - Test `GetInventoryDisplay` with equipped Armor and upgraded Weapon after simulated shop purchases.
  - Test `GetInventoryDisplay` with varying potion counts (0/3, 1/3, 3/3).
  - Test `GetInventoryDisplay` across English, Spanish, French, and Italian to verify complete localization parity.
- **Exploration Path Validation Seam**:
  - Test `IsValidPathChoice` asserting `true` for `"i"`, `"I"`, `"inv"`, `"INV"`, `"inventory"`, `"Inventory"`, including leading/trailing whitespace.
  - Test that existing valid path options (`"n"`, `"s"`, `"e"`, `"x"`, etc.) continue to return `true`.
  - Test that invalid entries continue to return `false`.
- **Combat Choice Validation Seam**:
  - Test `IsValidCombatChoice` asserting `true` for `"i"`, `"I"`, `"inv"`, `"inventory"`, with whitespace.
  - Test that existing valid combat options (`"a"`, `"h"`, `"r"`) continue to return `true`.
- **Localization Dictionary Seam**:
  - Test that all newly introduced inventory keys exist in all four language sections of the dictionary and contain non-empty strings.
  - Test that `attack_prompt` supports composite string formatting with the potion count parameter without throwing format exceptions.

### Prior Art
- `PlayerTests.cs`: Prior art for testing character initialization, occupation weapons, and stats.
- `GameTests.cs`: Prior art for validating path choices and combat choices via pure boolean methods.
- `MessagesTests.cs`: Prior art for asserting that dictionary keys exist across all four languages.

---

## Out of Scope

- Dropping, discarding, or selling items.
- Multiple armor slots (e.g., helmets, boots, shields).
- Consuming potions outside of combat directly from the inventory command (potions remain an in-combat recovery item per ADR 0003).
- Inventory capacity limitations for gold or general items (potions remain capped at 3 on the potion belt per ADR 0003).
- Adding inventory inspection submenus inside the Merchant shop dialog (crossroads command is used before/after entering).

---

## Further Notes

- Fully conforms to ADR 0001 (Localization Parity), ADR 0002 (Merchant Encounter & Equipment), ADR 0003 (Healing Potion & Ancient Shrine), and ADR 0004 (Inventory Display System).
- Domain terms align strictly with `GLOSSARY.md`.
