# Specification: Merchant Encounter and Equipment Progression

## Problem Statement

When playing Choose Your Own Adventure, traveling South along the mountain path leads to an uneventful dead-end narrative where nothing happens and the player returns home empty-handed. In addition, when confronting the dragon in the North, players have no progression mechanics, gear customization, or defensive mitigation options; they are forced to fight with starting gear and rely purely on basic agility rolls to avoid damage. This eliminates strategic decision-making and limits player agency.

## Solution

Introduce an interactive Merchant encounter along the South path where players can spend their starting gold on either protective magical armor or an upgraded occupation-tailored magical weapon. Purchasing armor introduces a damage reduction mechanic in combat, while purchasing a weapon increases damage output. The South path seamlessly loops back to the mountain crossroads, empowering players to strategize and upgrade before venturing North to battle the dragon. The encounter fully supports all four localized languages (English, Spanish, French, Italian) and maintains persistent stock so purchased items cannot be duplicated.

## User Stories

1. As a player, I want to meet a merchant when traveling South, so that the southern mountain trail provides an active gameplay encounter rather than a dead-end message.
2. As a player, I want to start my adventure with 50 Gold, so that I have currency to purchase items from the merchant.
3. As a player, I want to see my current Gold displayed when visiting the merchant, so that I know what I can afford before making a purchase.
4. As a player, I want the merchant to offer Magical Plate Armor for 30 Gold, so that I can invest in defensive survivability.
5. As a player, I want the merchant to offer an upgraded Magical Weapon for 30 Gold tailored to my occupation, so that I can invest in offensive capability matching my class fantasy.
6. As a Fighter, I want the merchant to offer a "flaming broadsword" with higher damage and unique ASCII art, so that my sword attacks deal more damage against the dragon.
7. As a Magician, I want the merchant to offer a "meteor strike spell" with higher damage and unique ASCII art, so that my spells are more potent in combat.
8. As a Thief, I want the merchant to offer a "shadowblade dagger" with higher damage and unique ASCII art, so that my strikes deal greater damage.
9. As an Archer, I want the merchant to offer an "elven composite bow" with higher damage and unique ASCII art, so that my archery strikes hit harder.
10. As a player, I want item prices to be balanced such that 50 Gold allows only one upgrade, so that I am faced with a meaningful tactical choice between offense and defense.
11. As a player, I want to see a clear "[SOLD OUT]" indicator on any item I have already purchased, so that I understand the merchant's stock is limited.
12. As a player, I want the merchant to inform me if I try to purchase a sold-out item, so that I know my gold will not be deducted and the transaction is rejected.
13. As a player, I want the merchant to inform me if I attempt a purchase with insufficient gold, so that I understand why the transaction failed.
14. As a player, I want the merchant menu to provide an exit option, so that I can leave the shop without making a purchase if I choose.
15. As a player, I want invalid inputs in the shop menu to display an error and reprompt, so that mistyping does not crash or exit the game unexpectedly.
16. As a player, I want to be returned to the mountain crossroads after leaving the merchant, so that I can choose to travel North to confront the dragon.
17. As a player, I want equipped armor to reduce incoming damage from the dragon by 4 points, so that I survive longer in combat.
18. As a player, I want combat damage to never drop below 1, so that dragon attacks remain dangerous even when heavily armored.
19. As a player, I want the combat log to explicitly narrate when my armor absorbs damage, so that I receive immediate feedback on my equipment's effectiveness.
20. As a player, I want to see my current Gold and equipped Armor reflected in my character summary, so that I can inspect my complete character status.
21. As a player, I want to see my current Gold and equipped Armor reflected in the combat status screen, so that I can monitor my defensive rating during battle.
22. As a player who retreated from combat with the dragon, I want to return to the crossroads and travel South to the merchant, so that I can upgrade my equipment before re-engaging.
23. As an international player choosing Spanish, French, or Italian, I want all merchant dialogue, item names, error prompts, and combat absorption text to appear in my selected language, so that the experience is fully localized and immersive.

## Implementation Decisions

- **Domain Models & Modules**:
  - A dedicated `Armor` domain module encapsulates armor attributes including item type name, flat damage reduction value, and ASCII art.
  - The `Player` entity is extended to track `Gold` (defaulting to 50) and an optional `Armor` reference.
  - A dedicated `Merchant` domain module manages shop inventory state (`ArmorSold`, `WeaponSold`), item pricing (fixed at 30 Gold), weapon generation based on player occupation, and transaction execution.
  - `Game` orchestrates the persistence of the merchant across the adventure loop and routes South path navigation into the shop loop.
- **Combat Resolution**:
  - Dragon attacks incorporate defensive mitigation during the damage resolution phase:
    $$\text{damage} = \max(1, \text{rawDamage} - \text{player.Armor.DamageReduction})$$
  - The combat log includes a dedicated narrative line when armor absorbs incoming damage.
  - Combat status display presents Player Gold and Armor details alongside existing combat attributes.
- **Navigation & Game Loop**:
  - Traveling South engages the merchant encounter loop and subsequently returns the player to the mountain path crossroads.
  - Dragon combat retreat returns the player to the crossroads, permitting subsequent travel to the South path while keeping merchant stock persistent.
- **Localization Integration**:
  - Full adherence to ADR 0001: centralized storage in `language_data.json` across English, Spanish, French, and Italian.
  - Dictionary keys added for merchant greetings, menu prompts, item purchase confirmations, insufficient funds errors, sold-out indicators, and armor absorption messages.
  - Introduction of a localized display map for armor types (`displayArmorMap`) and extensions to `displayWeaponMap` for all upgraded weapon variants.

## Testing Decisions

- **Testing Principles**:
  - Tests verify external behavior, invariants, and public state contracts rather than private console I/O implementation details.
- **Seams for Testing**:
  1. *Merchant Transaction Seam*: Verify `BuyArmor` and `BuyWeapon` directly through public methods on `Merchant`. Validate initial stock, successful purchases, gold deductions, sold-out rejections, and insufficient fund rejections.
  2. *Combat Damage Mitigation Seam*: Verify `Dragon.Attack` against a player equipped with armor to ensure damage is reduced by the specified rating, damage floor ($\ge 1$) is strictly enforced, and absorption messages are logged.
  3. *Localization Lookup Seam*: Verify through `Messages` that every new dictionary key and translation entry exists and resolves non-empty strings without fallback brackets across English, Spanish, French, and Italian.
  4. *Input Validation Seam*: Verify `Merchant.IsValidChoice` against valid choices ("1", "2", "3") and invalid inputs (empty, null, out-of-range).
- **Prior Art**:
  - Directly follows patterns established in `CombatTests.cs`, `DragonTests.cs`, and `MessagesTests.cs`.

## Out of Scope

- Selling starting weapons or trading items back to the merchant for gold.
- Consumable items such as health potions or temporary stat buffs.
- Multiple tiers of armor or secondary weapon slots.
- Persistent save files across multiple game executions.
- Random loot drops from defeated enemies.

## Further Notes

- Documented as ADR 0002 (`0002-merchant-encounter-and-equipment-system.md`) in `docs/adr/`.
- All 129 automated tests in `CYOATests` pass with 100% success rate.
