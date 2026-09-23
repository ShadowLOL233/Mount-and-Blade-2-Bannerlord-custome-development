param(
    [Parameter(Mandatory=$false)][switch]$Enable,
    [Parameter(Mandatory=$false)][switch]$Disable,
    [Parameter(Mandatory=$false)][switch]$Status,
    [string]$GameRoot     = 'E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord',
    [string]$LauncherData = 'E:\Bannerlord-UserData\Mount and Blade II Bannerlord\Configs\LauncherData.xml',
    [string]$SaveDir      = 'E:\Bannerlord-UserData\Mount and Blade II Bannerlord\Game Saves'
)

$ErrorActionPreference = 'Stop'

$modes = @($Enable.IsPresent, $Disable.IsPresent, $Status.IsPresent) | Where-Object { $_ }
if ($modes.Count -ne 1) {
    Write-Host "Usage: .\mnr-toggle.ps1 [-Enable | -Disable | -Status]" -ForegroundColor Yellow
    Write-Host "  -Enable   Flip MNR SubModule DefaultModule=true + LauncherData IsSelected=true (Phase 3 armed)"
    Write-Host "  -Disable  Reverse (safe with old vanilla-map saves)"
    Write-Host "  -Status   Show current MNR + PSCacheWarmup config"
    return
}

$mnrXml       = Join-Path $GameRoot 'Modules\MoreNationsRemastered\SubModule.xml'
$pscwXml      = Join-Path $GameRoot 'Modules\PSCacheWarmup\SubModule.xml'
if (-not (Test-Path $mnrXml))       { throw "MNR SubModule.xml not found at $mnrXml -- is MNR installed?" }
if (-not (Test-Path $pscwXml))      { throw "PSCacheWarmup SubModule.xml not found at $pscwXml -- is PSCacheWarmup deployed?" }
if (-not (Test-Path $LauncherData)) { throw "LauncherData.xml not found at $LauncherData" }

function Get-MnrDefaultModule {
    param($path)
    ([xml](Get-Content $path -Raw)).Module.DefaultModule.value
}

function Set-MnrDefaultModule {
    param($path, [bool]$value)
    $val = if ($value) { 'true' } else { 'false' }
    $text = Get-Content $path -Raw
    $new = $text -replace '<DefaultModule value="(true|false)"/>', "<DefaultModule value=`"$val`"/>"
    Set-Content -Path $path -Value $new -Encoding utf8 -NoNewline
}

function Get-LauncherIsSelected {
    param($path, [string]$modId)
    $xml = [xml](Get-Content $path -Raw)
    $entry = $xml.UserData.SingleplayerData.ModDatas.UserModData | Where-Object { $_.Id -eq $modId } | Select-Object -First 1
    if ($entry) { return $entry.IsSelected } else { return $null }
}

function Set-LauncherIsSelected {
    param($path, [string]$modId, [bool]$value)
    $val = if ($value) { 'true' } else { 'false' }
    $text = Get-Content $path -Raw
    $pattern = "(?ms)(<UserModData>\s*<Id>$modId</Id>\s*<LastKnownVersion>[^<]+</LastKnownVersion>\s*<IsSelected>)(true|false)(</IsSelected>)"
    if ($text -notmatch $pattern) { throw "Failed to locate $modId UserModData entry in LauncherData" }
    $new = [regex]::Replace($text, $pattern, "`${1}$val`${3}")
    Set-Content -Path $path -Value $new -Encoding utf8 -NoNewline
}

function Show-Status {
    Write-Host ""
    Write-Host "=== MNR / PS / PSCacheWarmup config status ===" -ForegroundColor Cyan
    $mnrDefault = Get-MnrDefaultModule $mnrXml
    $mnrSelected = Get-LauncherIsSelected $LauncherData 'MoreNationsRemastered'
    $psSelected  = Get-LauncherIsSelected $LauncherData 'PlayerSettlement'
    $pscwSelected= Get-LauncherIsSelected $LauncherData 'PSCacheWarmup'
    Write-Host ("  MoreNationsRemastered SubModule DefaultModule = {0}" -f $mnrDefault)
    Write-Host ("  MoreNationsRemastered LauncherData IsSelected = {0}" -f $mnrSelected)
    Write-Host ("  PlayerSettlement       LauncherData IsSelected = {0}" -f $psSelected)
    Write-Host ("  PSCacheWarmup          LauncherData IsSelected = {0}" -f $pscwSelected)
    Write-Host ""
    $armed = ($mnrDefault -eq 'true') -and ($mnrSelected -eq 'true') -and ($pscwSelected -eq 'true') -and ($psSelected -eq 'true')
    if ($armed) { Write-Host "STATE: ARMED for Phase 3 (new campaign) -- old vanilla-map saves WILL NOT WORK" -ForegroundColor Yellow }
    else        { Write-Host "STATE: SAFE for existing vanilla-map saves" -ForegroundColor Green }
}

if ($Status) { Show-Status; return }

if ($Enable) {
    Write-Host "=== Enabling MNR + PSCacheWarmup for Phase 3 ===" -ForegroundColor Cyan
    if (Test-Path $SaveDir) {
        $count = (Get-ChildItem $SaveDir -Filter '*.sav' -File | Measure-Object).Count
        Write-Host "  ! $count existing save(s) in $SaveDir -- MNR is a map replacement, they may not load. Back them up first if precious." -ForegroundColor Yellow
    }
    Write-Host "  1/3 MNR SubModule.xml DefaultModule -> true..."
    Set-MnrDefaultModule $mnrXml $true
    Write-Host "  2/3 LauncherData MoreNationsRemastered IsSelected -> true..."
    Set-LauncherIsSelected $LauncherData 'MoreNationsRemastered' $true
    Write-Host "  3/3 LauncherData PSCacheWarmup IsSelected verify (leave as-is if already true)..."
    $cur = Get-LauncherIsSelected $LauncherData 'PSCacheWarmup'
    if ($cur -ne 'true') { Set-LauncherIsSelected $LauncherData 'PSCacheWarmup' $true; Write-Host "      set to true" -ForegroundColor Green } else { Write-Host "      already true" -ForegroundColor Green }
    Show-Status
    Write-Host ""
    Write-Host "NEXT STEPS:" -ForegroundColor Cyan
    Write-Host "  1. Open launcher. Verify 'More Nations Remastered' is checked + loads after Sandbox (auto-order)."
    Write-Host "  2. Start a NEW campaign. Wait 1:30-3:00 min for world generation."
    Write-Host "  3. Play for a few in-game days, watch for lag."
    Write-Host "  4. Build a Village via PlayerSettlement. Message bar should show '[PS Cache Warmup] queued...'."
    Write-Host "  5. Run .\mnr-phase3-verify.ps1 after the session to sanity-check the log."
    return
}

if ($Disable) {
    Write-Host "=== Disabling MNR -- reverting to vanilla map ===" -ForegroundColor Cyan
    Write-Host "  1/2 MNR SubModule.xml DefaultModule -> false..."
    Set-MnrDefaultModule $mnrXml $false
    Write-Host "  2/2 LauncherData MoreNationsRemastered IsSelected -> false..."
    Set-LauncherIsSelected $LauncherData 'MoreNationsRemastered' $false
    Show-Status
    Write-Host ""
    Write-Host "MNR is now inert. Vanilla-map saves are safe to load. PSCacheWarmup left enabled (harmless without MNR/PS use)." -ForegroundColor Green
    return
}
