# 02: Protective Armor Equipment & Combat Damage Mitigation

**What to build:**
Protective magical plate armor that absorbs incoming damage in combat. When equipped, dragon attacks are mitigated by 4 damage reduction (with damage strictly floored at 1), the combat log explicitly narratively confirms armor absorption, and equipped armor and damage reduction rating are displayed in character summaries and combat screens across all 4 languages.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [ ] Armor domain object models armor type ("magical plate armor"), damage reduction rating (4), and ASCII art (`[|###|]`)
- [ ] Dragon attacks hitting an armored player are reduced by the armor's damage reduction value
- [ ] Damage dealt to player is floored at a minimum of 1 damage regardless of armor rating
- [ ] Combat log displays a localized message when armor absorbs damage
- [ ] Character summary and combat status screens display equipped armor name and damage reduction rating (or "None" if unequipped)
- [ ] All armor strings, labels, and display mappings are localized across English, Spanish, French, and Italian
- [ ] Unit tests verify damage mitigation, the damage floor invariant, and display translations
