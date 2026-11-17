# 01: Player Inventory Domain & Decoupled Presentation

**What to build:** The Player domain provides decoupled inventory string generation and console display. It clearly groups possessions into two distinct categories: **Equipped Gear** (displaying the wielded Weapon name and Maximum Damage, along with the worn Armor name and Damage Reduction, or "None" if unarmored) and **Carried Items** (displaying the current Gold balance and Healing Potions count out of maximum capacity `3`). All titles, section headers, row formats, and empty slot indicators are localized across English, Spanish, French, and Italian dictionaries. Unit tests verify decoupled string formatting for new characters, upgraded characters, varying potion counts, and all four languages without console redirection.

**Blocked by:** None (can start immediately)

**Status:** completed

- [x] Dictionary keys for inventory headers, section dividers, row format strings, and empty armor indicators exist and are localized across English, Spanish, French, and Italian in the language schema
- [x] A decoupled inventory presentation method builds and returns a formatted multi-line inventory string matching the two-section layout (Equipped Gear and Carried Items)
- [x] An unarmored character displays the localized string for "None" in the Armor slot
- [x] An armored character displays the translated armor name and its damage reduction value
- [x] The wielded weapon displays the translated weapon name and its maximum damage value
- [x] The Carried Items section displays the current Gold balance and Healing Potions count out of capacity (`X/3`)
- [x] A console display method writes the decoupled inventory string to the console
- [x] Automated unit tests in the test suite assert inventory string output across multiple character configurations and all four supported languages without console stream redirection
