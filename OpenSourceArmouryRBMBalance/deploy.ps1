# Open Source Armoury RBM Balance Patch - deploy to game Modules dir.
# Copies SubModule.xml + ModuleData/* to Modules/OpenSourceArmouryRBMBalance/.
# Pure XML mod, no build step needed.

param(
    [string]$GameRoot = 'E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord'
)

$ErrorActionPreference = 'Stop'
$here = $PSScriptRoot
$modName = 'OpenSourceArmouryRBMBalance'

$modulesDir = Join-Path $GameRoot 'Modules'
if (-not (Test-Path -LiteralPath $modulesDir)) {
    throw "Game Modules dir not found: $modulesDir"
}

$targetDir = Join-Path $modulesDir $modName

# Warn if launcher is running (locks nothing here, but user should know).
$launcher = Get-Process -Name 'TaleWorlds.MountAndBlade.Launcher' -ErrorAction SilentlyContinue
if ($launcher) {
    Write-Warning "Bannerlord launcher is running (PID $($launcher.Id)). XMLs will be picked up next launch."
}

# Fresh directory.
if (Test-Path -LiteralPath $targetDir) {
    Remove-Item -LiteralPath $targetDir -Recurse -Force
}
New-Item -ItemType Directory -Path $targetDir | Out-Null
New-Item -ItemType Directory -Path (Join-Path $targetDir 'ModuleData') | Out-Null

# Copy SubModule.xml
Copy-Item -LiteralPath (Join-Path $here 'SubModule.xml') -Destination $targetDir -Force
Write-Host "Copied SubModule.xml -> $targetDir" -ForegroundColor Green

# Copy ModuleData XMLs
$srcModData = Join-Path $here 'ModuleData'
Get-ChildItem $srcModData -Filter '*.xml' -File | ForEach-Object {
    Copy-Item -LiteralPath $_.FullName -Destination (Join-Path $targetDir 'ModuleData') -Force
    Write-Host "Copied $($_.Name) -> $targetDir\ModuleData\" -ForegroundColor Green
}

Write-Host "`nDeploy done. Enable 'Open Source Armoury RBM Balance Patch' in the Bannerlord launcher (load after OSA/OSW/RBM)." -ForegroundColor Yellow
