# MapBlockadePSBridge build + deploy
# Prereq: .NET SDK, MapBlockade + PlayerSettlement installed in game Modules.

param(
    [string]$Config = 'Release',
    [string]$GameRoot = 'E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord'
)

$ErrorActionPreference = 'Stop'
$here = $PSScriptRoot
$modName = 'MapBlockadePSBridge'

$srcDir = Join-Path $here 'src'
$subXml = Join-Path $here 'SubModule.xml'
$modulesDir = Join-Path $GameRoot 'Modules'

if (-not (Test-Path $modulesDir)) { throw "Game Modules dir not found: $modulesDir" }

$targetDir = Join-Path $modulesDir $modName
$binDir = Join-Path $targetDir 'bin\Win64_Shipping_Client'

$launcher = Get-Process -Name 'TaleWorlds.MountAndBlade.Launcher' -ErrorAction SilentlyContinue
if ($launcher) {
    Write-Warning "Bannerlord launcher running (PID $($launcher.Id)); DLL copy may fail. Close it first."
}

Write-Host "Building $modName ($Config)..." -ForegroundColor Cyan
Push-Location $srcDir
try {
    dotnet build -c $Config
    if ($LASTEXITCODE -ne 0) { throw "dotnet build failed with exit $LASTEXITCODE" }
}
finally { Pop-Location }

New-Item -ItemType Directory -Force -Path $binDir | Out-Null

Copy-Item -Path $subXml -Destination $targetDir -Force
Write-Host "Copied SubModule.xml -> $targetDir" -ForegroundColor Green

$dllOut = Join-Path $srcDir "bin\$Config\MapBlockadePSBridge.dll"
if (-not (Test-Path $dllOut)) { throw "Built DLL not found at $dllOut" }
Copy-Item -Path $dllOut -Destination $binDir -Force
Write-Host "Copied MapBlockadePSBridge.dll -> $binDir" -ForegroundColor Green

$pdbOut = Join-Path $srcDir "bin\$Config\MapBlockadePSBridge.pdb"
if (Test-Path $pdbOut) {
    Copy-Item -Path $pdbOut -Destination $binDir -Force
    Write-Host "Copied MapBlockadePSBridge.pdb -> $binDir" -ForegroundColor Green
}

Write-Host "`nDeploy done. In the launcher: enable 'MapBlockade x PlayerSettlement Bridge'." -ForegroundColor Yellow
Write-Host "Load order: MapBlockade and PlayerSettlement must load BEFORE this bridge." -ForegroundColor Yellow
