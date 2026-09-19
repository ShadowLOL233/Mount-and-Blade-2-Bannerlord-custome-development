# Tier6Injector build + deploy
# Requires: .NET SDK (any version that can target net472). Game installed at $gameRoot.
# Run from repo root or Tier6Injector/ (uses $PSScriptRoot).

param(
    [string]$Config = 'Release',
    [string]$GameRoot = 'E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord'
)

$ErrorActionPreference = 'Stop'
$here = $PSScriptRoot
$modName = 'Tier6Injector'

$srcDir = Join-Path $here 'src'
$subXml = Join-Path $here 'SubModule.xml'

$modulesDir = Join-Path $GameRoot 'Modules'
if (-not (Test-Path $modulesDir)) {
    throw "Game Modules dir not found: $modulesDir"
}

$targetDir = Join-Path $modulesDir $modName
$binDir = Join-Path $targetDir 'bin\Win64_Shipping_Client'

# --- Warn if launcher is running (it locks mod DLLs) ---
$launcher = Get-Process -Name 'TaleWorlds.MountAndBlade.Launcher' -ErrorAction SilentlyContinue
if ($launcher) {
    Write-Warning "Bannerlord launcher is running (PID $($launcher.Id)). It may lock $modName.dll and cause the copy to fail. Close it first."
}

# --- Build ---
Write-Host "Building $modName ($Config)..." -ForegroundColor Cyan
Push-Location $srcDir
try {
    dotnet build -c $Config
    if ($LASTEXITCODE -ne 0) { throw "dotnet build failed with exit $LASTEXITCODE" }
}
finally { Pop-Location }

# --- Prepare target ---
New-Item -ItemType Directory -Force -Path $binDir | Out-Null

# --- Copy SubModule.xml ---
Copy-Item -Path $subXml -Destination $targetDir -Force
Write-Host "Copied SubModule.xml -> $targetDir" -ForegroundColor Green

# --- Copy DLL ---
$dllOut = Join-Path $srcDir "bin\$Config\Tier6Injector.dll"
if (-not (Test-Path $dllOut)) {
    throw "Built DLL not found at $dllOut"
}
Copy-Item -Path $dllOut -Destination $binDir -Force
Write-Host "Copied Tier6Injector.dll -> $binDir" -ForegroundColor Green

# --- Optional PDB for debugging ---
$pdbOut = Join-Path $srcDir "bin\$Config\Tier6Injector.pdb"
if (Test-Path $pdbOut) {
    Copy-Item -Path $pdbOut -Destination $binDir -Force
    Write-Host "Copied Tier6Injector.pdb -> $binDir" -ForegroundColor Green
}

Write-Host "`nDeploy done. Enable 'Tier6 Injector' in the Bannerlord launcher Mods tab." -ForegroundColor Yellow
