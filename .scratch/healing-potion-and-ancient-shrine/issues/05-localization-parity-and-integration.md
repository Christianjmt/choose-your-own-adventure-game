# 05: Localization Parity & Integration Verification (ADR 0001)

**What to build:** All user-facing strings across all new features—including Ancient Shrine narrative, roll outcome messages, sealed vault notices, merchant potion dialogue, belt capacity errors, combat heal prompts, full-health warnings, and combat recovery logs—are localized with 100% parity across English (`en`), Spanish (`es`), French (`fr`), and Italian (`it`) in `language_data.json`. Unit tests verify that every single key exists and is non-empty across all 4 languages, and the entire test suite passes cleanly with zero build errors.

**Blocked by:** 01: Player Healing Domain & Potion Belt Capacity, 02: Merchant Potion Transactions & Capacity Guard, 03: In-Combat Free-Action Healing & ASCII Visuals, 04: Ancient Shrine Discovery Trial & Retreat Synergy

**Status:** completed

- [x] All new shrine, merchant, and combat message keys added to English section of `language_data.json`
- [x] Complete Spanish translations added to Spanish section of `language_data.json`
- [x] Complete French translations added to French section of `language_data.json`
- [x] Complete Italian translations added to Italian section of `language_data.json`
- [x] `Messages.cs` provides helper methods/translations where needed
- [x] Data-driven unit tests in `CYOATests/MessagesTests.cs` assert all new dictionary keys are populated in all 4 languages
- [x] Full solution build succeeds with 0 errors and 0 warnings
- [x] All unit tests pass cleanly
