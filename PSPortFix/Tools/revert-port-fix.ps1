# PSPortFix · revert-port-fix.ps1
# Restores PS workshop XML files from Baselines/

$ErrorActionPreference = 'Stop'

$psWorkshopRoot = "E:\SteamLibrary\steamapps\workshop\content\261550\3720376888\ModuleData\Player_Settlement_Templates"
$baselineDir = Join-Path $PSScriptRoot "..\Baselines"

if (-not (Test-Path $baselineDir)) {
  Write-Host "ERROR: No baselines found at $baselineDir" -ForegroundColor Red
  Write-Host "Nothing to revert. Was apply-port-fix.ps1 ever run?" -ForegroundColor Yellow
  exit 1
}

$cultures = @('empire','vlandia','battania','sturgia','aserai','khuzait')
$restored = 0

foreach ($culture in $cultures) {
  $baselineFile = Join-Path $baselineDir "${culture}_settlements_templates_default.xml.baseline"
  $targetFile = Join-Path $psWorkshopRoot "${culture}_settlements_templates_default.xml"

  if (-not (Test-Path $baselineFile)) {
    Write-Host "SKIP: no baseline for $culture" -ForegroundColor DarkGray
    continue
  }

  Copy-Item $baselineFile $targetFile -Force
  $hash = (Get-FileHash $targetFile -Algorithm SHA256).Hash
  Write-Host "Restored: $culture (sha256=$($hash.Substring(0,16))...)" -ForegroundColor Green
  $restored++
}

Write-Host ""
Write-Host "=== Revert complete ===" -ForegroundColor Cyan
Write-Host "Cultures restored: $restored / 6"
Write-Host ""
Write-Host "Warning: PS default towns now revert to original state (crash on Enter Port)."
Write-Host "Baselines kept in $baselineDir for future re-apply."
