# Specification: Healing Potion Consumable and Ancient Shrine System

**Triage Label:** `ready-for-agent`  
**Governing ADRs:** ADR 0001 (Localization Parity), ADR 0002 (Merchant Encounter & Equipment), ADR 0003 (Healing Potion & Ancient Shrine)  
**Domain Vocabulary:** Defined in `GLOSSARY.md` (Healing Potion, Maximum Health Points, Ancient Shrine, Merchant, Dragon)

---

## Problem Statement

During encounters with the Dragon, the Player takes substantial incoming damage with no recovery mechanisms. Combat is an uncompromising battle of attrition: any damage taken remains permanent until defeat or retreat. Furthermore, while the southern path offers commercial progression via the Merchant, the mountain crossroads lacks exploration variety, offering only North (danger) and South (commerce). Players desire an in-combat recovery tool and richer world exploration that synergizes with the retreat mechanic.

---

## Solution

Introduce a consumable **Healing Potion** item that fully restores the Player's life force to their **Maximum Health Points**. The Player can acquire potions through two distinct channels:
1. **Commerce**: Purchased from the Merchant in the southern glade for 10 Gold (unlimited supply up to a carrying capacity of 3 potions).
2. **Exploration**: Discovered at an **Ancient Shrine** along a new eastern path from the crossroads via a 20-sided die trial (rolling 12 or higher). If the trial is failed, the shrine seals shut, reopening only after the Player confronts the Dragon and retreats.

In combat, drinking a Healing Potion is a tactical **free action** that instantly restores full health and leaves the Player at the action prompt to execute an attack or retreat within the same round, capped at one potion per combat round.

---

## User Stories

1. As a Player creating a new character, I want my initial rolled health recorded as my Maximum Health Points, so that subsequent healing effects restore me accurately without exceeding my initial vitality.
2. As a Player creating a new character, I want to start my journey with an empty potion belt (0 Healing Potions), so that acquiring potions feels like a deliberate tactical investment.
3. As a Player viewing my character stats, I want to see my current potion belt capacity (e.g., 0/3, 1/3, 3/3), so that I know how many emergency supplies I am carrying.
4. As a Player standing at the mountain crossroads, I want to see navigation options for North (Mountain Peak), South (Merchant's Glade), East (Ancient Shrine), and Exit, so that I can explore new territories before facing the Dragon.
5. As a Player traveling East to the Ancient Shrine for the first time, I want to attempt a trial of fortune by rolling a 20-sided die, so that I have a chance to claim a free Healing Potion.
6. As a Player at the Ancient Shrine rolling 12 or higher, I want to successfully retrieve the Healing Potion with celebratory visual art, so that I feel rewarded for exploration.
7. As a Player at the Ancient Shrine who succeeds on the roll while already carrying 3 potions, I want the game to inform me that my belt is full and leave the potion safely on the altar, so that I can return to claim it after using a potion.
8. As a Player at the Ancient Shrine rolling below 12, I want to suffer no physical damage while watching the shrine's stone vault seal shut, so that failure feels tense without being immediately fatal.
9. As a Player who returns to the Ancient Shrine while it is sealed, I want to see that the sanctuary remains closed, so that I cannot farm repeated rolls without engaging the rest of the world.
10. As a Player who was previously turned away by the sealed Ancient Shrine, I want the shrine to unseal and allow a fresh trial attempt after I retreat from the Dragon, so that retreating provides a strategic recovery reward.
11. As a Player visiting the Merchant in the southern glade, I want to see Healing Potion listed on the shop menu for 10 Gold, so that I can choose to purchase consumables with my starting gold.
12. As a Player with at least 10 Gold and fewer than 3 potions, I want purchasing a Healing Potion from the Merchant to deduct 10 Gold and add 1 potion to my belt, so that my transaction completes cleanly.
13. As a Player with fewer than 10 Gold attempting to buy a potion, I want the Merchant to refuse the sale with a polite explanation, so that I cannot enter negative debt.
14. As a Player with 3 potions attempting to buy a potion, I want the Merchant to refuse the sale because my potion belt is full, so that I do not waste gold on items I cannot carry.
15. As a Player in the Merchant's shop, I want the option to leave the shop and return to the mountain crossroads at any time, so that I retain full control of my movements.
16. As a Player engaged in combat with the Dragon, I want to see my current potion belt capacity in the combat summary header, so that I can make informed survival choices.
17. As a Player in active combat, I want to see an option to Heal alongside Attack and Retreat, so that I know I have the choice to use an emergency draught.
18. As an injured Player choosing to Heal during combat, I want my health to immediately recover to 100% of my Maximum Health Points, so that I can survive lethal incoming attacks.
19. As a Player who drinks a potion in combat, I want the action to be a free action that returns me directly to the action prompt, so that I can still choose to attack or retreat during the current round.
20. As a Player in combat, I want consumption restricted to at most one potion per round, so that healing requires deliberate timing.
21. As a Player at full health who accidentally inputs Heal in combat, I want the game to warn me that I am already at full health and keep my potion intact, so that I do not waste valuable consumables.
22. As a Player with 0 potions who inputs Heal in combat, I want the game to inform me that my belt is empty without consuming my turn, so that I can choose another action.
23. As a Player who drinks a potion in combat, I want to see a potion flask illustration, so that the moment feels visually distinct and rewarding.
24. As an international Player playing in Spanish, French, or Italian, I want all shrine descriptions, merchant dialogue, potion names, combat logs, and warning messages fully translated, so that the game experience remains completely localized.

---

## Implementation Decisions

### Domain & State Management
- **Maximum Health Points Representation**: The player state tracks an immutable maximum health value established during initial character creation attribute rolling. This value serves as the hard ceiling for all healing logic.
- **Potion Belt Tracking**: The player state tracks an integer count of carried potions, constrained to the inclusive range [0, 3]. Starting count is 0.
- **Consumption Contract**: The player domain exposes a pure consumption action that validates two prerequisites: potion count $> 0$ and current health $<$ maximum health. Upon validation, health is set equal to maximum health, potion count decrements by 1, and success is signaled. If either check fails, state is unchanged and specific failure reasons are returned.
- **Crossroads Map Expansion**: The world navigation loop adds the East cardinal path. Navigating East triggers the Ancient Shrine encounter; completing or leaving the shrine safely returns the player to the mountain crossroads.
- **Ancient Shrine State**: The game loop tracks two persistence flags: whether the single hidden potion has been claimed, and whether the shrine vault is currently sealed.
  - If claimed: visiting displays an empty altar and returns to crossroads.
  - If sealed: visiting displays a locked stone vault and returns to crossroads.
  - If unsealed and unclaimed: the player executes a 20-sided die roll. A roll of 12 or higher claims the potion (if inventory space exists). A roll below 12 transitions the shrine to sealed.
- **Retreat Synergy**: When the player elects to retreat from combat with the Dragon, any sealed state on the Ancient Shrine is cleared, reopening the trial.
- **Merchant Shop Restructuring**: The merchant interaction presents four explicit numbered selections: Buy Armor, Buy Upgraded Weapon, Buy Healing Potion (10 Gold), and Leave Shop. The transaction verifies available gold ($\ge 10$) and potion capacity ($< 3$). Stock is unlimited.
- **Combat Flow & Free Action**: The combat action prompt offers Attack, Heal, and Retreat. Executing Heal performs the consumption contract. If successful, health is replenished, visual art and narrative recovery text are rendered, and the prompt re-appears to receive the round's tactical decision (Attack or Retreat). A round-scoped flag prevents multiple potion drinks within the same combat exchange.
- **Localization Parity (ADR 0001)**: All user-facing strings across all branches (shrine narrative, success, failure, sealed, merchant prompts, capacity rejections, combat prompts, heal actions, full-health warnings, empty-belt warnings) are localized across English, Spanish, French, and Italian dictionaries.

---

## Testing Decisions

### Testing Principles
- Tests must exclusively evaluate observable external behavior, state mutations, and returned contracts, never internal helper methods or private fields.
- Console input/output must not be directly asserted through stream interception where pure domain and transactional seams exist.

### Modules Under Test
- **Player Healing Domain**:
  - Verification that character creation locks Maximum Health Points to the initial roll.
  - Verification that consuming a potion restores damaged health to exactly maximum health.
  - Verification that consuming a potion decrements inventory count by 1.
  - Verification that potion consumption is rejected and count preserved when health is already at maximum.
  - Verification that potion consumption is rejected when potion count is 0.
- **Merchant Shop Transactions**:
  - Verification that purchasing with sufficient funds and open capacity increments potion count by 1 and deducts 10 Gold.
  - Verification that purchasing with insufficient funds ($< 10$ Gold) leaves funds and inventory untouched.
  - Verification that purchasing with full capacity (3 potions) leaves funds and inventory untouched.
  - Verification that merchant choice validation recognizes all valid menu inputs (1, 2, 3, 4).
- **Ancient Shrine & World Flow**:
  - Verification that a d20 roll $\ge 12$ marks the shrine claimed and grants a potion.
  - Verification that a d20 roll $\ge 12$ with full inventory leaves the shrine unclaimed and unsealed.
  - Verification that a d20 roll $< 12$ marks the shrine sealed and grants 0 potions.
  - Verification that retreating from the Dragon unseals a previously sealed shrine.
  - Verification that crossroads path validation recognizes East ("e", "east").
- **Combat Input & Status Presentation**:
  - Verification that combat choice validation recognizes Heal ("h", "heal").
  - Verification that combat display formatting includes localized potion capacity (`Healing Potions: X/3`).
- **Localization Dictionary Coverage**:
  - Verification that all new localization keys exist and contain non-empty strings across English, Spanish, French, and Italian.

### Prior Art
- Test suites in `CYOATests/MerchantTests.cs`, `CYOATests/CombatTests.cs`, and `CYOATests/MessagesTests.cs` serve as direct structural precedents.

---

## Out of Scope

- Consuming potions out-of-combat while at the mountain crossroads or in the merchant's glade.
- Other potion varieties (e.g., strength-buffing elixirs, agility potions, poisons).
- Selling items or potions back to the Merchant for gold.
- Earning potions as post-combat loot drops from defeating the Dragon.
- Increasing potion belt carrying capacity beyond 3.

---

## Further Notes

- The starting economy of 50 Gold gives players a balanced tactical opening: they may buy 1 piece of equipment (Armor or Weapon for 30 Gold) and up to 2 Healing Potions (20 Gold), or save their gold while attempting the free East shrine trial first.
