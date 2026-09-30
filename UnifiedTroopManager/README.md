# Unified Troop Manager

A Bannerlord mod that unifies pre-battle roster selection with per-troop formation
assignment. It replaces the CYT (ChooseYourTroops) + SSYF (Stop Shuffling You Fools /
FormationManager) combo with a single mod so their assumptions do not fight each other.

## Why this exists

Running CYT + SSYF together triggers a real bug: SSYF's `OrderOfBattleVMInitializePatch`
forcibly overwrites the Order-of-Battle slot classes and locks the weight sliders. As a
result, any troop the player selected in CYT that was **not** covered by an SSYF plan
gets shoved out of the spawn wave. In-game symptom: you chose 5 crossbowmen + 10
legionaries, only the legionaries show up.

Because CYT and SSYF do not know about each other and neither exposes an API to fix it
from the outside, patching one to be aware of the other is fragile. Merging their
responsibilities into a single data model and a single set of Harmony patches removes the
class of bug entirely.

## Status

- Phase 0: design frozen at DESIGN.md v0.3 (2026-09-28)
- **Phase 1 (this commit): scaffolding only, no gameplay changes yet**
- Phase 2-4: roster picker, formation plans, OoB fix (bug fix acceptance test)
- Phase 7: full scenario pass (Field / Siege / Hideout / Lord's Hall)

## Dependencies

- Native / SandBoxCore / Sandbox / StoryMode (v1.4.7)
- Bannerlord.Harmony (>= 2.4.2)
- Bannerlord.ButterLib (>= 2.12)
- Bannerlord.UIExtenderEx (>= 2.13)
- Bannerlord.MBOptionScreen / MCM v5 (>= 5.12)

## Attribution

This mod is a **clean-room reimplementation**. It shares no source code with the mods
that inspired it. The design draws on ideas from:

- **ChooseYourTroops** (Steam Workshop ID 2957211804) — the concept of a per-battle
  roster picker driving `MapEventSide.AllocateTroops`.
- **Stop Shuffling You Fools / FormationManager** (Nexus 11869) — the concept of a
  persisted per-troop formation plan driving `Mission.SpawnTroop`.
- **TroopClassifier** (Nexus 12104) — role classification (planned for Phase 5+).

None of the above mods ship a LICENSE file so their code is treated as All Rights
Reserved. Only publicly-observable Bannerlord API surface was used as a reference; no
member names, variable names, or code fragments were copied.

## Build + deploy

```powershell
# From this folder
./deploy.ps1                                 # builds Release + copies to E:\SteamLibrary\...
./deploy.ps1 -Config Debug                   # debug build
./deploy.ps1 -GameRoot 'D:\Games\Bannerlord' # alternate game install
```

Environment variables `$env:BannerlordBin` and `$env:WorkshopRoot` override the default
E:-drive paths inside the .csproj so cross-machine builds keep working without touching
the repo files.
