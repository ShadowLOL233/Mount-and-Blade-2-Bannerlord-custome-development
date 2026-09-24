# PSPortFix · verify-port-fix.ps1
# Checks whether PS workshop XMLs are currently patched with port location.
# Reports any missing coverage (Steam update may have overwritten).

$ErrorActionPreference = 'Continue'

$psWorkshopRoot = "E:\SteamLibrary\steamapps\workshop\content\261550\3720376888\ModuleData\Player_Settlement_Templates"
$baselineDir = Join-Path $PSScriptRoot "..\Baselines"

if (-not (Test-Path $psWorkshopRoot)) {
  Write-Host "ERROR: PS workshop path not found" -ForegroundColor Red
  exit 1
}

$cultures = @('empire','vlandia','battania','sturgia','aserai','khuzait')
$allOK = $true
$totalTowns = 0
$totalWithPort = 0
$totalWithMarker = 0

Write-Host "=== PSPortFix Verification ===" -ForegroundColor Cyan

foreach ($culture in $cultures) {
  $file = Join-Path $psWorkshopRoot "${culture}_settlements_templates_default.xml"
  if (-not (Test-Path $file)) {
    Write-Host "$culture : file not found" -ForegroundColor Yellow
    continue
  }

  $content = Get-Content $file -Raw
  $hasMarker = $content -match '<!-- PSPortFix v1 -->'

  # Count town settlements
  $townMatches = [regex]::Matches($content, '<Settlement[^>]*template_type="Town"[^>]*?>.*?</Settlement>', 'Singleline')
  $townCount = $townMatches.Count

  # Count towns with port location
  $portCount = 0
  foreach ($m in $townMatches) {
    if ($m.Value -match '<Location id="port"') {
      $portCount++
    }
  }

  $totalTowns += $townCount
  $totalWithPort += $portCount

  $markerStatus = if ($hasMarker) { "[MARKER OK]" } else { "[NO MARKER]" }
  $coverage = if ($townCount -gt 0) { "$portCount/$townCount towns have port" } else { "no towns" }

  if ($portCount -eq $townCount -and $hasMarker) {
    Write-Host "$culture : $coverage $markerStatus" -ForegroundColor Green
  } else {
    Write-Host "$culture : $coverage $markerStatus" -ForegroundColor Red
    $allOK = $false
  }

  # Check baseline still matches (detect if Steam updated PS)
  $baselineFile = Join-Path $baselineDir "${culture}_settlements_templates_default.xml.baseline"
  if (Test-Path $baselineFile) {
    $baselineContent = Get-Content $baselineFile -Raw
    # Strip marker from current before comparing
    $strippedCurrent = $content -replace '<Location id="port"[^/]*/><!-- PSPortFix v1 -->', ''
    if ($strippedCurrent.Length -ne $baselineContent.Length) {
      $diff = $strippedCurrent.Length - $baselineContent.Length
      Write-Host "  Baseline size diff: $diff bytes (Steam may have updated PS since apply)" -ForegroundColor Yellow
    }
  }
}

Write-Host ""
if ($allOK) {
  Write-Host "=== ALL OK ===" -ForegroundColor Green
  Write-Host "Total: $totalWithPort / $totalTowns towns patched"
  Write-Host "Safe to enter PS town ports."
} else {
  Write-Host "=== ATTENTION NEEDED ===" -ForegroundColor Red
  Write-Host "Total: $totalWithPort / $totalTowns towns patched"
  Write-Host "Re-run .\apply-port-fix.ps1 to fix coverage."
  exit 2
}
