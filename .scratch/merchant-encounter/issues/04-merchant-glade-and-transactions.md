# 04: Interactive Merchant Glade & Currency Transactions

**What to build:**
A merchant encounter along the southern mountain trail where players can spend 30 gold to purchase either magical plate armor or an upgraded magical weapon for their occupation. The shop displays player gold and prices, marks bought items as `[SOLD OUT]`, prevents repeat purchases or purchases with insufficient gold, and returns the player to the mountain crossroads upon leaving.

**Blocked by:**
- 01: Player Gold Economy & Character Summary Display
- 02: Protective Armor Equipment & Combat Damage Mitigation
- 03: Occupation-Tailored Magical Weapon Progression

**Status:** ready-for-agent

- [ ] Traveling South from the mountain path crossroads engages the merchant encounter
- [ ] Merchant menu displays player gold and options to buy armor (30 Gold), buy class weapon (30 Gold), or leave
- [ ] Successfully purchasing an item deducts 30 gold, equips the item, and flags the item as sold out
- [ ] Sold-out items display a `[SOLD OUT]` label and reject further purchases with a dedicated notification
- [ ] Attempting to buy an item with less than 30 gold is rejected with an insufficient funds message
- [ ] Starting with 50 gold allows the player to purchase only one upgrade
- [ ] Exiting the shop returns the player to the mountain crossroads
- [ ] Invalid inputs in the shop menu display an error and reprompt
- [ ] Merchant stock state persists across multiple trips South within the same game
- [ ] All merchant dialogue, menu prompts, sold-out tags, and error strings are localized across English, Spanish, French, and Italian
- [ ] Unit tests verify purchase transactions, gold deductions, sold-out state, and menu input validation
