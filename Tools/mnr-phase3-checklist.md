# MNR × PlayerSettlement Phase 3 · new-save verification checklist

Living checklist for when you decide to enable MNR (`DefaultModule=true` + `IsSelected=true`) and start a new campaign to actually test `PSCacheWarmup v0.1.1` + MNR × PS coexistence in the running game.

Two companion scripts (this same `Tools/` dir):
- `mnr-toggle.ps1 -Enable | -Disable | -Status`
- `mnr-phase3-verify.ps1` — parses `Configs\ModLogs\PSCacheWarmup.log` after the session

## Pre-flight (before enabling MNR)

- [ ] **Back up saves**. MNR replaces the map + adds 597 settlements + 7 kingdoms + 52 factions. Existing vanilla-map saves will not run under MNR. Copy `E:\Bannerlord-UserData\Mount and Blade II Bannerlord\Game Saves\` somewhere safe.
- [ ] **Steam Cloud** for Bannerlord confirmed off (see journal env table; junction + Steam Cloud has caused save loss).
- [ ] `.\mnr-toggle.ps1 -Status` shows current state = **SAFE for existing vanilla-map saves** before you flip.
- [ ] Enhancer + Pinned GD + Village Defense + BetterPatrols crash-free confirmed on current save (already done 2026-09-22).

## Arm and launch

- [ ] `.\mnr-toggle.ps1 -Enable` — flips MNR `DefaultModule` and `IsSelected` in one step.
- [ ] `.\mnr-toggle.ps1 -Status` verifies **STATE: ARMED for Phase 3**.
- [ ] Launcher opened. Check the Mods tab has:
  - `More Nations Remastered` ✓ (loads after Sandbox — auto-ordered by DependedModules).
  - `PS Cache Warmup` ✓ (loads after PlayerSettlement).
  - `Player Settlement` ✓.
- [ ] Launch.

## New campaign creation

- [ ] Main menu → New Campaign.
- [ ] MNR shows an in-game notice (its `InitialMenuOptionPatch`): "world generation takes 1:30-3:00 min". Wait it out; do NOT click away.
- [ ] Character creation runs to completion without freeze.
  - _Known cousin bug (§32 note)_: some Steam-integrity issues froze users at appearance customization. If it freezes, verify game files via Steam and retry — not a mod bug.
- [ ] World map loads. New settlements should have `HH_` / `WW_` prefixed names (MNR namespace).

## In-game observation, first 5 in-game days

- [ ] Message bar shortly after load: `[GD Msg Enhancer] patch installed.` (Enhancer cyan message).
- [ ] Message bar: `[PS Cache Warmup] active. Subscribed to SettlementBuildCompleteEvent.` (or the mod's equivalent green load message).
- [ ] Day-tick performance is acceptable (baseline pre-MNR was ~180 settlements; MNR takes it to ~777, so daily tick may take longer but should not stall).
- [ ] No red-text unhandled exceptions in `Configs\ModLogs\butterlib<今天>.txt`.

## Build a Village via PlayerSettlement

- [ ] Enter your fief menu → PS "Build settlement" flow.
- [ ] Pick Village → wait for construction → completion.
- [ ] When completion fires, message bar should show four lines from `[PS Cache Warmup]`:
  1. `queued 'Village X' for warmup (queue size 1).`
  2. `warming distance cache for 'Village X' (~776 pairs). Expect brief loading...`
  3. Periodic `progress N / total pairs...`
  4. `done for 'Village X' (776 pairs).` (about 9 seconds in)
- [ ] After the done line, world-map movement + AI decisions should feel smooth (no random spike lag).

## Try Castle / Town (if v0.1.1 is enough — otherwise v0.2 blocked on Phase 1 agent)

- [ ] Try building a Castle. If lag returns despite the "done" line, that's the signal that Town/Castle have extra heavy-init subsystems beyond distance cache — the Phase 1 agent's spec targets this exact gap for `PSCacheWarmup v0.2`.
- [ ] Try building a Town. Same expectation as Castle.

## Post-session validation

- [ ] Exit to main menu. Should not crash (§40 confirmed net48+AssemblyInfo fix).
- [ ] Run `.\mnr-phase3-verify.ps1` — reports 4-layer evidence + settlement build event history + any exceptions.
- [ ] If PASS: MNR × PS × PSCacheWarmup v0.1.1 confirmed on the Village side.
- [ ] If PARTIAL: check which layer missed; likely candidates:
  - Layer 3 miss → PS's `PlayerSettlementBehaviour` type name changed; update reflection lookup in `PSCacheWarmup\src\SubModule.cs`.
  - Layer 4a miss → subscription happened but PS didn't fire the event for this session's build; may just mean no PS settlement was actually completed.
  - Layer 4b miss → event fired but drain never advanced; check `OnApplicationTick` code path.

## Rollback

- [ ] If you want to shelf MNR and come back to your old vanilla-map saves: `.\mnr-toggle.ps1 -Disable`. Confirmed by `-Status` showing **SAFE for existing vanilla-map saves**. PSCacheWarmup stays enabled but harmless (no MNR = no massive settlement count = no cache-warmup lag pattern to solve).

## Follow-up if v0.1.1 covers Village but not Castle/Town

- Wait for the Phase 1 agent's report (auto-notification into the parent conversation). It's investigating three specifically-not-yet-covered subsystems:
  1. PS `Settlement build complete` side effects beyond distance cache (garrison spawn, building tree, prosperity init).
  2. MNR 8 KB DLL scan for undocumented daily-tick loops over 597 settlements.
  3. `SandBoxNavigationCache` batch API alternatives to the current 3-pair-per-tick drain.
- Once that report lands, write `PSCacheWarmup v0.2` with the additional warmup targets and re-run this whole checklist.
