# PSCacheWarmup build + deploy.
# Mirror of RBMPlayerStaminaPoiseBuff\deploy.ps1 pattern.
param(
    [string]$Config = 'Release',
    [string]$GameRoot = 'E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord'
)

$ErrorActionPreference = 'Stop'
$here = $PSScriptRoot
$modName = 'PSCacheWarmup'

$srcDir = Join-Path $here 'src'
$subXml = Join-Path $here 'SubModule.xml'

$modulesDir = Join-Path $GameRoot 'Modules'
if (-not (Test-Path -LiteralPath $modulesDir)) {
    throw "Game Modules dir not found: $modulesDir"
}

$targetDir = Join-Path $modulesDir $modName
$binDir = Join-Path $targetDir 'bin\Win64_Shipping_Client'

$launcher = Get-Process -Name 'TaleWorlds.MountAndBlade.Launcher' -ErrorAction SilentlyContinue
if ($launcher) {
    Write-Warning "Launcher running (PID $($launcher.Id)); it may lock $modName.dll."
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

$dllOut = Join-Path $srcDir "bin\$Config\$modName.dll"
if (-not (Test-Path $dllOut)) { throw "Built DLL not found at $dllOut" }
Copy-Item -Path $dllOut -Destination $binDir -Force
Write-Host "Copied $modName.dll -> $binDir" -ForegroundColor Green

$pdbOut = Join-Path $srcDir "bin\$Config\$modName.pdb"
if (Test-Path $pdbOut) {
    Copy-Item -Path $pdbOut -Destination $binDir -Force
    Write-Host "Copied $modName.pdb -> $binDir" -ForegroundColor Green
}

Write-Host "`nDeploy done. Enable 'PS Cache Warmup' in launcher (loads after PlayerSettlement if PS is installed)." -ForegroundColor Yellow
