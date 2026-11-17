# 02: Mountain Crossroads Inventory Navigation & Exploration Loop

**What to build:** While exploring the Mountain Crossroads, the player can enter `'i'`, `'inv'`, or `'inventory'` (case-insensitive, trimmed) to inspect their inventory. The narrative crossroads prompt explicitly lists the inventory command across English, Spanish, French, and Italian dictionaries. Entering the inventory command displays the player's inventory and immediately re-prompts for a crossroads direction without advancing game state, spending turns, or triggering travel events. Entering an invalid input displays a localized error message that lists `'i'` among valid options.

**Blocked by:** 01: Player Inventory Domain & Decoupled Presentation

**Status:** completed

- [x] Path validation recognizes `'i'`, `'inv'`, and `'inventory'` (case-insensitive with leading/trailing whitespace trimmed) as valid navigation choices
- [x] Existing valid path choices (`'n'`, `'s'`, `'e'`, `'x'`) and invalid inputs continue to be validated correctly
- [x] The full crossroads narrative prompt in English, Spanish, French, and Italian includes the instruction to enter `'i'` for Inventory
- [x] The invalid choice error message in English, Spanish, French, and Italian includes `'i'` among the accepted choices
- [x] At the Mountain Crossroads, entering the inventory command displays the player's inventory and loops back to the crossroads prompt without advancing game state or triggering path transitions
- [x] Automated unit tests in the test suite assert path validation behavior for all forms of the inventory command alongside existing cardinal directions
