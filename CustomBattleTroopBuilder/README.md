# CustomBattleTroopBuilder (CBTB)

In-game troop builder for Bannerlord **Custom Battle** mode. Clone any CharacterObject, edit its equipment / skills / body properties, save as JSON, and the custom troop appears in Custom Battle roster for selection.

## Status

**Phase 1 scaffold** (2026-10-12) · OnSubModuleLoad + Ctrl+Shift+B hotkey smoke test. No UI yet.

## Scope (planned · ~2-3 weeks across sessions)

| Phase | Deliverable |
|---|---|
| 1 (this session) | Scaffold · SubModule.xml + csproj + Main.cs + deploy.ps1 · Harmony instance ready · hotkey smoke test |
| 2 | Harmony patch `CustomBattleMenuVM` + add "Troop Builder" button · placeholder editor screen |
| 3 | Base troop picker (full CharacterObject.All + search filter) · clone + rename |
| 4 | Equipment 10-slot picker (filter by ItemObject.All) · RBM tooltip reuse |
| 5 | Skill 20+ edit panel · body props · culture custom |
| 6 | Save/load JSON preset · inject custom troops to CustomBattle roster |
| 7 | Journal + REPLICATE archive · end-to-end validation |

## Install (dev · from this repo)

```powershell
.\deploy.ps1
# Launcher -> enable 'Custom Battle Troop Builder' -> Play
```

## Depends

- Harmony v2.4.2
- ButterLib v2.12.0
- Native / SandBoxCore / Sandbox / CustomBattle (vanilla)

## Logs

`C:\ProgramData\Mount and Blade II Bannerlord\logs\CustomBattleTroopBuilder.log`

## Design decisions

- **Full CharacterObject pool** (vanilla + mod) · search filter in UI
- **Not Campaign-tied** · pure CustomBattle sandbox · doesn't touch Clan/Hero/Retinues state
- **Harmony + full Gauntlet screen** (not UIExtender mixin) · own UI prefab
- **JSON persistence** at `Modules\CustomBattleTroopBuilder\CustomTroops\*.json`
