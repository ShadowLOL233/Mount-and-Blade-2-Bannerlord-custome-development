<#
.SYNOPSIS
PlayerSettlement Rebuilt (PSLR) - P5 deploy script

.DESCRIPTION
Copy workshop 3720376888 complete PS structure to game Modules, overlay 2 rebuild DLLs,
drop bundled MCMv5.dll v5.11.3 (conflict with workshop MCM v5.12.3), use modified SubModule.xml.

Steam workshop subscription is KEPT (user unsubscribes manually after P6 verification).

.PARAMETER DryRun
No write operations - print what would be done.

.PARAMETER Force
Overwrite existing Modules\PlayerSettlement\ without backup prompt. Default behavior backs up to .bak-{timestamp}.
#>

[CmdletBinding()]
param(
  [switch]$DryRun,
  [switch]$Force
)

$ErrorActionPreference = 'Stop'

$RepoRoot = $PSScriptRoot
$GameRoot = "E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord"
$WorkshopPS = "E:\SteamLibrary\steamapps\workshop\content\261550\3720376888"
$LocalModule = Join-Path $GameRoot "Modules\PlayerSettlement"

$PSDll = Join-Path $RepoRoot "src\PlayerSettlement\bin\Release\PlayerSettlement.dll"
$PSPdb = Join-Path $RepoRoot "src\PlayerSettlement\bin\Release\PlayerSettlement.pdb"
$PSFixesDll = Join-Path $RepoRoot "src\PlayerSettlementFixes\bin\Release\PlayerSettlementFixes.dll"
$PSFixesPdb = Join-Path $RepoRoot "src\PlayerSettlementFixes\bin\Release\PlayerSettlementFixes.pdb"
$SubModuleXml = Join-Path $RepoRoot "SubModule.xml"
$VectorsDll = Join-Path $RepoRoot "src\PlayerSettlement\bin\Release\System.Numerics.Vectors.dll"

function Say($msg, $color = 'White') { Write-Host $msg -ForegroundColor $color }
function Fmt($n) { return ('{0:N0}' -f $n) }
function Do-Action($msg, [scriptblock]$block) {
  if ($DryRun) { Say "  [DRY-RUN] $msg" 'Yellow' }
  else { Say "  $msg" 'Gray'; & $block }
}

Say "=== PSLR P5 deploy ===" 'Cyan'
Say "DryRun=$DryRun Force=$Force"
Say ""

# ---- Preflight ----
Say "## Preflight" 'Cyan'
$missing = @()
foreach ($f in @($PSDll, $PSFixesDll, $SubModuleXml, $VectorsDll, $WorkshopPS)) {
  if (-not (Test-Path $f)) { $missing += $f }
}
if ($missing.Count -gt 0) {
  Say "FAIL - missing source files:" 'Red'
  $missing | ForEach-Object { Say "  $_" 'Red' }
  exit 1
}
$psLen = (Get-Item $PSDll).Length
$pfLen = (Get-Item $PSFixesDll).Length
Say "  OK: rebuild PS.dll ($(Fmt $psLen) B)"
Say "  OK: rebuild PSFixes.dll ($(Fmt $pfLen) B)"
Say "  OK: modified SubModule.xml"
Say "  OK: workshop PS source $WorkshopPS"
Say ""

# ---- Backup existing ----
Say "## Backup existing (if any)" 'Cyan'
if (Test-Path $LocalModule) {
  $stamp = Get-Date -Format "yyyyMMdd-HHmmss"
  $backup = "$LocalModule.bak-$stamp"
  if ($Force) {
    Do-Action "SKIP backup - Force passed - will overwrite $LocalModule" { }
  } else {
    Do-Action "BACKUP $LocalModule to $backup" {
      Move-Item $LocalModule $backup -Force
    }
  }
} else {
  Say "  OK: no existing $LocalModule - fresh install"
}
Say ""

# ---- Copy workshop -> local ----
Say "## Copy workshop to local Modules" 'Cyan'
Do-Action "mkdir $LocalModule" {
  New-Item -ItemType Directory -Path $LocalModule -Force | Out-Null
}

$excludes = @(
  'bin\Win64_Shipping_Client\MCMv5.dll',
  'bin\Win64_Shipping_Client\PlayerSettlement.dll',
  'bin\Win64_Shipping_Client\PlayerSettlement.pdb',
  'bin\Win64_Shipping_Client\PlayerSettlementFixes.dll',
  'bin\Win64_Shipping_Client\PlayerSettlementFixes.pdb',
  'SubModule.xml'
)
$excludedDirs = @('bin\Gaming.Desktop.x64_Shipping_Client')

$copyCount = 0
$skipCount = 0
Get-ChildItem $WorkshopPS -Recurse -File | ForEach-Object {
  $rel = $_.FullName.Substring($WorkshopPS.Length + 1)
  $skipDir = $false
  foreach ($d in $excludedDirs) { if ($rel.StartsWith($d)) { $skipDir = $true; break } }
  if ($skipDir -or ($excludes -contains $rel)) {
    $skipCount++
    return
  }
  $dest = Join-Path $LocalModule $rel
  if ($DryRun) {
    $copyCount++
  } else {
    $destDir = Split-Path $dest -Parent
    if (-not (Test-Path $destDir)) { New-Item -ItemType Directory -Path $destDir -Force | Out-Null }
    Copy-Item $_.FullName $dest -Force
    $copyCount++
  }
}
Say "  copy=$copyCount skip=$skipCount (excluded)"
Say ""

# ---- Overlay rebuild DLLs + modified SubModule.xml ----
Say "## Overlay rebuild artifacts" 'Cyan'
$binTarget = Join-Path $LocalModule "bin\Win64_Shipping_Client"
Do-Action "mkdir $binTarget (if needed)" {
  if (-not (Test-Path $binTarget)) { New-Item -ItemType Directory -Path $binTarget -Force | Out-Null }
}

Do-Action "cp PS.dll (rebuild v7.5.1 - 278 KB) to $binTarget\PlayerSettlement.dll" {
  Copy-Item $PSDll (Join-Path $binTarget "PlayerSettlement.dll") -Force
  if (Test-Path $PSPdb) { Copy-Item $PSPdb (Join-Path $binTarget "PlayerSettlement.pdb") -Force }
}

Do-Action "cp PSFixes.dll (rebuild v1.0.1 - 66 KB) to $binTarget\PlayerSettlementFixes.dll" {
  Copy-Item $PSFixesDll (Join-Path $binTarget "PlayerSettlementFixes.dll") -Force
  if (Test-Path $PSFixesPdb) { Copy-Item $PSFixesPdb (Join-Path $binTarget "PlayerSettlementFixes.pdb") -Force }
}

Do-Action "cp System.Numerics.Vectors.dll (NuGet - 115 KB) to $binTarget\" {
  Copy-Item $VectorsDll (Join-Path $binTarget "System.Numerics.Vectors.dll") -Force
}

Do-Action "cp modified SubModule.xml to $LocalModule\SubModule.xml" {
  Copy-Item $SubModuleXml (Join-Path $LocalModule "SubModule.xml") -Force
}
Say ""

# ---- Verify ----
if (-not $DryRun) {
  Say "## Verify" 'Cyan'
  $expect = @(
    @{ Path = "$LocalModule\SubModule.xml"; MinSize = 2000 },
    @{ Path = "$binTarget\PlayerSettlement.dll"; MinSize = 270000 },
    @{ Path = "$binTarget\PlayerSettlementFixes.dll"; MinSize = 60000 },
    @{ Path = "$binTarget\System.Numerics.Vectors.dll"; MinSize = 100000 },
    @{ Path = "$binTarget\0Harmony.dll"; MinSize = 2000000 }
  )
  $notBundled = Join-Path $binTarget "MCMv5.dll"
  $allOk = $true
  foreach ($e in $expect) {
    if (Test-Path $e.Path) {
      $size = (Get-Item $e.Path).Length
      $mod = (Get-Item $e.Path).LastWriteTime
      if ($size -ge $e.MinSize) {
        Say "  OK: $($e.Path) ($(Fmt $size) B, $mod)"
      } else {
        Say "  BAD: $($e.Path) too small ($size lt $($e.MinSize))" 'Red'
        $allOk = $false
      }
    } else {
      Say "  BAD: $($e.Path) MISSING" 'Red'
      $allOk = $false
    }
  }
  if (Test-Path $notBundled) {
    Say "  BAD: $notBundled STILL EXISTS (should have been excluded)" 'Red'
    $allOk = $false
  } else {
    Say "  OK: bundled MCMv5.dll not present (workshop MCM v5.12.3 will load exclusively)"
  }
  Say ""
  if ($allOk) {
    Say "=== P5 deploy OK ===" 'Green'
    Say ""
    Say "Next steps:" 'Cyan'
    Say "  1. Launch Bannerlord launcher, verify PlayerSettlement is listed and enabled"
    Say "  2. Start game, reach main menu without crash"
    Say "  3. (P6) Create new campaign, test build settlement"
    Say "  4. After P6 passes: Steam client -> unsubscribe workshop 3720376888"
    Say ""
    Say "Rollback (if P6 fails):" 'Yellow'
    Say "  Remove-Item -Recurse -Force '$LocalModule'"
    Say "  Steam auto-reloads workshop version on next launch"
  } else {
    Say "=== P5 deploy FAILED verification ===" 'Red'
    exit 1
  }
} else {
  Say "## DRY-RUN complete - no changes made" 'Yellow'
  Say "Re-run without -DryRun to execute."
}
