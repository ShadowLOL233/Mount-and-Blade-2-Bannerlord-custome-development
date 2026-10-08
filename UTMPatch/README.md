# UTMPatch

Thin Harmony patch mod. Fixes the single-point OoB conflict between **ChooseYourTroops (CYT)** and **FormationManager / Stop Shuffling You Fools (FM)** on Bannerlord v1.4.7.

## Problem

FM's `OrderOfBattleVMInitializePatch.Postfix` writes `Classes[0].Class = <planned>` + `Classes[1].Class = NumberOfAllFormations` on each formation slot, then calls `OobWeightDistributor.LockManagedSliders()`. This triple-chain squeezes out any troop class that doesn't have an active FM plan — including troops CYT just picked for the battle.

Result: a battle where the player wanted a mixed roster but only sees FM-planned classes in Order of Battle.

## What UTMPatch does

`OrderOfBattleVM.Initialize` Postfix (priority `Last`, after FM):

- Reset `Classes[0].Class` + `Classes[1].Class` back to `NumberOfAllFormations` on every slot where FM wrote a specific class. This re-opens the slot's class pool.
- (Optional) Unlock the sliders FM locked.

FM's actual Formation assignment at spawn (`MissionAgentSpawnPatch.Postfix` + `FormationAssignmentResolver`) is **not touched** — FM's core feature keeps working. CYT's roster filter (`MapEventSide.AllocateTroops` Prefix) is **not touched** — CYT keeps working.

## Not touched (by design)

- FM `MissionAgentSpawnPatch` — spawn-time Formation assignment (FM's real feature)
- FM `OobPreviewAssignmentApplier.Apply` — benign preview placement
- FM `OobWeightDistributor.DistributeWeights` — benign weight calculation (just visual)
- CYT any patch — CYT's filter is correct and complete
- vanilla OoB — anything not touched by FM stays vanilla

## Install

1. `deploy.ps1`
2. Enable `UTM Patch (FM × CYT bridge)` in Bannerlord launcher after FormationManager and ChooseYourTroops.

Harmony-only. No MCM. No UI. No save data.
