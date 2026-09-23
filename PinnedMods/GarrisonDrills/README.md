# GarrisonDrills (Pinned)

Local pinned copy of workshop mod 3735834360 (Garrison Drills v1.1.0) with the +20/+60/+100 XP byte-patch + matching UI strings baked in. Deployed as a separate module Id `GarrisonDrillsPinned` so the workshop original can stay subscribed for update detection but not loaded.

## Why pinned

Workshop auto-updates repeatedly reverted the DLL byte-patch and the root-level `std_GarrisonDrills_strings.xml` (see ModdingJournal modification #3 and follow-up 2026-09-22). Pinning the mod under `Modules\GarrisonDrillsPinned\` moves the loaded copy outside Steam's workshop directory, so subscription updates no longer touch it.

## Layout

- `bin\Win64_Shipping_Client\GarrisonDrills.dll` — byte-patched (2332/2348/2365 = 0x14/0x3C/0x64)
- `ModuleData\Languages\std_GarrisonDrills_strings.xml` — root/English fallback, +20/+60/+100
- `ModuleData\Languages\EN\std_GarrisonDrills_strings_en.xml` — English, +20/+60/+100
- `ModuleData\Languages\CNs\std_GarrisonDrills_strings_cns.xml` — Simplified Chinese, +20/+60/+100
- `SubModule.xml` — Id renamed to `GarrisonDrillsPinned`, Name suffixed "(Pinned)"
- `baseline.json` — records workshop version + hash at pin time, patch inventory, rebase notes

## Deploy

```powershell
.\deploy.ps1                                                       # default GameRoot = E:\SteamLibrary\...
.\deploy.ps1 -GameRoot "D:\SteamLibrary\...\Mount & Blade II Bannerlord"  # second device
```

Deploys to `<GameRoot>\Modules\GarrisonDrillsPinned\` and verifies byte-patch. Also excludes `deploy.ps1`, `README.md`, `baseline.json` from the deployed copy (dev-only files).

## Rebase when workshop bumps version

`Tools\check-pinned-mods.ps1` diffs current workshop DLL hash against `baseline.json.workshop_dll_sha256_at_pin`. On mismatch:

1. Inspect new workshop version — did author change XP constants or the training tier logic? Read `bin\Win64_Shipping_Client\GarrisonDrills.dll` via `ilspycmd -t GarrisonDrills.*` to find the current byte offsets of the three `ldc.i4.s` operands
2. Re-copy workshop tree over PinnedMods
3. Re-apply patches (or automate via a `rebase.ps1` per patch)
4. Update `baseline.json`
5. `deploy.ps1`
6. Journal entry
