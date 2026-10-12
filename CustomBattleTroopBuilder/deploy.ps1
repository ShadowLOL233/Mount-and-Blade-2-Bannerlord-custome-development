<#
.SYNOPSIS
CustomBattleTroopBuilder (CBTB) deploy script

.DESCRIPTION
Build Release + copy SubModule.xml + DLL + pdb to Modules\CustomBattleTroopBuilder\.
Phase 1 scaffold: only ships the SubModule.cs skeleton. No GUI/Prefabs/ModuleData yet.

.PARAMETER DryRun
No write operations.

.EXAMPLE
.\deploy.ps1
.\deploy.ps1 -DryRun
#>

[CmdletBinding()]
param(
  [switch]$DryRun
)

$ErrorActionPreference = 'Stop'

$RepoRoot = $PSScriptRoot
$GameRoot = "E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord"
$LocalModule = Join-Path $GameRoot "Modules\CustomBattleTroopBuilder"
$BinTarget = Join-Path $LocalModule "bin\Win64_Shipping_Client"

$SubModuleXml = Join-Path $RepoRoot "SubModule.xml"
$Csproj = Join-Path $RepoRoot "src\CustomBattleTroopBuilder.csproj"
$Dll = Join-Path $RepoRoot "src\bin\Release\CustomBattleTroopBuilder.dll"
$Pdb = Join-Path $RepoRoot "src\bin\Release\CustomBattleTroopBuilder.pdb"

function Say($msg, $color = 'White') { Write-Host $msg -ForegroundColor $color }

Say "=== CBTB deploy ===" 'Cyan'
Say "DryRun=$DryRun"

Say "## Build (Release)" 'Cyan'
if (-not $DryRun) {
  Push-Location (Split-Path $Csproj -Parent)
  try {
    $buildOutput = & dotnet build -c Release 2>&1 | Out-String
    if ($LASTEXITCODE -ne 0) {
      Say "BUILD FAILED:" 'Red'
      Say $buildOutput 'Red'
      exit 1
    }
    Say "  build OK"
  } finally {
    Pop-Location
  }
}

Say "## Preflight (artifacts)" 'Cyan'
foreach ($f in @($SubModuleXml, $Dll)) {
  if (-not (Test-Path $f)) {
    Say "FAIL missing: $f" 'Red'
    exit 1
  }
}
Say "  OK: SubModule.xml + DLL $('{0:N0}' -f (Get-Item $Dll).Length) B"

Say "## Deploy to $LocalModule" 'Cyan'
if (-not $DryRun) {
  if (-not (Test-Path $BinTarget)) {
    New-Item -ItemType Directory -Path $BinTarget -Force | Out-Null
    Say "  mkdir $BinTarget"
  }
  Copy-Item $SubModuleXml (Join-Path $LocalModule "SubModule.xml") -Force
  Copy-Item $Dll (Join-Path $BinTarget "CustomBattleTroopBuilder.dll") -Force
  if (Test-Path $Pdb) { Copy-Item $Pdb (Join-Path $BinTarget "CustomBattleTroopBuilder.pdb") -Force }
  # Phase 2b-1: copy GUI/ (Prefabs) · needed by GauntletLayer.LoadMovie
  $GuiSrc = Join-Path $RepoRoot "GUI"
  if (Test-Path $GuiSrc) {
    $GuiDst = Join-Path $LocalModule "GUI"
    if (Test-Path $GuiDst) { Remove-Item $GuiDst -Recurse -Force }
    Copy-Item $GuiSrc $GuiDst -Recurse -Force
    $prefabCount = (Get-ChildItem $GuiDst -Recurse -Filter '*.xml' | Measure-Object).Count
    Say "  copied GUI/ · $prefabCount prefab XML"
  }
  Say "  copied SubModule.xml + DLL + pdb"
} else {
  Say "  [DRY-RUN] would copy to $LocalModule" 'Yellow'
}

Say "## Verify" 'Cyan'
if (-not $DryRun) {
  $expected = @(
    (Join-Path $LocalModule "SubModule.xml"),
    (Join-Path $BinTarget "CustomBattleTroopBuilder.dll")
  )
  foreach ($e in $expected) {
    if (Test-Path $e) {
      $fi = Get-Item $e
      Say "  OK: $($fi.Name) $('{0:N0}' -f $fi.Length) B at $($fi.LastWriteTime)"
    } else {
      Say "  BAD: $e MISSING" 'Red'
      exit 1
    }
  }
  Say "=== deploy OK ===" 'Green'
  Say "Enable in launcher: 'Custom Battle Troop Builder'"
  Say "Phase 1 verification: launch game -> main menu -> message 'CBTB v0.1.0-scaffold loaded'"
  Say "Hotkey Ctrl+Shift+B anywhere in main menu -> info message proves hotkey works"
  Say "Logs: C:\ProgramData\Mount and Blade II Bannerlord\logs\CustomBattleTroopBuilder.log"
}
