# Extract every damage_factor-bearing crafting piece from a set of XML files.
# Emits one row per piece with: source, id, tier, wclass, swing_type/factor, thrust_type/factor.
# Also prints group averages by (source, wclass, damage_kind).

$files = @(
    @{ src='OSA';    path='E:\SteamLibrary\steamapps\workshop\content\261550\3010984416\ModuleData\OSA_crafting_pieces.xml' },
    @{ src='RBM.sword';  path='E:\SteamLibrary\steamapps\workshop\content\261550\2859251492\ModuleData\RBMCombat_sword_blades.xml' },
    @{ src='RBM.axe';    path='E:\SteamLibrary\steamapps\workshop\content\261550\2859251492\ModuleData\RBMCombat_axe_pieces.xml' },
    @{ src='RBM.mace';   path='E:\SteamLibrary\steamapps\workshop\content\261550\2859251492\ModuleData\RBMCombat_mace_pieces.xml' },
    @{ src='RBM.couched'; path='E:\SteamLibrary\steamapps\workshop\content\261550\2859251492\ModuleData\RBMCombat_couched_lances.xml' },
    @{ src='RBM_WS';     path='E:\SteamLibrary\steamapps\workshop\content\261550\3635788184\ModuleData\RBMCombat_WS_crafting_pieces.xml' }
)

function Get-WClass($body) {
    if (-not $body) { return '(none)' }
    if ($body -match '^bo_axe')                    { return 'axe' }
    if ($body -match '^bo_mace')                   { return 'mace' }
    if ($body -match '^bo_sword_two')              { return 'sword_2h' }
    if ($body -match '^bo_sword_one')              { return 'sword_1h' }
    if ($body -match '^bo_sword')                  { return 'sword' }
    if ($body -match '^bo_pike')                   { return 'pike' }
    if ($body -match '^bo_spear')                  { return 'spear' }
    if ($body -match '^bo_glaive|^bo_polearm|^bo_billhook') { return 'polearm_2h' }
    if ($body -match '^bo_dagger|^bo_knife')       { return 'dagger' }
    return $body
}

$all = foreach ($f in $files) {
    if (-not (Test-Path -LiteralPath $f.path)) {
        Write-Warning "Missing: $($f.path)"
        continue
    }
    $xml = [xml](Get-Content -LiteralPath $f.path -Raw)
    foreach ($p in $xml.CraftingPieces.CraftingPiece) {
        if ($p.piece_type -ne 'Blade') { continue }
        $bd = $p.BladeData
        if (-not $bd) { continue }
        $sw = $bd.Swing
        $th = $bd.Thrust
        [pscustomobject]@{
            source        = $f.src
            id            = $p.id
            tier          = if ($p.tier) { [int]$p.tier } else { $null }
            body_name     = $bd.body_name
            wclass        = Get-WClass $bd.body_name
            swing_type    = $sw.damage_type
            swing_factor  = if ($sw -and $sw.damage_factor) { [double]$sw.damage_factor } else { $null }
            thrust_type   = $th.damage_type
            thrust_factor = if ($th -and $th.damage_factor) { [double]$th.damage_factor } else { $null }
        }
    }
}

"=== Row counts by source ==="
$all | Group-Object source | Format-Table Count, Name -AutoSize

"=== Row counts by source + wclass ==="
$all | Group-Object source, wclass | Sort-Object Name | Format-Table Count, Name -AutoSize

"=== Averages: swing_factor by source + wclass ==="
$all | Where-Object { $_.swing_factor -ne $null } |
    Group-Object source, wclass |
    ForEach-Object {
        $g = $_.Group
        [pscustomobject]@{
            source_wclass  = $_.Name
            n              = $_.Count
            swing_avg      = [math]::Round(( ($g | Measure-Object -Property swing_factor -Average).Average ), 3)
            swing_min      = ($g | Measure-Object -Property swing_factor -Minimum).Minimum
            swing_max      = ($g | Measure-Object -Property swing_factor -Maximum).Maximum
        }
    } | Sort-Object source_wclass | Format-Table -AutoSize

"=== Averages: thrust_factor by source + wclass ==="
$all | Where-Object { $_.thrust_factor -ne $null } |
    Group-Object source, wclass |
    ForEach-Object {
        $g = $_.Group
        [pscustomobject]@{
            source_wclass  = $_.Name
            n              = $_.Count
            thrust_avg     = [math]::Round(( ($g | Measure-Object -Property thrust_factor -Average).Average ), 3)
            thrust_min     = ($g | Measure-Object -Property thrust_factor -Minimum).Minimum
            thrust_max     = ($g | Measure-Object -Property thrust_factor -Maximum).Maximum
        }
    } | Sort-Object source_wclass | Format-Table -AutoSize
