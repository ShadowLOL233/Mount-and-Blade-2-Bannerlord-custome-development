param(
    [string]$Config = 'Release',
    [string]$GameRoot = 'E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord'
)

$ErrorActionPreference = 'Stop'
$here = $PSScriptRoot
$modName = 'PlayerFiefRecruitBoost'

$srcDir = Join-Path $here 'src'
$subXml = Join-Path $here 'SubModule.xml'
$moduleDataSrc = Join-Path $here 'ModuleData'

$modulesDir = Join-Path $GameRoot 'Modules'
if (-not (Test-Path $modulesDir)) { throw "Game Modules dir not found: $modulesDir" }

$targetDir = Join-Path $modulesDir $modName
$binDir = Join-Path $targetDir 'bin\Win64_Shipping_Client'

$launcher = Get-Process -Name 'TaleWorlds.MountAndBlade.Launcher' -ErrorAction SilentlyContinue
if ($launcher) {
    Write-Warning "Bannerlord launcher is running (PID $($launcher.Id)). It may lock $modName.dll and cause the copy to fail. Close it first."
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

if (Test-Path $moduleDataSrc) {
    $moduleDataDst = Join-Path $targetDir 'ModuleData'
    if (-not (Test-Path $moduleDataDst)) {
        New-Item -ItemType Directory -Path $moduleDataDst | Out-Null
    }
    Get-ChildItem $moduleDataSrc -File | ForEach-Object {
        $dst = Join-Path $moduleDataDst $_.Name
        # Don't overwrite user's edited config.xml if it already exists
        if ($_.Name -eq 'config.xml' -and (Test-Path $dst)) {
            Write-Host "Skipped config.xml (already exists at destination)" -ForegroundColor DarkGray
        } else {
            Copy-Item -Path $_.FullName -Destination $dst -Force
            Write-Host "Copied $($_.Name) -> $moduleDataDst" -ForegroundColor Green
        }
    }
}

$dllOut = Join-Path $srcDir "bin\$Config\$modName.dll"
if (-not (Test-Path $dllOut)) { throw "Built DLL not found at $dllOut" }
Copy-Item -Path $dllOut -Destination $binDir -Force
Write-Host "Copied $modName.dll -> $binDir" -ForegroundColor Green

$pdbOut = Join-Path $srcDir "bin\$Config\$modName.pdb"
if (Test-Path $pdbOut) {
    Copy-Item -Path $pdbOut -Destination $binDir -Force
    Write-Host "Copied $modName.pdb -> $binDir" -ForegroundColor Green
}

Write-Host "`nDeploy done. Enable 'Player Fief Recruit Boost' in the Bannerlord launcher Mods tab." -ForegroundColor Yellow
Write-Host "Tune values in Modules\$modName\ModuleData\config.xml if desired." -ForegroundColor DarkGray
