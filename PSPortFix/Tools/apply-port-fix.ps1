# PSPortFix · apply-port-fix.ps1
# Injects <Location id="port" scene_name="{culture}_shipyard"/> into all PS default town variants
# Fixes crash on entering port menu for river/inland PS towns.

$ErrorActionPreference = 'Stop'

$psWorkshopRoot = "E:\SteamLibrary\steamapps\workshop\content\261550\3720376888\ModuleData\Player_Settlement_Templates"
$baselineDir = Join-Path $PSScriptRoot "..\Baselines"

if (-not (Test-Path $psWorkshopRoot)) {
  Write-Host "ERROR: PS workshop path not found: $psWorkshopRoot" -ForegroundColor Red
  Write-Host "Verify PlayerSettlement (workshop id 3720376888) is subscribed." -ForegroundColor Yellow
  exit 1
}

$cultures = @('empire','vlandia','battania','sturgia','aserai','khuzait')

if (-not (Test-Path $baselineDir)) {
  New-Item -ItemType Directory -Path $baselineDir -Force | Out-Null
  Write-Host "Created Baselines directory: $baselineDir" -ForegroundColor Cyan
}

$totalTownsPatched = 0
$culturesProcessed = 0

foreach ($culture in $cultures) {
  $sourceFile = Join-Path $psWorkshopRoot "${culture}_settlements_templates_default.xml"
  $baselineFile = Join-Path $baselineDir "${culture}_settlements_templates_default.xml.baseline"

  if (-not (Test-Path $sourceFile)) {
    Write-Host "SKIP: $sourceFile not found" -ForegroundColor Yellow
    continue
  }

  # Backup only if baseline doesn't exist
  if (-not (Test-Path $baselineFile)) {
    Copy-Item $sourceFile $baselineFile -Force
    $hash = (Get-FileHash $baselineFile -Algorithm SHA256).Hash
    Write-Host "Baseline saved: $culture (sha256=$($hash.Substring(0,16))...)" -ForegroundColor Green
  } else {
    Write-Host "Baseline exists for $culture, keeping original" -ForegroundColor DarkGray
  }

  # Read current content
  $content = Get-Content $sourceFile -Raw

  # Check if already patched (contains our injection marker)
  if ($content -match '<!-- PSPortFix v1 -->') {
    Write-Host "Already patched: $culture (skipping)" -ForegroundColor DarkGray
    continue
  }

  $portScene = "${culture}_shipyard"
  $injection = "<Location id=`"port`" scene_name=`"$portScene`" /><!-- PSPortFix v1 -->"

  # Match all Town settlement blocks, inject before </Locations>
  # Pattern: <Settlement ... template_type="Town" ...>...<Locations ...>...</Locations>...</Settlement>
  $townPattern = '(<Settlement[^>]*template_type="Town"[^>]*?>.*?<Locations[^>]*?>)((?:(?!</Locations>).)*)(</Locations>.*?</Settlement>)'
  $regex = [regex]::new($townPattern, 'Singleline')

  $count = 0
  $newContent = $regex.Replace($content, {
    param($m)
    $script:count++
    return $m.Groups[1].Value + $m.Groups[2].Value + $injection + $m.Groups[3].Value
  })

  # Write back
  Set-Content -Path $sourceFile -Value $newContent -NoNewline -Encoding UTF8

  Write-Host "Patched: $culture - injected port location into $count town variants" -ForegroundColor Green
  $totalTownsPatched += $count
  $culturesProcessed++
}

Write-Host ""
Write-Host "=== PSPortFix v1.0.0 Applied ===" -ForegroundColor Cyan
Write-Host "Cultures processed: $culturesProcessed / 6"
Write-Host "Total town variants patched: $totalTownsPatched"
Write-Host ""
Write-Host "Next steps:"
Write-Host "  1. Close Bannerlord if running"
Write-Host "  2. Launch game and enter Aetofolia (or any PS town)"
Write-Host "  3. Click 'Enter Port' - should now load empire_shipyard scene without crash"
Write-Host ""
Write-Host "If Steam updates PlayerSettlement in the future:"
Write-Host "  - Run .\verify-port-fix.ps1 to detect override"
Write-Host "  - Re-run .\apply-port-fix.ps1 to re-apply"
