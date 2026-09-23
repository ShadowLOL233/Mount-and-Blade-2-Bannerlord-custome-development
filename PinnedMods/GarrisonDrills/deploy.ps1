param(
    [string]$Config = 'Release',
    [string]$GameRoot = 'E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord'
)

$ErrorActionPreference = 'Stop'
$here = $PSScriptRoot

$srcDir = Join-Path $here 'src'
$subXml = Join-Path $here 'SubModule.xml'
$moduleDataSrc = Join-Path $here 'ModuleData'

$modulesDir = Join-Path $GameRoot 'Modules'
if (-not (Test-Path $modulesDir)) { throw "Game Modules dir not found: $modulesDir" }

$targetDir = Join-Path $modulesDir 'GarrisonDrillsPinned'
$binDir = Join-Path $targetDir 'bin\Win64_Shipping_Client'

$launcher = Get-Process -Name 'TaleWorlds.MountAndBlade.Launcher' -ErrorAction SilentlyContinue
if ($launcher) {
    Write-Warning "Bannerlord launcher is running (PID $($launcher.Id)). It may lock GarrisonDrills.dll and cause the copy to fail. Close it first."
}

Write-Host "Building GarrisonDrills recompile ($Config)..." -ForegroundColor Cyan
Push-Location $srcDir
try {
    dotnet build -c $Config
    if ($LASTEXITCODE -ne 0) { throw "dotnet build failed with exit $LASTEXITCODE" }
}
finally { Pop-Location }

New-Item -ItemType Directory -Force -Path $binDir | Out-Null

Copy-Item -Path $subXml -Destination $targetDir -Force
Write-Host "Copied SubModule.xml -> $targetDir" -ForegroundColor Green

if (Test-Path $moduleDataSrc) {
    $moduleDataDst = Join-Path $targetDir 'ModuleData'
    if (Test-Path $moduleDataDst) { Remove-Item -Recurse -Force $moduleDataDst }
    Copy-Item -Path $moduleDataSrc -Destination $targetDir -Recurse -Force
    Write-Host "Copied ModuleData/ -> $targetDir" -ForegroundColor Green
}

$dllOut = Join-Path $srcDir "bin\$Config\GarrisonDrills.dll"
if (-not (Test-Path $dllOut)) { throw "Built DLL not found at $dllOut" }
Copy-Item -Path $dllOut -Destination $binDir -Force
Write-Host "Copied GarrisonDrills.dll (source-built) -> $binDir" -ForegroundColor Green

$pdbOut = Join-Path $srcDir "bin\$Config\GarrisonDrills.pdb"
if (Test-Path $pdbOut) {
    Copy-Item -Path $pdbOut -Destination $binDir -Force
    Write-Host "Copied GarrisonDrills.pdb -> $binDir" -ForegroundColor Green
}

Write-Host "`nDeploy done." -ForegroundColor Yellow
