# ADR 0001: Include Italian as a Supported Language

* **Status:** Accepted
* **Date:** 2026-11-09
* **Deciders:** Christian Tanquerido
* **Consulted:** Dr. Sean Humpherys (CIDM 4390)
* **Informed:** Development Team

---

## Context and Problem Statement

The Choose Your Own Adventure (CYOA) console application originally supported three languages: English, Spanish, and French. Internationalization (i18n) and localization (l10n) are core features of the game, implemented through a centralized JSON resource file (`language_data.json`) and managed at runtime via the `Messages` class.

To improve accessibility, broaden user reach, and evaluate the modularity and extensibility of the existing localization architecture, we evaluated whether and how to introduce Italian as a new supported language.

---

## Decision Drivers

1. **User Accessibility & Reach**: Expand the game's audience to Italian-speaking players.
2. **Architecture Validation**: Prove that adding a new language requires zero modifications to core game business logic (dice rolling, character creation logic, turn-based combat calculations).
3. **Maintainability**: Ensure consistent schema structure and translation completeness without introducing technical debt or runtime overhead.
4. **Testability**: Ensure the new language is fully verified by the automated unit test suite (`CYOATests`).

---

## Considered Options

* **Option 1: Add Italian to `language_data.json` and update UI & tests (Chosen)**
* **Option 2: Dynamic runtime translation via third-party API**
* **Option 3: Modularize translations into separate per-language JSON files (`en.json`, `it.json`, etc.)**

---

## Decision Outcome

**Chosen Option:** **Option 1**, because it directly leverages the existing data-driven architecture, requires no external dependencies, preserves zero network latency, and maintains strict backward compatibility.

### Implementation Details:
1. **Data Definition (`language_data.json`)**:
   - Added an `"Italian"` section containing all 70+ required narrative prompts, error strings, victory/defeat texts, dragon encounter prompts, and combat taunts/replies.
   - Configured input mapping for character creation:
     - `raceMap`: `"umano" -> "Human"`, `"elfo" -> "Elf"`, `"nano" -> "Dwarf"`, `"halfling" -> "Halfling"`.
     - `occupationMap`: `"guerriero" -> "Fighter"`, `"mago" -> "Magician"`, `"ladro" -> "Thief"`, `"arciere" -> "Archer"`.
   - Configured localized display maps (`displayRaceMap`, `displayOccupationMap`, `displayWeaponMap`).

2. **Game Interface (`Game.cs`)**:
   - Updated the initial language selection prompt to display `4. Italiano`.
   - Extended the language switch statement to map option `"4"` to `"Italian"`.

3. **Automated Verification (`CYOATests`)**:
   - Added test cases in `MessagesTests.cs` for Italian language selection, dictionary loading, goodbye text, and verification of all required dictionary keys.
   - Added test cases in `GameTests.cs` for win, defeat, and retreat game endings in Italian.

---

## Pros and Cons of the Options

### Option 1: Integrate Italian into existing `language_data.json` (Selected)
* **Positive:** Zero changes to combat, player, or dragon gameplay logic.
* **Positive:** Instantaneous in-memory loading without network calls or API keys.
* **Positive:** Full offline support and zero external points of failure.
* **Negative:** Adding future content requires updating 4 language sections instead of 3.

### Option 2: Dynamic runtime translation via API
* **Positive:** Supports any language dynamically.
* **Negative:** Introduces network latency, API rate limits, costs, and failure points for a local console game.
* **Negative:** Machine translation often produces unnatural dialogue for fantasy gaming narratives.

### Option 3: Separate files per language (`it.json`, `en.json`)
* **Positive:** Isolates language files, allowing smaller file sizes per load.
* **Negative:** Requires refactoring the `Messages.ReadDictionary()` file discovery logic and modifying MSBuild `<Content>` copy tasks across projects, increasing risk of breaking existing tests.

---

## Consequences

* **Positive Consequences:**
  - Italian-speaking users can play the game with fully localized narratives, prompts, character stats, and combat dialogues.
  - Confirmed the robustness of `Messages.cs` and verified that dictionary lookup maps correctly handle case-insensitive Italian input.
  - The project maintains 100% test passing status with enhanced coverage.

* **Negative Consequences & Trade-offs:**
  - `language_data.json` grew from 314 lines to 418 lines (+104 lines).
  - Any future narrative key additions must include valid translations across English, Spanish, French, and Italian.
