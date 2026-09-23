# Garrison Drills Message Enhancer

A tiny Harmony extension for `GarrisonDrillsPinned` (see `../PinnedMods/GarrisonDrills/`) that adds XP-per-soldier and total-XP info to every pulse notification.

Vanilla GarrisonDrills v1.1.0 pulse message: `{TIER} training: experience gained by {COUNT} soldiers (-{COST}{GOLD_ICON}).` — omits how much XP was granted.

This mod: `{TIER} training: {COUNT} soldiers each gained +{XP_PER} xp (total +{XP_TOTAL} xp, -{COST}{GOLD_ICON}).`

## Design

- **Prefix `GarrisonDrills.GarrisonDrillsBehavior.DoPulse` returning false + full reimplementation** — cleanest for a small method; uses `AccessTools` reflection to call the target's private helpers (`CountTrainableSoldiers`, `CurrentTier`, `ApplyTraining`, `StopTraining`) so game-logic branch parity is preserved without duplication
- **Own localization key `GDMsgEnh_Pulse`** in this mod's own `ModuleData/Languages/` — does NOT modify Pinned's XML, so if the Harmony patch fails to load, the original DoPulse runs and shows the un-enhanced message (graceful degradation, no `{XP_PER}` literals visible)
- **Load order**: after `Bannerlord.Harmony` and `GarrisonDrillsPinned` (declared via `DependedModuleMetadata` order=LoadBeforeThis)

## Build + deploy

```powershell
.\deploy.ps1                                                          # default GameRoot = E:\SteamLibrary\...
.\deploy.ps1 -GameRoot "D:\SteamLibrary\...\Mount & Blade II Bannerlord"  # second device
```

Targets `Modules\GarrisonDrillsMessageEnhancer\`.

## When Pinned baseline changes

If you rebase `PinnedMods\GarrisonDrills\` to a newer workshop version and any of these private-member signatures shift, the reflection lookups in `DoPulsePatch.TargetMethod` will throw at first game load. Signs it's stale:

- `CountTrainableSoldiers` / `CurrentTier` / `ApplyTraining` / `StopTraining` renamed or resignatured
- Nested `TrainTier` class renamed or its `Xp`/`Gold`/`NameKey` fields renamed

Fix by re-decompiling the new Pinned DLL (`ilspycmd -t GarrisonDrills.GarrisonDrillsBehavior`) and adjusting `AccessTools` lookups.
