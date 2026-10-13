# Open Source Armoury RBM Balance Patch - build + deploy.
# Builds OSABalanceRBMBridge.dll from src/, then copies SubModule.xml + ModuleData/*
# + bin/Win64_Shipping_Client/* to Modules/OpenSourceArmouryRBMBalance/.

param(
    [string]$Config = 'Release',
    [string]$GameRoot,
    [switch]$SkipBuild
)

$ErrorActionPreference = 'Stop'
$here = $PSScriptRoot
$modName = 'OpenSourceArmouryRBMBalance'

if (-not $GameRoot) {
    $candidates = @(
        'E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord',
        'D:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord'
    )
    $GameRoot = $candidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
    if (-not $GameRoot) { throw "No game install found. Pass -GameRoot explicitly." }
}

$modulesDir = Join-Path $GameRoot 'Modules'
if (-not (Test-Path -LiteralPath $modulesDir)) {
    throw "Game Modules dir not found: $modulesDir"
}

$targetDir = Join-Path $modulesDir $modName
$targetBin = Join-Path $targetDir 'bin\Win64_Shipping_Client'
$targetModData = Join-Path $targetDir 'ModuleData'

# Warn if launcher is running (may hold DLL file lock).
$launcher = Get-Process -Name 'TaleWorlds.MountAndBlade.Launcher' -ErrorAction SilentlyContinue
if ($launcher) {
    Write-Warning "Bannerlord launcher running (PID $($launcher.Id)); it may lock OSABalanceRBMBridge.dll."
}

# Build step
$srcDir = Join-Path $here 'src'
$dllOut = Join-Path $srcDir "bin\$Config\OSABalanceRBMBridge.dll"
$pdbOut = Join-Path $srcDir "bin\$Config\OSABalanceRBMBridge.pdb"

if (-not $SkipBuild) {
    Write-Host "Building OSABalanceRBMBridge ($Config)..." -ForegroundColor Cyan
    # Pass BannerlordBin + WorkshopRoot derived from $GameRoot so D:/E: both work.
    # $GameRoot = <drive>\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord
    #   → steamapps = parent of parent → .../SteamLibrary/steamapps
    #   → workshop  = $steamapps\workshop\content\261550
    $env:BannerlordBin = Join-Path $GameRoot 'bin\Win64_Shipping_Client'
    $steamapps = Split-Path (Split-Path $GameRoot) -Parent
    $env:WorkshopRoot = Join-Path $steamapps 'workshop\content\261550'
    if (-not (Test-Path -LiteralPath (Join-Path $env:WorkshopRoot '2859188632\bin\Win64_Shipping_Client\0Harmony.dll'))) {
        throw "0Harmony.dll not found under derived WorkshopRoot='$($env:WorkshopRoot)'. Pass -GameRoot explicitly or subscribe Harmony (workshop id 2859188632)."
    }
    Push-Location $srcDir
    try {
        dotnet build -c $Config
        if ($LASTEXITCODE -ne 0) { throw "dotnet build failed with exit $LASTEXITCODE" }
    }
    finally { Pop-Location }
    if (-not (Test-Path $dllOut)) { throw "Built DLL not found at $dllOut" }
}

# Fresh module directory.
if (Test-Path -LiteralPath $targetDir) {
    Remove-Item -LiteralPath $targetDir -Recurse -Force
}
New-Item -ItemType Directory -Path $targetDir | Out-Null
New-Item -ItemType Directory -Path $targetModData | Out-Null
New-Item -ItemType Directory -Path $targetBin | Out-Null

# SubModule.xml
Copy-Item -LiteralPath (Join-Path $here 'SubModule.xml') -Destination $targetDir -Force
Write-Host "Copied SubModule.xml -> $targetDir" -ForegroundColor Green

# ModuleData XMLs
$srcModData = Join-Path $here 'ModuleData'
Get-ChildItem $srcModData -Filter '*.xml' -File | ForEach-Object {
    Copy-Item -LiteralPath $_.FullName -Destination $targetModData -Force
    Write-Host "Copied $($_.Name) -> ModuleData\" -ForegroundColor Green
}

# DLL + PDB
Copy-Item -LiteralPath $dllOut -Destination $targetBin -Force
Write-Host "Copied OSABalanceRBMBridge.dll -> bin\Win64_Shipping_Client\" -ForegroundColor Green
if (Test-Path $pdbOut) {
    Copy-Item -LiteralPath $pdbOut -Destination $targetBin -Force
    Write-Host "Copied OSABalanceRBMBridge.pdb -> bin\Win64_Shipping_Client\" -ForegroundColor Green
}

Write-Host "`nDeploy done. Enable 'Open Source Armoury RBM Balance Patch' in the launcher (loads after OSA/OSW/RBM)." -ForegroundColor Yellow
Write-Host "Smoke test: on main menu, look for green 'OSABalance RBMBridge: PBS 9->11, CFM 16->22, POM 20->27' message." -ForegroundColor Yellow
