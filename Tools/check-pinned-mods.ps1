param(
    [string]$WorkshopRoot = "E:\SteamLibrary\steamapps\workshop\content\261550",
    [string]$PinnedRoot   = (Join-Path $PSScriptRoot '..\PinnedMods')
)

$ErrorActionPreference = 'Stop'
$PinnedRoot = (Resolve-Path $PinnedRoot).Path

$alerts = @()
$mods = Get-ChildItem -Path $PinnedRoot -Directory

foreach ($mod in $mods) {
    $baselinePath = Join-Path $mod.FullName 'baseline.json'
    if (-not (Test-Path $baselinePath)) {
        Write-Host "SKIP $($mod.Name) : no baseline.json" -ForegroundColor DarkGray
        continue
    }
    $baseline = Get-Content $baselinePath -Raw | ConvertFrom-Json
    $wsId = $baseline.workshop_id
    $wsDir = Join-Path $WorkshopRoot $wsId
    if (-not (Test-Path $wsDir)) {
        Write-Host "WARN $($mod.Name) : workshop $wsId not installed" -ForegroundColor Yellow
        continue
    }

    $currentDll = Get-ChildItem -Path $wsDir -Recurse -Filter '*.dll' -ErrorAction SilentlyContinue |
                  Where-Object { $_.FullName -match 'bin\\Win64_Shipping_Client' } |
                  Select-Object -First 1
    if (-not $currentDll) {
        Write-Host "WARN $($mod.Name) : no DLL in workshop $wsId" -ForegroundColor Yellow
        continue
    }
    $currentHash = (Get-FileHash $currentDll.FullName -Algorithm SHA256).Hash
    $baselineHash = $baseline.workshop_dll_sha256_at_pin

    if ($currentHash -eq $baselineHash) {
        Write-Host "OK   $($mod.Name) : workshop DLL unchanged ($($baseline.pinned_from_workshop_version))" -ForegroundColor Green
    } else {
        Write-Host "STALE $($mod.Name) : workshop DLL bumped!" -ForegroundColor Red
        Write-Host "     baseline: $baselineHash ($($baseline.pinned_from_workshop_version))" -ForegroundColor Red
        Write-Host "     current : $currentHash" -ForegroundColor Red
        $alerts += "$($mod.Name): workshop_dll_sha256 changed from $baselineHash to $currentHash"
    }
}

if ($alerts.Count -gt 0) {
    $alertFile = Join-Path $PinnedRoot 'UPDATE_ALERTS.md'
    $lines = @("# PinnedMods Update Alerts", "", "Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')", "") + ($alerts | ForEach-Object { "- $_" })
    Set-Content -Path $alertFile -Value $lines -Encoding utf8
    Write-Host ""
    Write-Host "-> $alertFile written ($($alerts.Count) alert(s))" -ForegroundColor Yellow
    exit 2
}
