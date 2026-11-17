# 05: Crossroads Dragon Retreat & Merchant Re-engagement Loop

**What to build:**
A non-terminating retreat loop from the dragon encounter in the North. When a player retreats from the dragon, they are returned to the mountain crossroads instead of ending the game, allowing them to travel South to visit the merchant and buy equipment before re-attempting the North path.

**Blocked by:**
- 04: Interactive Merchant Glade & Currency Transactions

**Status:** ready-for-agent

- [ ] Retreating from the dragon before combat returns the player to the crossroads loop
- [ ] Retreating from the dragon during combat returns the player to the crossroads loop
- [ ] Player can travel South to the merchant after retreating from the North
- [ ] Defeating the dragon or player death in combat still terminates the game normally
- [ ] Merchant inventory remains persistent when visited after a combat retreat
- [ ] Unit tests verify game loop state and retreat flow behavior
