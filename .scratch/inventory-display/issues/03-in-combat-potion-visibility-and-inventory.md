# 03: In-Combat Dynamic Potion Visibility & Free-Action Inventory Inspection

**What to build:** During combat with the Dragon, the round action prompt dynamically displays the player's remaining Healing Potions count out of capacity (`{0}/3 remaining`) and includes `'i' for inventory` across English, Spanish, French, and Italian dictionaries. Players can input `'i'`, `'inv'`, or `'inventory'` as a free action to view their complete inventory. Inspecting the inventory during combat does not spend the player's turn, does not advance the round counter, does not trigger a Dragon retaliation attack, and re-prompts the player for their combat choice. Combat choice validation is updated to accept the inventory command.

**Blocked by:** 01: Player Inventory Domain & Decoupled Presentation

**Status:** completed

- [x] Combat choice validation recognizes `'i'`, `'inv'`, and `'inventory'` (case-insensitive with leading/trailing whitespace trimmed) as valid combat inputs
- [x] Existing valid combat choices (`'a'`, `'h'`, `'r'`) and invalid inputs continue to be validated correctly
- [x] The combat action prompt across English, Spanish, French, and Italian dictionaries dynamically formats with the remaining potions count and includes `'i' for inventory`
- [x] In combat, entering the inventory command displays the player's inventory as a free action without consuming the turn or allowing the Dragon to attack
- [x] After displaying the inventory in combat, the battle prompt immediately reappears to accept an attack, heal, retreat, or subsequent inventory check
- [x] If a potion has already been used in the current round, the inventory command remains available as a free action
- [x] Automated unit tests in the test suite assert combat choice validation and verify composite string formatting for the dynamic combat prompt in all supported languages
