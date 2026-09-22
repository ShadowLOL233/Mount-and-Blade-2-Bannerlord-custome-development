<!--
  REPLICATE.md — machine-follow execution档案 for cloning this modpack onto a new device.
  Audience: Claude Code running on a fresh Windows install.
  Not optimized for human reading. Optimized for: (a) sequential top-to-bottom execution,
  (b) each [P<x>.<y>] step is self-contained (GOAL / PRE / DO / VERIFY / ROLLBACK),
  (c) hardcoded paths use the source device's E:\ layout — override via variables in [P0.1].

  If any step's VERIFY fails, stop and read that section's ROLLBACK before continuing.
  Non-idempotent steps are marked [ONCE]. All others can be re-run safely.
-->

# REPLICATE — 完整复刻档案

```yaml
purpose: Full replication of this Bannerlord modpack on a new Windows device
audience: Claude Code on target machine
source-device-snapshot: 2026-09-22
game-version: Bannerlord v1.4.7 (build 117484)
game-beta-branch: v1.4.7
workshop-count-required: 20 subscribed + 4 optional (see [P1.3])
custom-mods-count: 7 (built from this repo)
config-file-edits: 15
dll-byte-patch: 1 (GarrisonDrills)
estimated-time: 90-120 min (excluding Steam download time)
```

**Companion documents** (must-read before diving in):
- [`ModdingJournal.md`](./ModdingJournal.md) — history / rationale / decompiled internals. Consult when a step's WHY is unclear or when debugging.
- [`OSA_Reference/README.md`](./OSA_Reference/README.md) — OSA/OSW/Saddlery item data (CSV) — recipe consumers.
- [`RBM_Reference/README.md`](./RBM_Reference/README.md) — RBM/RBM_WS item data (CSV) — recipe consumers.
- [`TroopDesignReference.md`](./TroopDesignReference.md) — troop skill / equipment templates (unrelated to replication itself).

---

## Path convention

The source device uses `E:\` for Steam library and user data. **Target device may differ**. All commands below reference two logical roots you should set first:

- `$GAME_ROOT` = Bannerlord install dir (contains `bin/Win64_Shipping_Client/` + `Modules/`)
- `$WORKSHOP_ROOT` = Steam workshop 261550 dir (contains one folder per subscribed mod, folder name = workshop id)
- `$USER_DATA` = Bannerlord user data root (contains `Configs/`, `Game Saves/`)

Source-device values (change if target differs):
```powershell
$GAME_ROOT     = 'E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord'
$WORKSHOP_ROOT = 'E:\SteamLibrary\steamapps\workshop\content\261550'
$USER_DATA     = 'E:\Bannerlord-UserData\Mount and Blade II Bannerlord'  # if junction'd (see P2)
# OR (default OneDrive location, if not junction'd):
$USER_DATA_DEFAULT = "$env:USERPROFILE\OneDrive\Documents\Mount and Blade II Bannerlord"
```

If Steam is on `D:\` (second dev machine): substitute accordingly. `RBMPlayerStaminaPoiseBuff/src/*.csproj` and other mod csproj files respect the `$env:BannerlordBin` environment variable — export it before `dotnet build` (see [P6.0]).

---

# PHASE 0 · Prerequisites

## [P0.1] Verify tooling

**GOAL**: Ensure all required tools are installed. Install anything missing.

**PRE**: Windows 10/11, admin rights for winget.

**DO** — run these detection commands; install any that report missing:
```powershell
# Required
Get-Command git         -ErrorAction SilentlyContinue     # winget install Git.Git
Get-Command dotnet      -ErrorAction SilentlyContinue     # winget install Microsoft.DotNet.SDK.8
(Get-Command dotnet).Path -and (dotnet --list-sdks | Select-String '8\.')  # confirms SDK 8+

# .NET Framework 4.8.1 Developer Pack (required by net472 target of custom mods)
Get-ChildItem 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework' -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -like 'v4.8*' }
# If nothing found: winget install Microsoft.DotNet.Framework.DeveloperPack_4

# PowerShell (must be 5.1+, comes with Windows)
$PSVersionTable.PSVersion

# Steam (must exist — Bannerlord licence goes through it)
Get-Command steam -ErrorAction SilentlyContinue
# OR check registry:
Get-ItemProperty 'HKCU:\SOFTWARE\Valve\Steam' -ErrorAction SilentlyContinue | Select SteamPath

# OPTIONAL — decompilation tools (only needed if debugging RBM/mod internals)
Get-Command ilspycmd -ErrorAction SilentlyContinue        # dotnet tool install -g ilspycmd --version 8.2.0.7535
Test-Path 'C:\Users\*\Desktop\dnSpy-net-win64\dnSpy.Console.exe'  # download from github.com/dnSpyEx/dnSpy
```

**VERIFY**: All 4 required commands (git, dotnet SDK 8, .NET Framework 4.8.1 Dev Pack, PowerShell 5.1+) resolve. Steam installed.

**ROLLBACK**: N/A (installations are non-destructive).

---

# PHASE 1 · Steam & Bannerlord baseline

## [P1.1] [ONCE] Lock Bannerlord beta branch to v1.4.7

**GOAL**: Prevent Steam from auto-upgrading past v1.4.7. Mods in this pack are validated only at v1.4.7.

**PRE**: Bannerlord installed via Steam.

**DO** (manual — Steam has no CLI for beta selection):
1. Steam → right-click *Mount & Blade II: Bannerlord* → Properties → Betas
2. Beta Participation dropdown → select **`v1.4.7`**
3. Close Properties. Steam will download the v1.4.7 baseline.

**VERIFY**:
```powershell
$acf = 'E:\SteamLibrary\steamapps\appmanifest_261550.acf'   # adjust drive letter
(Get-Content $acf -Raw) -match 'BetaKey"\s*"v1\.4\.7"'
# Also confirm build id 117484:
Get-Item "$GAME_ROOT\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll" |
    Select-Object VersionInfo
```

**ROLLBACK**: Change Beta Participation back to "None - Opt out of all beta programs" and let Steam re-download current.

---

## [P1.2] [ONCE] Disable Steam Cloud for Bannerlord

**GOAL**: Prevent Steam Cloud from overwriting saves after junction step ([P2.1]) — this bit us in Bug #2 (journal §Bug #2).

**PRE**: [P1.1] complete.

**DO** (manual):
1. Steam → right-click *Bannerlord* → Properties → General
2. **Uncheck** "Keep game saves in the Steam Cloud"

**VERIFY**: Same dialog reopened, box stays unchecked.

**ROLLBACK**: Re-check the box (will merge cloud saves back).

---

## [P1.3] Subscribe Workshop mods (source-of-truth list)

**GOAL**: Subscribe all mods used by this pack. Steam auto-downloads to `$WORKSHOP_ROOT\<id>\`.

**PRE**: [P1.1] complete.

**DO** (manual — Steam Workshop has no bulk-subscribe API, you'll click through each):

Required (20):

| Workshop ID | Mod Name | Version at snapshot |
|---|---|---|
| 2859188632 | Bannerlord.Harmony | v2.4.2.248 |
| 2859232415 | Bannerlord.ButterLib | v2.12.0 |
| 2859222409 | Bannerlord.UIExtenderEx | v2.13.3 |
| 2859238197 | Mod Configuration Menu v5 (MCM) | v5.12.3 |
| 3761572229 | Mod Configuration Menu v5 (旧版) | v5.11.4 |
| 2859251492 | Realistic Battle Mod (RBM) | v4.5.0 |
| 3635788184 | RBM Weapons + Shields (RBM_WS) | v4.5.0 |
| 3747725551 | RTSCamera | v5.4.16 |
| 3747771970 | RTSCamera.CommandSystem | v5.4.16 |
| 2875093027 | DismembermentPlus | v2.0.8.8 |
| 2859265386 | ImprovedGarrisons (IG) | v4.2.0.7 |
| 3599557394 | Retinues | v1.4.14.31 |
| 3372837208 | Party Size Reunited (PSR) | v2.2.0 |
| 2957211804 | ChooseYourTroops (CYT) | v1.8.3 |
| 3720376888 | PlayerSettlement (PS) | v7.5.0 |
| 3735834360 | GarrisonDrills | v1.1.0 |
| 3011479883 | Open Source Armory (OSA) | v2.0.0 |
| 3010984416 | Open Source Weaponry (OSW) | v2.0.1 |
| 3010990914 | Open Source Saddlery | v2.0.0 |
| 3799709056 | AutoParry | v2.0.0 |

Optional but currently enabled (4):

| Workshop ID | Mod Name | Version | Purpose |
|---|---|---|---|
| 2882183897 | (misc — subscribed on source device, low priority) | — | — |
| 3782054420 | BetterPatrols | v1.0.0 | Adds patrol logic |
| 3780900172 | Village Defense (Xiangyong) | v1.1.11 | 村庄乡勇防御 |
| 3177450219 | MarriageFertility | v0.6.7 | (Some features unstable — user-installed, verified compatible with the rest) |

Disabled on source device (skip subscription unless needed):
- 3701507503 (unknown misc)
- 3764647388 (Test / War Sails Ship Lab) — dev sandbox
- 2875138158 (unknown misc)
- 3749328895 (UI Mount Types Summary / Bannerlord.HorseSummary)
- BirthAndDeath / FastMode / Cheats / XorberaxLegacy / BahamutArmory / swadian armoury / CustomClanPartySize — Native or 3rd party, currently IsSelected=false in LauncherData

**VERIFY**:
```powershell
$expectedIds = @('2859188632','2859232415','2859222409','2859238197','3761572229',
                 '2859251492','3635788184','3747725551','3747771970','2875093027',
                 '2859265386','3599557394','3372837208','2957211804','3720376888',
                 '3735834360','3011479883','3010984416','3010990914','3799709056')
$missing = @()
foreach ($id in $expectedIds) {
    if (-not (Test-Path "$WORKSHOP_ROOT\$id\SubModule.xml")) { $missing += $id }
}
if ($missing) { "MISSING: $($missing -join ', ')" } else { "All 20 required mods present." }
```

**ROLLBACK**: unsubscribe individual mods via Steam Workshop.

---

## [P1.4] Wait for downloads

**GOAL**: All subscribed mods fully downloaded before proceeding.

**DO**: Steam client → Downloads tab, watch queue drain. Approx 3-6 GB depending on line speed.

**VERIFY**: [P1.3]'s VERIFY block reports all present. Also:
```powershell
Get-ChildItem $WORKSHOP_ROOT -Directory | Measure-Object | Select Count
# should be >= 20
```

---

# PHASE 2 · User-data junction (OPTIONAL but recommended)

## [P2.1] [ONCE] Move Bannerlord user data off OneDrive

**GOAL**: OneDrive Files-On-Demand throttles Bannerlord IO (loads / saves stall). Move to local disk via directory junction.

**PRE**: Bannerlord launched at least once (so `~/Documents/Mount and Blade II Bannerlord/` exists).

**DO**:
```powershell
$src = "$env:USERPROFILE\OneDrive\Documents\Mount and Blade II Bannerlord"
$dst = 'E:\Bannerlord-UserData\Mount and Blade II Bannerlord'   # target-device-adjustable

# Move existing data
New-Item -ItemType Directory -Force -Path (Split-Path $dst -Parent) | Out-Null
Move-Item -LiteralPath $src -Destination "$src.OneDriveBackup"
Move-Item -LiteralPath "$src.OneDriveBackup" -Destination $dst
# create the junction pointing OneDrive path -> local disk
cmd /c mklink /J "`"$src`"" "`"$dst`""

# Verify reparse tag (should be 0xA0000003 = IO_REPARSE_TAG_MOUNT_POINT)
Get-Item -LiteralPath $src -Force | Select-Object Name, Attributes, Target
```

**VERIFY**:
```powershell
(Get-Item -LiteralPath $src -Force).Attributes -match 'ReparsePoint'
Test-Path "$dst\Configs"   # true (files at real location)
Test-Path "$src\Configs"   # true (via junction)
```

**ROLLBACK**:
```powershell
Remove-Item -LiteralPath $src -Force    # removes junction only (not real files)
Move-Item -LiteralPath $dst -Destination $src   # restore
```

Update `$USER_DATA` to `$dst`.

---

# PHASE 3 · First launch to generate default configs

## [P3.1] [ONCE] Boot Bannerlord to main menu, then quit

**GOAL**: Force each mod to create default `Configs\ModSettings\<ModName>\...xml` files. These don't exist until first launch.

**PRE**: [P1.4] complete, [P2.1] optional. LauncherData.xml may not yet enable all mods — that's OK, just enable core-lib mods (Harmony, ButterLib, UIExtenderEx, MCM) and RBM/Retinues so their Configs generate.

**DO** (manual):
1. Launch Bannerlord via Steam
2. In Launcher, tick core mods (Harmony, ButterLib, UIExtenderEx, MCM, RBM, Retinues, PSR, IG, GarrisonDrills — minimum for Configs generation)
3. Click Play → reach Main Menu
4. Quit
5. Confirm files exist:
```powershell
Test-Path "$USER_DATA\Configs\ModSettings\PartySizeReunited\PartySizeReunited.json"
Test-Path "$USER_DATA\Configs\ModSettings\Retinues\Retinues.Settings.xml"
Test-Path "$USER_DATA\Configs\RBM\config.xml"
Test-Path "$USER_DATA\Configs\ImprovedGarrisons"
Test-Path "$USER_DATA\Configs\LauncherData.xml"
```

**VERIFY**: all 5 return True.

**ROLLBACK**: N/A (this only creates files).

---

# PHASE 4 · Config file edits

## [P4.1] PartySizeReunited: `psr_bonus_scope` 2 → 0

**GOAL**: Only expand player party; leave AI lords vanilla. Fixes Bug #1 (mass-battle settlement deadlock).

**DO**:
```powershell
$f = "$USER_DATA\Configs\ModSettings\PartySizeReunited\PartySizeReunited.json"
Copy-Item $f "$f.bak-replicate-$((Get-Date).ToString('yyyyMMdd'))"
$j = Get-Content $f -Raw | ConvertFrom-Json
$j.psr_bonus_scope = 0
$j | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $f -Encoding UTF8
```

**VERIFY**:
```powershell
(Get-Content $f -Raw | ConvertFrom-Json).psr_bonus_scope -eq 0
```

**ROLLBACK**: `Move-Item "$f.bak-replicate-*" $f -Force`

---

## [P4.2] RBM food economy — 3 keys

**GOAL**: Fix RBM's fief food deficit (-800/day) by combining village production buff + per-troop cost reduction. Details in journal §5.

**DO**:
```powershell
$f = "$USER_DATA\Configs\RBM\config.xml"
Copy-Item $f "$f.bak-replicate-$((Get-Date).ToString('yyyyMMdd'))"
[xml]$x = Get-Content $f -Raw
$x.SelectSingleNode('//VillageProductionMultiplier').'#text' = '0.75'   # was 0.5
$x.SelectSingleNode('//TroopFoodWageFraction').'#text'      = '0.1'    # was 0.5
$x.Save($f)
```

**VERIFY**:
```powershell
([xml](Get-Content $f -Raw)).SelectSingleNode('//VillageProductionMultiplier').'#text'  # 0.75
([xml](Get-Content $f -Raw)).SelectSingleNode('//TroopFoodWageFraction').'#text'        # 0.1
```

**ROLLBACK**: restore `.bak-replicate-*`.

---

## [P4.3] ImprovedGarrisons — enable food-gathering + player bonus

**GOAL**: +1000/day food per player fief (IG's `[IG-Cheats]` module). Complements [P4.2].

**PRE**: [P3.1] complete AND user has entered at least one campaign so that IG save-per-hash config files exist under `Configs\ImprovedGarrisons\Saves\`. If target device is starting fresh with no saves, this step waits until a new campaign is created and re-run.

**DO** (batch-apply to all IG save configs):
```powershell
$dir = "$USER_DATA\Configs\ImprovedGarrisons\Saves"
$stamp = (Get-Date).ToString('yyyyMMdd')
Get-ChildItem $dir -Filter 'IGConfiguration_*.xml' | ForEach-Object {
    Copy-Item $_.FullName "$($_.FullName).bak-replicate-$stamp"
    [xml]$x = Get-Content $_.FullName -Raw
    $x.SelectSingleNode('//LoadFoodGatheringModule').'#text' = 'true'
    $x.SelectSingleNode('//EnablePlayerFoodBonus').'#text'   = 'true'
    $x.SelectSingleNode('//DailyFoodGatheringAmount').'#text' = '1000'  # was 10
    # NPCSpawnGuards flip (journal modification #9)
    $x.SelectSingleNode('//NPCSpawnGuards').'#text' = 'true'  # was false
    $x.Save($_.FullName)
}
```

**VERIFY**:
```powershell
Get-ChildItem "$USER_DATA\Configs\ImprovedGarrisons\Saves" -Filter 'IGConfiguration_*.xml' | ForEach-Object {
    $x = [xml](Get-Content $_.FullName -Raw)
    [pscustomobject]@{
        file = $_.Name
        LoadFoodGathering = $x.SelectSingleNode('//LoadFoodGatheringModule').'#text'
        DailyFood = $x.SelectSingleNode('//DailyFoodGatheringAmount').'#text'
        NPCGuards = $x.SelectSingleNode('//NPCSpawnGuards').'#text'
    }
} | Format-Table
```
Expected: every row = true / 1000 / true.

**ROLLBACK**: `Get-ChildItem $dir -Filter '*.bak-replicate-*' | ForEach { Move-Item $_.FullName ($_.FullName -replace '\.bak-replicate-.*$','') -Force }`

---

## [P4.4] Retinues — MaxTroopTier / XP economy / Doctrine costs

**GOAL**: Raise troop tier ceiling to 10, zero out all XP costs, bypass doctrine feats and set all doctrine gold/influence costs to zero. Details in journal §6/§7/§8.

**DO**:
```powershell
$f = "$USER_DATA\Configs\ModSettings\Retinues\Retinues.Settings.xml"
Copy-Item $f "$f.bak-replicate-$((Get-Date).ToString('yyyyMMdd'))"
[xml]$x = Get-Content $f -Raw

# §6: tier ceiling
$x.SelectSingleNode('//MaxTroopTier').'#text' = '10'   # was 8

# §7: XP economy zero-out
$x.SelectSingleNode('//BaseSkillXpCost').'#text'      = '0'      # was 100
$x.SelectSingleNode('//SkillXpCostPerPoint').'#text'  = '0'      # was 1
$x.SelectSingleNode('//SharedXpPool').'#text'         = 'true'   # was false
$x.SelectSingleNode('//ForceXpRefunds').'#text'       = 'true'   # was false

# §8: doctrine costs zero-out
$x.SelectSingleNode('//EnableFeatRequirements').'#text'         = 'false'  # was true
$x.SelectSingleNode('//DoctrineGoldCostMultiplier').'#text'     = '0'      # was 1
$x.SelectSingleNode('//DoctrineInfluenceCostMultiplier').'#text' = '0'     # was 1

$x.Save($f)
```

**VERIFY**:
```powershell
$x = [xml](Get-Content "$USER_DATA\Configs\ModSettings\Retinues\Retinues.Settings.xml" -Raw)
[pscustomobject]@{
    MaxTroopTier   = $x.SelectSingleNode('//MaxTroopTier').'#text'                    # 10
    BaseSkillXp    = $x.SelectSingleNode('//BaseSkillXpCost').'#text'                 # 0
    SharedXpPool   = $x.SelectSingleNode('//SharedXpPool').'#text'                    # true
    FeatReqs       = $x.SelectSingleNode('//EnableFeatRequirements').'#text'          # false
    DoctrineGold   = $x.SelectSingleNode('//DoctrineGoldCostMultiplier').'#text'      # 0
}
```

**ROLLBACK**: restore `.bak-replicate-*`.

---

# PHASE 5 · GarrisonDrills DLL byte patch

## [P5.1] Patch training XP constants (10/30/50 → 20/60/100)

**GOAL**: Double GarrisonDrills training XP per soldier per 6h tick. Details in journal §3.

**PRE**: GarrisonDrills subscribed and downloaded ([P1.3]).

**WARN**: **Non-idempotent** — running twice on already-patched DLL will corrupt. Check byte values before patching.

**DO**:
```powershell
$dll = "$WORKSHOP_ROOT\3735834360\bin\Win64_Shipping_Client\GarrisonDrills.dll"
$stamp = (Get-Date).ToString('yyyyMMdd')
$backup = "$dll.orig-replicate-$stamp"

if (-not (Test-Path $backup)) {
    Copy-Item $dll $backup
}

$bytes = [IO.File]::ReadAllBytes($dll)

# Sanity check: expect original values at those offsets. If already 0x14/0x3C/0x64,
# DLL is already patched -> skip.
$isVanilla = ($bytes[2332] -eq 0x0A) -and ($bytes[2348] -eq 0x1E) -and ($bytes[2365] -eq 0x32)
$isPatched = ($bytes[2332] -eq 0x14) -and ($bytes[2348] -eq 0x3C) -and ($bytes[2365] -eq 0x64)

if ($isPatched) {
    Write-Host "DLL already patched — skipping."
} elseif ($isVanilla) {
    $bytes[2332] = 0x14   # Basic XP   10 -> 20
    $bytes[2348] = 0x3C   # Advanced   30 -> 60
    $bytes[2365] = 0x64   # Masterful  50 -> 100
    [IO.File]::WriteAllBytes($dll, $bytes)
    Write-Host "Patched OK."
} else {
    Write-Warning "Unknown byte values at target offsets — GarrisonDrills version may have changed. Abort. Bytes: $($bytes[2332].ToString('X2')), $($bytes[2348].ToString('X2')), $($bytes[2365].ToString('X2'))"
}
```

**VERIFY**:
```powershell
$b = [IO.File]::ReadAllBytes("$WORKSHOP_ROOT\3735834360\bin\Win64_Shipping_Client\GarrisonDrills.dll")
"$($b[2332].ToString('X2'))/$($b[2348].ToString('X2'))/$($b[2365].ToString('X2'))"  # 14/3C/64
```

**ALSO** (cosmetic — sync UI text):
```powershell
$xmlBase = "$WORKSHOP_ROOT\3735834360\ModuleData\Languages"
foreach ($xml in @("$xmlBase\std_GarrisonDrills_strings.xml", "$xmlBase\CNs\std_GarrisonDrills_strings_cns.xml")) {
    if (Test-Path $xml) {
        Copy-Item $xml "$xml.bak-replicate-$stamp"
        (Get-Content $xml -Raw) -replace '\+10','+20' -replace '\+30','+60' -replace '\+50','+100' |
            Set-Content -LiteralPath $xml -Encoding UTF8
    }
}
```

**ROLLBACK**:
```powershell
Move-Item "$dll.orig-replicate-*" $dll -Force
```

**RE-APPLY TRIGGER**: If Steam updates GarrisonDrills, DLL is overwritten → re-run [P5.1].

---

# PHASE 6 · Clone repo + build all custom mods

## [P6.0] Clone repo + set BannerlordBin env var

**GOAL**: Get all custom mod source, prepare build env for target device paths.

**DO**:
```powershell
$repoRoot = "$env:USERPROFILE\git\Mount-and-Blade-2-Bannerlord-custome-development"
if (-not (Test-Path $repoRoot)) {
    New-Item -ItemType Directory -Force -Path (Split-Path $repoRoot -Parent) | Out-Null
    git clone https://github.com/ShadowLOL233/Mount-and-Blade-2-Bannerlord-custome-development.git $repoRoot
} else {
    git -C $repoRoot pull --rebase
}

# All custom mods' csproj files honour $env:BannerlordBin. Set it for this session:
$env:BannerlordBin = "$GAME_ROOT\bin\Win64_Shipping_Client"
# Sanity: BannerlordBin must contain TaleWorlds.CampaignSystem.dll
Test-Path "$env:BannerlordBin\TaleWorlds.CampaignSystem.dll"
```

**VERIFY**: repo cloned, `Test-Path` returns True.

---

## [P6.1] Build + deploy EquipmentSpawnerMod (v1.8.1)

**GOAL**: Equipment injector + personal stash + culture-filtered UI. See mod's own README + journal §26/§25/§24/§23/§22/§21/§20/§19/§18/§17/§16/§15/§14/§13/§10 for feature history.

**PRE**: [P6.0] complete.

**DO**:
```powershell
& "$repoRoot\EquipmentSpawnerMod\deploy.ps1" -GameRoot $GAME_ROOT
```
Script builds Release + copies `SubModule.xml` + `bin/*.dll` + `GUI/PrefabExtensions/*.xml` to `$GAME_ROOT\Modules\EquipmentSpawnerMod\`.

**VERIFY**:
```powershell
Test-Path "$GAME_ROOT\Modules\EquipmentSpawnerMod\bin\Win64_Shipping_Client\EquipmentSpawnerMod.dll"
Test-Path "$GAME_ROOT\Modules\EquipmentSpawnerMod\GUI\PrefabExtensions\InventoryCultureButtonRow.xml"
(Select-String -Path "$GAME_ROOT\Modules\EquipmentSpawnerMod\SubModule.xml" -Pattern 'v1\.8\.1').Count -eq 1
```

**ROLLBACK**: `Remove-Item "$GAME_ROOT\Modules\EquipmentSpawnerMod" -Recurse -Force`

---

## [P6.2] Build + deploy OpenSourceArmouryRBMBalance (v1.1)

**GOAL**: XML override mod that buffs OSA armor to RBM scale + downscales OSA blade damage_factor. Generated from a PowerShell recipe (`src/generate.ps1`) using RBM/OSA workshop XML as input.

**PRE**: [P6.0] + OSA/OSW/RBM/RBM_WS all subscribed.

**DO**:
```powershell
# Regenerate override XML from workshop (accounts for any workshop update since commit)
& "$repoRoot\OpenSourceArmouryRBMBalance\src\generate.ps1" -WorkshopRoot $WORKSHOP_ROOT

# Deploy: this mod is pure XML — just copy Modules/ContentFolder over
$src = "$repoRoot\OpenSourceArmouryRBMBalance"
$dst = "$GAME_ROOT\Modules\OpenSourceArmouryRBMBalance"
New-Item -ItemType Directory -Force -Path $dst | Out-Null
Copy-Item -Path "$src\SubModule.xml" -Destination $dst -Force
Copy-Item -Path "$src\ModuleData" -Destination $dst -Recurse -Force
```

**VERIFY**:
```powershell
Test-Path "$dst\ModuleData\OSABalance_armor_override.xml"
Test-Path "$dst\ModuleData\OSABalance_pieces_override.xml"
(Select-String -Path "$dst\SubModule.xml" -Pattern 'v1\.1').Count -eq 1
# Sanity: armor XML should have ~1500+ Item lines
(Select-String -Path "$dst\ModuleData\OSABalance_armor_override.xml" -Pattern '<Item id=').Count
```

**ROLLBACK**: `Remove-Item $dst -Recurse -Force`.

---

## [P6.3] Build + deploy RetinuesCultureFilter (v1.4)

**GOAL**: Culture filter buttons + Harmony patch on Retinues' EquipmentListVM. See journal §16-§20.

**PRE**: [P6.0] + Retinues subscribed.

**DO**:
```powershell
& "$repoRoot\RetinuesCultureFilter\deploy.ps1" -GameRoot $GAME_ROOT
```

**VERIFY**:
```powershell
Test-Path "$GAME_ROOT\Modules\RetinuesCultureFilter\bin\Win64_Shipping_Client\RetinuesCultureFilter.dll"
(Select-String -Path "$GAME_ROOT\Modules\RetinuesCultureFilter\SubModule.xml" -Pattern 'v1\.4').Count -eq 1
```

**ROLLBACK**: `Remove-Item "$GAME_ROOT\Modules\RetinuesCultureFilter" -Recurse -Force`.

---

## [P6.4b] Build + deploy PSCacheWarmup (v0.1.1)

**GOAL**: Auto-warm vanilla MapDistanceModel cache after PlayerSettlement building complete, to remove Building-complete lag in MNR-style large-map scenarios. See journal §31 (v0.1 architecture) + §32 (v0.1.1 file-log addition).

**PRE**: [P6.0] complete. PlayerSettlement subscribed via [P1.3] (soft dep — mod is harmless if PS missing).

**DO**:
```powershell
& "$repoRoot\PSCacheWarmup\deploy.ps1" -GameRoot $GAME_ROOT
```

**VERIFY**:
```powershell
Test-Path "$GAME_ROOT\Modules\PSCacheWarmup\bin\Win64_Shipping_Client\PSCacheWarmup.dll"
(Select-String -Path "$GAME_ROOT\Modules\PSCacheWarmup\SubModule.xml" -Pattern 'v0\.1\.1').Count -eq 1
```

**In-game verify** (once launcher restarted — see APX.6): check `$USER_DATA\Configs\ModLogs\PSCacheWarmup.log` for `OnSubModuleLoad called` and `SUBSCRIBE SUCCEEDED` lines.

**ROLLBACK**: `Remove-Item "$GAME_ROOT\Modules\PSCacheWarmup" -Recurse -Force`.

---

## [P6.4c] Build + deploy BetterPatrolsBrake (v0.1)

**GOAL**: Removes 2 BetterPatrols Harmony patches (`PatrolFrequentRethinkPatch` + `PatrolResumeScoringPatch`) that break vanilla AI throttling and cause strategy-map fast-forward lag. Preserves BetterPatrols' other ~24 patches (PatrolSizeTable / QualityTable / WanderRadius / VillageDefender etc.). See journal §33 for decompile analysis + trade-off table.

**PRE**: [P6.0] complete. BetterPatrols subscribed via [P1.3] (soft dep — mod is harmless without BetterPatrols). **⚠ Status per journal §33: v0.1 not yet in-game verified as of 2026-09-22.**

**DO**:
```powershell
& "$repoRoot\BetterPatrolsBrake\deploy.ps1" -GameRoot $GAME_ROOT
```

**VERIFY**:
```powershell
Test-Path "$GAME_ROOT\Modules\BetterPatrolsBrake\bin\Win64_Shipping_Client\BetterPatrolsBrake.dll"
(Select-String -Path "$GAME_ROOT\Modules\BetterPatrolsBrake\SubModule.xml" -Pattern 'v0\.1').Count -eq 1
```

**In-game verify** (once launcher restarted): check `$USER_DATA\Configs\ModLogs\BetterPatrolsBrake.log` for `Postfix count before=1 after=0 (successfully removed 1)` on both HourlyTickParty and AiHourlyTick — that is the decisive evidence Brake actually removed the hot patches.

**ROLLBACK**: `Remove-Item "$GAME_ROOT\Modules\BetterPatrolsBrake" -Recurse -Force`.

---

## [P6.4] Build + deploy RBMPlayerStaminaPoiseBuff (v1.0.2)

**GOAL**: Boost player-only RBM stamina/posture regen by 6×/2×. AI unchanged. See journal §29 for the v1.0.2 crash-fix architecture (deferred manual patch — critical, do not revert to v1.0.1).

**PRE**: [P6.0] + RBM subscribed.

**DO**:
```powershell
& "$repoRoot\RBMPlayerStaminaPoiseBuff\deploy.ps1" -GameRoot $GAME_ROOT
```

**VERIFY**:
```powershell
Test-Path "$GAME_ROOT\Modules\RBMPlayerStaminaPoiseBuff\bin\Win64_Shipping_Client\RBMPlayerStaminaPoiseBuff.dll"
(Select-String -Path "$GAME_ROOT\Modules\RBMPlayerStaminaPoiseBuff\SubModule.xml" -Pattern 'v1\.0\.2').Count -eq 1
```

**ROLLBACK**: `Remove-Item "$GAME_ROOT\Modules\RBMPlayerStaminaPoiseBuff" -Recurse -Force`.

---

## [P6.5] Reference data packs (no build — repo-tracked CSVs)

**GOAL**: Have `OSA_Reference/` and `RBM_Reference/` CSVs available for balance queries. They're already in the repo; nothing to build.

**OPTIONAL** — regenerate CSVs against current workshop state (idempotent):
```powershell
& "$repoRoot\OSA_Reference\scripts\extract_osa.ps1" -WorkshopRoot $WORKSHOP_ROOT
& "$repoRoot\RBM_Reference\scripts\extract_rbm.ps1" -WorkshopRoot $WORKSHOP_ROOT
```

**VERIFY**:
```powershell
Test-Path "$repoRoot\OSA_Reference\data\osa_items.csv"
Test-Path "$repoRoot\RBM_Reference\data\rbm_items.csv"
(Import-Csv "$repoRoot\OSA_Reference\data\osa_items.csv" | Measure-Object).Count -ge 1800
(Import-Csv "$repoRoot\RBM_Reference\data\rbm_items.csv" | Measure-Object).Count -ge 1200
```

---

# PHASE 7 · LauncherData.xml configuration

## [P7.1] Set enabled/disabled state for all mods

**GOAL**: The launcher persists mod enable/disable + load order in `$USER_DATA\Configs\LauncherData.xml`. Populate it so the correct 20+5 mods are active.

**PRE**: [P1.4] complete (workshop mods present), [P3.1] complete (LauncherData.xml auto-generated on first launch), [P6.1-P6.4] complete (custom mods deployed).

**APPROACH**: You have two options; use whichever fits target-device state:

**Option A** — manual toggle in launcher (safest for first-time replication):
1. Launch Bannerlord → in launcher, tick these mods (order as loaded — dependencies must precede dependents):
   ```
   Native / SandBoxCore / Sandbox / StoryMode / CustomBattle / NavalDLC (all Native, tick)
   Bannerlord.Harmony
   Bannerlord.ButterLib
   Bannerlord.UIExtenderEx
   Bannerlord.MBOptionScreen (v5.12.3)
   RBM
   RBM_WS
   RTSCamera
   RTSCamera.CommandSystem
   DismembermentPlus
   ImprovedGarrisons
   Retinues
   PartySizeReunited
   ChooseYourTroops
   PlayerSettlement
   GarrisonDrills
   AutoParry
   TroopClassifier              (see [P7.2] if missing)
   FormationManager             (see [P7.2] if missing)
   BattleSizeResized            (see [P7.2] if missing)
   OpenSourceArmory
   OpenSourceSaddlery
   OpenSourceWeaponry
   MarriageFertility            (optional)
   BetterPatrols                (optional — currently combined with BetterPatrolsBrake below)
   Xiangyong                    (optional)
   OpenSourceArmouryRBMBalance  ← must load AFTER OSA/OSW/Saddlery/RBM/RBM_WS
   EquipmentSpawnerMod
   RetinuesCultureFilter        ← must load AFTER Retinues
   RBMPlayerStaminaPoiseBuff    ← must load AFTER RBM
   PSCacheWarmup                ← must load AFTER PlayerSettlement (soft dep, safe if PS absent)
   BetterPatrolsBrake           ← must load AFTER BetterPatrols (soft dep, safe if BP absent)
   ```
2. Ensure `BirthAndDeath`, `FastMode`, `CustomClanPartySize`, `XorberaxLegacy`, `Cheats`, `Bannerlord.HorseSummary`, `Test` are UNticked.

**Option B** — programmatic (idempotent, best for automation):
Use the source-device LauncherData.xml as a template (available at
`$USER_DATA\Configs\LauncherData.xml` on source device; not in git because it's user data).
The definitive current-state snapshot can be recreated by reading source device's LauncherData
or by referencing journal §当前 IsSelected 状态 (search journal for "LauncherData" for the last
verified state).

**VERIFY**:
```powershell
[xml]$ld = Get-Content "$USER_DATA\Configs\LauncherData.xml" -Raw
$expected = @('Bannerlord.Harmony','Bannerlord.ButterLib','Bannerlord.UIExtenderEx',
    'Bannerlord.MBOptionScreen','Native','SandBoxCore','Sandbox','StoryMode','CustomBattle',
    'NavalDLC','RTSCamera','RTSCamera.CommandSystem','RBM','DismembermentPlus',
    'ImprovedGarrisons','Retinues','PartySizeReunited','ChooseYourTroops','PlayerSettlement',
    'RBM_WS','GarrisonDrills','AutoParry','OpenSourceArmory','OpenSourceSaddlery',
    'OpenSourceWeaponry','OpenSourceArmouryRBMBalance','EquipmentSpawnerMod',
    'RetinuesCultureFilter','RBMPlayerStaminaPoiseBuff')
$missing = @(); $disabled = @()
foreach ($id in $expected) {
    $node = $ld.SelectSingleNode("//UserModData[Id='$id']")
    if (-not $node) { $missing += $id; continue }
    if ($node.IsSelected -ne 'true') { $disabled += $id }
}
"Missing: $($missing -join ', ')"
"Disabled but expected enabled: $($disabled -join ', ')"
```

**ROLLBACK**:
```powershell
# LauncherData.xml is regenerated on launcher exit — no explicit rollback needed.
# To force reset: delete file, restart launcher, re-select mods manually.
Remove-Item "$USER_DATA\Configs\LauncherData.xml"
```

---

## [P7.2] [ONCE] Nexus-installed non-workshop mods (TroopClassifier + FormationManager + BattleSizeResized)

**GOAL**: These 3 mods are Nexus-only (not on Steam Workshop). Manual install.

**PRE**: Downloaded ZIPs available. Source-device notes (journal §模组清单待下载/计划安装):

| Mod | Version | Nexus ID |
|---|---|---|
| Stop Shuffling You Fools – Formation Manager | 0.5.2 | 11869 |
| TroopClassifier | v0.2.0 | 12104 |
| BattleSizeResized | 2.0.4 (for 1.4.x) | 8177 |

**DO** (manual per mod):
1. Download ZIP from Nexus mods (URL: `https://www.nexusmods.com/mountandblade2bannerlord/mods/<ID>`)
2. Extract to `$GAME_ROOT\Modules\<ModId>\` — the archive should already have this structure
3. Confirm `SubModule.xml` present at `Modules\<ModId>\SubModule.xml`

**VERIFY**:
```powershell
Test-Path "$GAME_ROOT\Modules\TroopClassifier\SubModule.xml"
Test-Path "$GAME_ROOT\Modules\FormationManager\SubModule.xml"
Test-Path "$GAME_ROOT\Modules\BattleSizeResized\SubModule.xml"
```

**ROLLBACK**: `Remove-Item "$GAME_ROOT\Modules\<ModId>" -Recurse -Force`.

---

# PHASE 8 · Verification (in-game)

## [P8.1] Launcher smoke test (before first launch)

**DO**:
1. Open Bannerlord launcher via Steam
2. Look at Mods tab — no red exclamation marks (would signal IsDangerous=true from broken DLL)
3. If any custom mod shows warning → check `Configs\LauncherData.xml` DLLCheckData section, and inspect `Configs\ModLogs\butterlib*.txt` from any previous launch attempt

**PASS CRITERIA**: All 25+ mods show clean status.

---

## [P8.2] Boot to main menu

**DO**: Click Play. Main menu should appear within 30-60s.

**FAIL MODES + REMEDIES**:
- **Silent crash before main menu, no crash dump**: Native access violation. Check `$USER_DATA\Configs\ModLogs\butterlib<yyyymmdd>.txt` — if last line is `Wrapping DebugManager of type ...` and log ends there, mod DLL failed to load after ButterLib init. Bisect: temporarily disable custom mods one at a time and retry.
- **Launcher's crash dialog with .NET exception**: Read exception; usually points to a missing dependency. Verify all core-libs enabled ([P7.1]).
- **Black screen / hang on 'Loading …'**: Usually shader-related. See journal Bug #7 (OSA shader cache). Try deleting `$WORKSHOP_ROOT\3011479883\Shaders\D3D11\` + `$WORKSHOP_ROOT\3010984416\Shaders\D3D11\` + `$WORKSHOP_ROOT\3010990914\Shaders\D3D11\` and re-launch (first launch will take 10-15 min to rebuild shaders).

**PASS**: Main menu loads.

---

## [P8.3] New-campaign run: 5-minute feature check

**GOAL**: Confirm all major config edits and custom mods are behaving.

**DO**:
1. Start New Campaign → skip intro
2. **Retinues XP economy**: Open Clan → Troops → pick any custom troop → try to add a skill point. UI should show "Cost: 0 XP".
3. **Retinues Doctrines**: Same screen → Doctrines tab. The 12 unlockable doctrines (see journal §8) should show `In Progress` and click-to-unlock at 0 gold / 0 influence.
4. **Retinues Culture Filter**: Enter custom-troop equipment editor → 6 culture buttons should appear beside the search bar. Click e.g. "Aserai" → equipment list filters to Aserai items.
5. **EquipmentSpawnerMod**: Enter any town → main menu should show `Manage personal equipment stash`. Open it → the inventory UI should have 6 culture buttons + "Inject to Stash" button at top of left column.
6. **Enter any tournament / custom battle** → Should see chat message: `RBM Player Stamina & Poise Buff v1.0.2 loaded. Player-only regen: stamina x6, posture x2. AI unchanged.` (may scroll fast — check log file if missed).
7. **Fight a battle** → player stamina/posture regen visibly ~6×/~2× faster than AI (subjective).
8. **Own a fief for 1 in-game day** → check daily food-change tooltip. Should show `[IG-Cheats] Garrison Food Bonus: +1000`, net change should be positive.
9. **Garrison training** → Town/castle → Train troops menu → should show `+20 / +60 / +100 XP per soldier`.
10. **220v240 mass battle** → post-battle settlement screen loads within seconds (Bug #1 fixed).

**FAIL SIGNAL FROM ANY ITEM**: Consult:
- [8.1-8.3 fail] → journal §Bug #<N> for that specific symptom
- Any Harmony patch warning → `Configs\ModLogs\trace<date>.txt`
- Any managed exception → `Configs\ModLogs\butterlib<date>.txt`

---

# APPENDICES

## [APX.1] Workshop ID quick lookup

Grep-friendly format:
```
2859188632  Bannerlord.Harmony             REQUIRED  v2.4.2.248
2859232415  Bannerlord.ButterLib           REQUIRED  v2.12.0
2859222409  Bannerlord.UIExtenderEx        REQUIRED  v2.13.3
2859238197  Bannerlord.MBOptionScreen      REQUIRED  v5.12.3
2859251492  RBM                            REQUIRED  v4.5.0
2859265386  ImprovedGarrisons              REQUIRED  v4.2.0.7
2875093027  DismembermentPlus              REQUIRED  v2.0.8.8
2957211804  ChooseYourTroops               REQUIRED  v1.8.3
3010984416  OpenSourceWeaponry (OSW)       REQUIRED  v2.0.1
3010990914  OpenSourceSaddlery             REQUIRED  v2.0.0
3011479883  OpenSourceArmory (OSA)         REQUIRED  v2.0.0
3177450219  MarriageFertility              OPTIONAL  v0.6.7
3372837208  PartySizeReunited (PSR)        REQUIRED  v2.2.0
3599557394  Retinues                       REQUIRED  v1.4.14.31
3635788184  RBM_WS                         REQUIRED  v4.5.0
3720376888  PlayerSettlement               REQUIRED  v7.5.0
3735834360  GarrisonDrills                 REQUIRED  v1.1.0 (DLL byte-patched: [P5.1])
3747725551  RTSCamera                      REQUIRED  v5.4.16
3747771970  RTSCamera.CommandSystem        REQUIRED  v5.4.16
3761572229  Bannerlord.MBOptionScreen 旧版 KEEP     v5.11.4 (co-exists with v5.12.3)
3780900172  Xiangyong (Village Defense)    OPTIONAL  v1.1.11
3782054420  BetterPatrols                  OPTIONAL  v1.0.0
3799709056  AutoParry                      REQUIRED  v2.0.0
# Below = subscribed on source device but IsSelected=false → skip if not needed:
2882183897  (misc)                         SKIP
3749328895  Bannerlord.HorseSummary        SKIP
3701507503  (misc)                         SKIP
3764647388  Test (War Sails Ship Lab)      SKIP (dev sandbox)
2875138158  (misc)                         SKIP
```

## [APX.2] Custom mod source-of-truth

All custom mods live inside this repo. Each has its own `SubModule.xml`, `src/`, `deploy.ps1` (or generator), and README:

```
EquipmentSpawnerMod/           v1.8.1  (equipment injector + personal stash + inventory culture UI)
OpenSourceArmouryRBMBalance/   v1.1    (OSA<->RBM armour + blade balance via XML overrides)
RetinuesCultureFilter/         v1.4.0  (culture filter for Retinues equipment editor)
RBMPlayerStaminaPoiseBuff/     v1.0.2  (RBM stamina/posture regen boost, player only)
PSCacheWarmup/                 v0.1.1  (auto-warm MapDistanceModel cache after PS building complete)
BetterPatrolsBrake/            v0.1    (remove 2 BetterPatrols hourly hot patches, keep the other 24)
```

## [APX.3] Cross-device path-variable table

If cloning to a device where Steam is on a different drive:

| Variable | Source device (E:\) | 2nd dev device (D:\) example |
|---|---|---|
| `$GAME_ROOT` | `E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord` | `D:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord` |
| `$WORKSHOP_ROOT` | `E:\SteamLibrary\steamapps\workshop\content\261550` | `D:\SteamLibrary\steamapps\workshop\content\261550` |
| `$USER_DATA` (junction'd) | `E:\Bannerlord-UserData\Mount and Blade II Bannerlord` | `$env:USERPROFILE\OneDrive\Documents\Mount and Blade II Bannerlord` (or any local path) |
| `$env:BannerlordBin` | `$GAME_ROOT\bin\Win64_Shipping_Client` | same |
| `$repoRoot` | `C:\Users\situj\git\Mount-and-Blade-2-Bannerlord-custome-development` | `$env:USERPROFILE\git\Mount-and-Blade-2-Bannerlord-custome-development` |

## [APX.4] Full log-file map (debugging)

Every `Configs\ModLogs\` file with what it says:
```
butterlib<date>.txt     ButterLib managed-code exceptions. First stop for crashes.
                         If last line is 'Wrapping DebugManager ...' and log ends
                         there → native crash after ButterLib init (bisect mods).
default<date>.log       MCM/ButterLib/Retinues general logs. AccessTools resolution
                         warnings, module load order.
trace<date>.txt         Harmony patch application, AccessTools 'method not found' warnings.
                         Check when: reflection-based mod (like RBM buff) fails silently.
Xiangyong.log           Village Defense per-day militia spawn/despawn.
```
Plus per-mod:
```
$WORKSHOP_ROOT\3599557394\debug.log           Retinues OnMissionStarted/OnEndMission probes
$USER_DATA\Configs\RBM\logs\garrison\          RBM garrison per-day accounting
$USER_DATA\CrashHandler\player_settlement_fixes.log  PlayerSettlement patch log
$USER_DATA\Configs\ImprovedGarrisons\Saves\    Per-save IG binary state + XML config
```

## [APX.5] Full rollback (nuke everything)

```powershell
# 1. Remove all custom-built mods
foreach ($m in 'EquipmentSpawnerMod','OpenSourceArmouryRBMBalance','RetinuesCultureFilter','RBMPlayerStaminaPoiseBuff') {
    Remove-Item "$GAME_ROOT\Modules\$m" -Recurse -Force -ErrorAction SilentlyContinue
}
# 2. Restore GarrisonDrills DLL
$dll = "$WORKSHOP_ROOT\3735834360\bin\Win64_Shipping_Client\GarrisonDrills.dll"
if (Test-Path "$dll.orig-replicate-*") { Move-Item "$dll.orig-replicate-*" $dll -Force }
# 3. Restore all Configs backups (assumes you kept .bak-replicate-* files)
Get-ChildItem "$USER_DATA\Configs" -Recurse -Filter '*.bak-replicate-*' | ForEach-Object {
    $orig = $_.FullName -replace '\.bak-replicate-.*$',''
    Move-Item $_.FullName $orig -Force
}
# 4. Manually re-untick custom + non-vanilla mods in launcher UI
```

## [APX.6] Common gotchas encountered on source device

1. **`--no-verify` / `-c commit.gpgsign=false`**: user setup requires signed commits — do NOT bypass.
2. **Launcher locks custom DLL**: If `deploy.ps1` fails with "file in use" — close Bannerlord launcher, re-run.
3. **Save format tied to `EquipmentSpawnerTypeDefiner`**: If you disable EquipmentSpawnerMod but a save was made with it enabled, that save can't load. Symptom = "save load fails but main menu OK" (journal §14).
4. **RBM v4.5.0 needs a NEW campaign** — save files from Bannerlord's early access era + RBM Campaign will hard-crash (journal Bug #3). Don't try to load pre-RBM saves.
5. **Windows Long Path** — some workshop dirs deep-nest. If `deploy.ps1` fails with path-too-long, enable long-path via `HKLM:\SYSTEM\CurrentControlSet\Control\FileSystem\LongPathsEnabled = 1`, reboot.
6. **XML BOM encoding** — when scripting XML edits, use `Set-Content -Encoding UTF8` (adds BOM) — Bannerlord tolerates both BOM/no-BOM, but consistency matters for `git diff`.
7. **OneDrive Files-on-Demand**: even without Steam Cloud, OneDrive can turn game saves into placeholder files. [P2.1] junction is strongly recommended.
8. **Launcher only scans Modules/ at startup, not at runtime**: if you deploy a new mod (new directory under `Modules/`) or replace a mod's DLL while `TaleWorlds.MountAndBlade.Launcher` is running, launcher will NOT see the change. **Always fully close launcher first, then deploy, then start launcher fresh.** Symptoms of ignoring this: (a) newly-deployed mod does not appear in launcher mod list; (b) `Copy-Item` in deploy.ps1 fails with `The process cannot access the file because it is being used by another process` — launcher holds a file handle on any DLL of a currently-selected mod even when idle at the mod list screen.
9. **`File read failed! Please try to verify your installation!` popup at startup**: engine-level error, usually caused by a partial workshop download or a corrupted vanilla file. Fix: Steam → right-click Bannerlord → Properties → Installed Files → Verify Integrity of Game Files. **Not a mod bug**, though can appear coincidentally after mod list changes if a Steam background update happens to coincide (encountered 2026-09-22).

---

*Snapshot: 2026-09-21. When source-device state drifts (new mod, deprecated mod, config change), update this file BEFORE the ModdingJournal narrative — REPLICATE.md is the machine-executable spec, journal is the human history.*
