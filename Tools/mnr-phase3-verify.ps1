param(
    [string]$Log = 'E:\Bannerlord-UserData\Mount and Blade II Bannerlord\Configs\ModLogs\PSCacheWarmup.log'
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path $Log)) {
    Write-Host "FAIL: log not found at $Log" -ForegroundColor Red
    Write-Host "  Meaning: PSCacheWarmup DLL did not load, or OnSubModuleLoad hook did not fire, or game has not started once yet." -ForegroundColor Yellow
    exit 1
}

$content = Get-Content $Log -Raw
$lines = Get-Content $Log

Write-Host ""
Write-Host "=== PSCacheWarmup Phase 3 verify (log: $Log) ===" -ForegroundColor Cyan
Write-Host "  size:      $((Get-Item $Log).Length) bytes"
Write-Host "  modified:  $((Get-Item $Log).LastWriteTime)"
Write-Host "  linecount: $($lines.Count)"
Write-Host ""

$pass = 0
$fail = 0

function Check {
    param([string]$label, [string]$pattern, [string]$hint)
    $found = $content -match $pattern
    if ($found) {
        $script:pass++
        Write-Host ("  [OK]   {0}" -f $label) -ForegroundColor Green
    }
    else {
        $script:fail++
        Write-Host ("  [MISS] {0}" -f $label) -ForegroundColor Yellow
        Write-Host ("         hint: {0}" -f $hint) -ForegroundColor DarkGray
    }
}

Check 'Layer 1: OnSubModuleLoad called (DLL loaded)' `
      'OnSubModuleLoad called' `
      'PSCacheWarmup DLL did not load -- check launcher IsSelected=true, DLL exists in Modules\PSCacheWarmup\bin\Win64_Shipping_Client\'

Check 'Layer 2: OnGameInitializationFinished (Campaign entered)' `
      'OnGameInitializationFinished called' `
      'Game did not reach Campaign init -- did you start a new campaign or load a save?'

Check 'Layer 3: TrySubscribeToPS SUCCEEDED (PS event subscribed)' `
      'SUBSCRIBE SUCCEEDED' `
      'PS event subscription failed -- check log for "psBehType NULL / eventProp NULL / evt NULL / exception"; PlayerSettlement may not be loaded or its type name changed'

Check 'Layer 4a: PS event FIRED (settlement build completed)' `
      "OnPSSettlementComplete: PS event FIRED" `
      'Never built a PS settlement in this session, or subscription failed silently'

Check 'Layer 4b: warmup drain executed' `
      "BeginNextTarget|progress \d+ / \d+|done for '" `
      'Warmup did not drain -- event fired but drain did not start'

Write-Host ""

$psFired = ([regex]::Matches($content, "OnPSSettlementComplete: PS event FIRED for '([^']+)'")).Groups | Where-Object { $_.Name -eq '1' } | ForEach-Object { $_.Value } | Sort-Object -Unique
if ($psFired) {
    Write-Host "Settlements that fired complete-event during this session:" -ForegroundColor Cyan
    $psFired | ForEach-Object { Write-Host "  - $_" }
    Write-Host ""
}

$doneMatches = [regex]::Matches($content, "done for '([^']+)' \((\d+) pairs\)")
if ($doneMatches.Count -gt 0) {
    Write-Host "Warmups completed:" -ForegroundColor Cyan
    foreach ($m in $doneMatches) { Write-Host ("  - {0}: {1} pairs" -f $m.Groups[1].Value, $m.Groups[2].Value) }
    Write-Host ""
}

$exceptions = $lines | Where-Object { $_ -match 'exception|Exception|EXCEPTION' }
if ($exceptions) {
    Write-Host "Exceptions in log:" -ForegroundColor Red
    $exceptions | Select-Object -First 10 | ForEach-Object { Write-Host "  $_" }
    Write-Host ""
}

Write-Host ("SUMMARY: {0} pass, {1} miss" -f $pass, $fail) -ForegroundColor $(if ($fail -eq 0) { 'Green' } else { 'Yellow' })
if ($fail -eq 0) { Write-Host "PSCacheWarmup v0.1.1 Village-side flow is fully working." -ForegroundColor Green }
elseif ($pass -ge 3 -and $fail -le 2) { Write-Host "Partial: mod initialized but no settlement was built or event flow broke mid-way." -ForegroundColor Yellow }
else { Write-Host "Broken: PSCacheWarmup is not functioning." -ForegroundColor Red }
