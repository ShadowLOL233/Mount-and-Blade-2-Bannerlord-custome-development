# Open Source Armoury RBM Balance Patch - override generator.
# Reads OSA + OSW workshop XMLs, computes per-(Type, mat, tier, slot) buffs
# vs RBM baseline, emits two override XMLs into ../ModuleData/.
#
# Design rules (see ModdingJournal.md "OSA 护甲 vs RBM 跨槽覆盖分析" + "OSA 武器
# damage_factor 完整对照表"):
#   armor:
#     - tier via DefaultItemValueModel.CalculateArmorTier formula
#     - buff factor = RBM_avg / OSA_avg at (Type, mat, tier, slot)
#     - fallback ladder: (Type, mat, tier) -> (Type, mat) -> skip
#     - max(1.0, factor): never downscale OSA (user 2026-09-20 decision)
#     - skip if factor <= 1.05
#     - LegArmor items matching /shoes|boots|moccasins/ in id: skip entirely
#     - Cape.arm slot: target = 12 flat (RBM has no baseline)
#     - HeadArmor mat in {Chainmail, Plate} tier >= 4: add body=head*0.43, arm=head*0.37
#   weapon pieces (Blade only):
#     - swing/thrust damage_factor -> pooled RBM avg per wclass (from body_name)
#     - factor = RBM_avg / OSA_avg per wclass
#     - Guard/Handle/Pommel have no damage_factor - skip

param(
    [string]$OsaArmorRoot   = 'E:\SteamLibrary\steamapps\workshop\content\261550\3011479883\ModuleData',
    [string]$OswWeaponRoot  = 'E:\SteamLibrary\steamapps\workshop\content\261550\3010984416\ModuleData',
    [string]$RbmArmorRoot   = 'E:\SteamLibrary\steamapps\workshop\content\261550\2859251492\ModuleData',
    [string]$RbmWsRoot      = 'E:\SteamLibrary\steamapps\workshop\content\261550\3635788184\ModuleData'
)

$ErrorActionPreference = 'Stop'
$here = $PSScriptRoot
$outDir = Resolve-Path (Join-Path $here '..\ModuleData')

Write-Host "== Open Source Armoury RBM Balance Patch generator ==" -ForegroundColor Cyan
Write-Host "output dir: $outDir"

# ---------- helpers ----------
function Get-ArmorTier {
    param([string]$Type, [int]$head, [int]$body, [int]$arm, [int]$leg)
    $raw = 1.2 * $head + 1.0 * $body + 1.0 * $leg + 1.0 * $arm
    switch ($Type) {
        'LegArmor'  { $raw *= 1.6 }
        'HandArmor' { $raw *= 1.7 }
        'HeadArmor' { $raw *= 1.2 }
        'Cape'      { $raw *= 1.8 }
    }
    $tierf = $raw * 0.1 - 0.4
    $rounded = [int]([Math]::Round($tierf))
    $clamped = [Math]::Max(0, [Math]::Min(6, $rounded))
    $tier = $clamped - 1
    if ($tier -lt 0) { return 1 }
    return $tier + 1
}

function Get-WClass {
    param([string]$body)
    if (-not $body) { return $null }
    if ($body -match '^bo_axe')                    { return 'axe' }
    if ($body -match '^bo_mace')                   { return 'mace' }
    if ($body -match '^bo_sword_two')              { return 'sword_2h' }
    if ($body -match '^bo_sword_one|^bo_sword')    { return 'sword_1h' }
    if ($body -match '^bo_pike')                   { return 'pike' }
    if ($body -match '^bo_spear')                  { return 'spear' }
    if ($body -match '^bo_glaive|^bo_polearm|^bo_billhook') { return 'polearm' }
    if ($body -match '^bo_dagger|^bo_knife')       { return 'dagger' }
    return $null
}

function Read-Xml {
    param([string]$path)
    if (-not (Test-Path -LiteralPath $path)) { return $null }
    try { return [xml](Get-Content -LiteralPath $path -Raw) } catch { Write-Warning "parse fail: $path"; return $null }
}

# ---------- Load RBM armor baseline ----------
Write-Host "`n[1/6] Loading RBM armor baseline..."
$rbmArmorFiles = @('RBMCombat_head_armors.xml','RBMCombat_body_armors.xml','RBMCombat_arm_armors.xml',
                   'RBMCombat_leg_armors.xml','RBMCombat_shoulder_armors.xml') |
                 ForEach-Object { Join-Path $RbmArmorRoot $_ }

$rbmArmorRows = @()
foreach ($p in $rbmArmorFiles) {
    $xml = Read-Xml $p
    if (-not $xml) { continue }
    foreach ($it in $xml.Items.Item) {
        $arm = $it.ItemComponent.Armor
        if (-not $arm) { continue }
        $Type = $it.Type; if (-not $Type) { continue }
        $h = [int]($arm.head_armor); $b = [int]($arm.body_armor); $a = [int]($arm.arm_armor); $l = [int]($arm.leg_armor)
        $w = if ($it.weight) { [double]$it.weight } else { 0.0 }
        $rbmArmorRows += [pscustomobject]@{
            Type = $Type
            mat  = if ($arm.material_type) { $arm.material_type } else { '(none)' }
            tier = Get-ArmorTier -Type $Type -head $h -body $b -arm $a -leg $l
            head_armor=$h; body_armor=$b; arm_armor=$a; leg_armor=$l
            weight = $w
        }
    }
}
Write-Host "  loaded $($rbmArmorRows.Count) RBM armor items"

# ---------- Build target lookup: (Type, mat, tier, slot) -> avg ----------
Write-Host "`n[2/6] Building RBM target-average lookup..."
$targetTable = @{}
foreach ($grp in ($rbmArmorRows | Group-Object Type, mat, tier)) {
    foreach ($slot in @('head_armor','body_armor','arm_armor','leg_armor')) {
        $nz = @($grp.Group | Where-Object { $_.$slot -gt 0 })
        if ($nz.Count -ge 3) {
            $key = "$($grp.Name)|$slot"
            $targetTable[$key] = [math]::Round( ($nz | Measure-Object -Property $slot -Average).Average, 1 )
        }
    }
    # weight target (only if cell has >=3 items with any positive weight)
    $wnz = @($grp.Group | Where-Object { $_.weight -gt 0 })
    if ($wnz.Count -ge 3) {
        $key = "$($grp.Name)|weight"
        $targetTable[$key] = [math]::Round( ($wnz | Measure-Object -Property weight -Average).Average, 3 )
    }
}
# material-level fallback
foreach ($grp in ($rbmArmorRows | Group-Object Type, mat)) {
    foreach ($slot in @('head_armor','body_armor','arm_armor','leg_armor')) {
        $nz = @($grp.Group | Where-Object { $_.$slot -gt 0 })
        if ($nz.Count -ge 3) {
            $key = "$($grp.Name)|*|$slot"  # * = any tier
            $targetTable[$key] = [math]::Round( ($nz | Measure-Object -Property $slot -Average).Average, 1 )
        }
    }
    $wnz = @($grp.Group | Where-Object { $_.weight -gt 0 })
    if ($wnz.Count -ge 3) {
        $key = "$($grp.Name)|*|weight"
        $targetTable[$key] = [math]::Round( ($wnz | Measure-Object -Property weight -Average).Average, 3 )
    }
}
Write-Host "  built $($targetTable.Count) target-average entries"

# ---------- Build OSA per-(Type,mat,tier,slot) current-average lookup (for factor denominator) ----------
Write-Host "`n[3/6] Loading OSA armor items..."
$osaArmorFiles = Get-ChildItem $OsaArmorRoot -Filter 'OSA_*.xml' -File |
                 Where-Object { $_.FullName -notmatch '\\Languages\\' } |
                 Select-Object -ExpandProperty FullName

$osaArmorItems = @()  # list of hashtables with all needed context
foreach ($p in $osaArmorFiles) {
    $xml = Read-Xml $p
    if (-not $xml) { continue }
    if (-not $xml.Items) { continue }
    foreach ($it in $xml.Items.Item) {
        $arm = $it.ItemComponent.Armor
        if (-not $arm) { continue }
        $Type = $it.Type; if (-not $Type) { continue }
        $h = [int]($arm.head_armor); $b = [int]($arm.body_armor); $a = [int]($arm.arm_armor); $l = [int]($arm.leg_armor)
        $w = if ($it.weight) { [double]$it.weight } else { 0.0 }
        $osaArmorItems += [pscustomobject]@{
            src_file = Split-Path $p -Leaf
            src_node = $it
            id       = $it.id
            Type     = $Type
            mat      = if ($arm.material_type) { $arm.material_type } else { '(none)' }
            tier     = Get-ArmorTier -Type $Type -head $h -body $b -arm $a -leg $l
            head_armor=$h; body_armor=$b; arm_armor=$a; leg_armor=$l
            weight = $w
        }
    }
}
Write-Host "  loaded $($osaArmorItems.Count) OSA armor items"

$osaCurrent = @{}
foreach ($grp in ($osaArmorItems | Group-Object Type, mat, tier)) {
    foreach ($slot in @('head_armor','body_armor','arm_armor','leg_armor')) {
        $nz = @($grp.Group | Where-Object { $_.$slot -gt 0 })
        if ($nz.Count -ge 1) {
            $key = "$($grp.Name)|$slot"
            $osaCurrent[$key] = [math]::Round( ($nz | Measure-Object -Property $slot -Average).Average, 2 )
        }
    }
    $wnz = @($grp.Group | Where-Object { $_.weight -gt 0 })
    if ($wnz.Count -ge 1) {
        $key = "$($grp.Name)|weight"
        $osaCurrent[$key] = [math]::Round( ($wnz | Measure-Object -Property weight -Average).Average, 3 )
    }
}
foreach ($grp in ($osaArmorItems | Group-Object Type, mat)) {
    foreach ($slot in @('head_armor','body_armor','arm_armor','leg_armor')) {
        $nz = @($grp.Group | Where-Object { $_.$slot -gt 0 })
        if ($nz.Count -ge 1) {
            $key = "$($grp.Name)|*|$slot"
            $osaCurrent[$key] = [math]::Round( ($nz | Measure-Object -Property $slot -Average).Average, 2 )
        }
    }
    $wnz = @($grp.Group | Where-Object { $_.weight -gt 0 })
    if ($wnz.Count -ge 1) {
        $key = "$($grp.Name)|*|weight"
        $osaCurrent[$key] = [math]::Round( ($wnz | Measure-Object -Property weight -Average).Average, 3 )
    }
}

# ---------- Emit armor override XML ----------
Write-Host "`n[4/6] Emitting armor override XML..."
$armorOut = New-Object System.Text.StringBuilder
[void]$armorOut.AppendLine('<?xml version="1.0" encoding="utf-8"?>')
[void]$armorOut.AppendLine('<!-- Open Source Armoury RBM Balance Patch - auto-generated. Do not hand-edit. -->')
[void]$armorOut.AppendLine('<!-- Regenerate via src/generate.ps1. -->')
[void]$armorOut.AppendLine('<Items>')

$statsChanged = 0; $statsSkippedShoes = 0; $statsHelmetExtended = 0; $statsWeightLightened = 0

foreach ($item in $osaArmorItems) {
    # skip shoes/boots/moccasins in LegArmor
    if ($item.Type -eq 'LegArmor' -and $item.id -match '(?i)shoes|boots|moccasins') {
        $statsSkippedShoes++
        continue
    }

    # Compute new slot values
    $newVals = @{
        head_armor = $item.head_armor
        body_armor = $item.body_armor
        arm_armor  = $item.arm_armor
        leg_armor  = $item.leg_armor
    }
    $touched = $false

    foreach ($slot in @('head_armor','body_armor','arm_armor','leg_armor')) {
        $current = $item.$slot
        if ($current -le 0) { continue }

        # Cape.arm special case
        if ($item.Type -eq 'Cape' -and $slot -eq 'arm_armor') {
            $target = 12.0
        } else {
            $tierKey = "$($item.Type), $($item.mat), $($item.tier)|$slot"
            $matKey  = "$($item.Type), $($item.mat)|*|$slot"
            $target = $targetTable[$tierKey]
            if (-not $target) { $target = $targetTable[$matKey] }
        }
        if (-not $target) { continue }

        # Per-cell factor uses OSA cell avg as denominator (so all items in the cell scale uniformly)
        if ($item.Type -eq 'Cape' -and $slot -eq 'arm_armor') {
            $denomKey = "Cape, $($item.mat), $($item.tier)|$slot"
            $osaAvg = $osaCurrent[$denomKey]
            if (-not $osaAvg) { $osaAvg = $osaCurrent["Cape, $($item.mat)|*|$slot"] }
        } else {
            $denomKey = "$($item.Type), $($item.mat), $($item.tier)|$slot"
            $osaAvg = $osaCurrent[$denomKey]
            if (-not $osaAvg) { $osaAvg = $osaCurrent["$($item.Type), $($item.mat)|*|$slot"] }
        }
        if (-not $osaAvg -or $osaAvg -le 0) { continue }

        $factor = $target / $osaAvg
        if ($factor -le 1.05) { continue }   # no downscale, no near-equal churn

        $newVals[$slot] = [int]([Math]::Round($current * $factor))
        $touched = $true
    }

    # Helmet extension: Chainmail/Plate HeadArmor tier >= 4 gets body + arm coverage
    if ($item.Type -eq 'HeadArmor' -and $item.tier -ge 4 -and ($item.mat -eq 'Chainmail' -or $item.mat -eq 'Plate')) {
        $srcHead = $newVals.head_armor
        if ($srcHead -gt 0) {
            $addBody = [int]([Math]::Round($srcHead * 0.43))
            $addArm  = [int]([Math]::Round($srcHead * 0.37))
            if ($newVals.body_armor -lt $addBody) { $newVals.body_armor = $addBody; $touched = $true; $statsHelmetExtended++ }
            if ($newVals.arm_armor  -lt $addArm)  { $newVals.arm_armor  = $addArm  }
        }
    }

    # Weight normalization: only lighten (min(1.0, factor)); never make items heavier
    $newWeight = $item.weight
    $weightTouched = $false
    if ($item.weight -gt 0) {
        $wTierKey = "$($item.Type), $($item.mat), $($item.tier)|weight"
        $wMatKey  = "$($item.Type), $($item.mat)|*|weight"
        $wTarget  = $targetTable[$wTierKey]
        if (-not $wTarget) { $wTarget = $targetTable[$wMatKey] }
        if ($wTarget) {
            $wDenom = $osaCurrent[$wTierKey]
            if (-not $wDenom) { $wDenom = $osaCurrent[$wMatKey] }
            if ($wDenom -and $wDenom -gt 0) {
                $wFactor = $wTarget / $wDenom
                # only lighten (factor < 0.95); skip near-equal or heavier
                if ($wFactor -lt 0.95) {
                    $newWeight = [math]::Round($item.weight * $wFactor, 2)
                    if ($newWeight -lt 0.1) { $newWeight = 0.1 }  # floor to avoid zero-weight
                    $weightTouched = $true
                }
            }
        }
    }
    if ($weightTouched) { $touched = $true; $statsWeightLightened++ }

    if (-not $touched) { continue }

    # Serialize the modified item: clone src_node, patch Armor attributes + weight, emit
    $clone = $item.src_node.CloneNode($true)
    $armor = $clone.ItemComponent.Armor
    $armor.SetAttribute('head_armor', [string]$newVals.head_armor)
    $armor.SetAttribute('body_armor', [string]$newVals.body_armor)
    $armor.SetAttribute('arm_armor',  [string]$newVals.arm_armor)
    $armor.SetAttribute('leg_armor',  [string]$newVals.leg_armor)
    if ($weightTouched) {
        $clone.SetAttribute('weight', [string]$newWeight)
    }

    $sw = New-Object System.IO.StringWriter
    $writer = New-Object System.Xml.XmlTextWriter($sw)
    $writer.Formatting = [System.Xml.Formatting]::Indented
    $clone.WriteTo($writer)
    $writer.Flush()
    [void]$armorOut.AppendLine('  ' + $sw.ToString())
    $statsChanged++
}

[void]$armorOut.AppendLine('</Items>')
$armorPath = Join-Path $outDir 'OSABalance_armor_override.xml'
$armorOut.ToString() | Set-Content -LiteralPath $armorPath -Encoding UTF8
Write-Host "  armor: $statsChanged items overridden ($statsHelmetExtended got helmet extension, $statsWeightLightened had weight lightened, $statsSkippedShoes shoes skipped)"

# ---------- Weapon piece override ----------
Write-Host "`n[5/6] Emitting weapon piece override XML..."
$osaPiecesPath = Join-Path $OswWeaponRoot 'OSA_crafting_pieces.xml'
$osaXml = Read-Xml $osaPiecesPath

# RBM baseline averages (pre-computed from prior analysis, see journal)
# swing / thrust factor targets per wclass
$rbmTarget = @{
    'axe|swing'      = 0.98
    'mace|swing'     = 0.74
    'mace|thrust'    = 0.88
    'sword_1h|swing' = 0.99
    'sword_1h|thrust'= 0.85
    'spear|swing'    = 0.75
    'spear|thrust'   = 0.93
    'polearm|swing'  = 0.75
}
# Pre-compute OSA piece averages for factor denominator
$osaPieceAgg = @{}
foreach ($p in $osaXml.CraftingPieces.CraftingPiece) {
    if ($p.piece_type -ne 'Blade') { continue }
    $bd = $p.BladeData; if (-not $bd) { continue }
    $wclass = Get-WClass $bd.body_name
    if (-not $wclass) { continue }
    if ($bd.Swing -and $bd.Swing.damage_factor) {
        $k = "$wclass|swing"
        if (-not $osaPieceAgg[$k]) { $osaPieceAgg[$k] = @() }
        $osaPieceAgg[$k] += [double]$bd.Swing.damage_factor
    }
    if ($bd.Thrust -and $bd.Thrust.damage_factor) {
        $k = "$wclass|thrust"
        if (-not $osaPieceAgg[$k]) { $osaPieceAgg[$k] = @() }
        $osaPieceAgg[$k] += [double]$bd.Thrust.damage_factor
    }
}
$osaPieceAvg = @{}
foreach ($k in $osaPieceAgg.Keys) {
    $osaPieceAvg[$k] = ($osaPieceAgg[$k] | Measure-Object -Average).Average
}

$pieceOut = New-Object System.Text.StringBuilder
[void]$pieceOut.AppendLine('<?xml version="1.0" encoding="utf-8"?>')
[void]$pieceOut.AppendLine('<!-- Open Source Armoury RBM Balance Patch - weapon piece damage overrides. -->')
[void]$pieceOut.AppendLine('<!-- Regenerate via src/generate.ps1. -->')
[void]$pieceOut.AppendLine('<CraftingPieces>')

$pieceChanged = 0
foreach ($p in $osaXml.CraftingPieces.CraftingPiece) {
    if ($p.piece_type -ne 'Blade') { continue }
    $bd = $p.BladeData; if (-not $bd) { continue }
    $wclass = Get-WClass $bd.body_name
    if (-not $wclass) { continue }

    $touched = $false
    $clone = $p.CloneNode($true)
    $cloneBd = $clone.BladeData

    foreach ($att in @(@{name='Swing'; kind='swing'}, @{name='Thrust'; kind='thrust'})) {
        $node = $cloneBd.($att.name)
        if (-not $node -or -not $node.damage_factor) { continue }
        $current = [double]$node.damage_factor
        $tgtKey = "$wclass|$($att.kind)"
        $target = $rbmTarget[$tgtKey]
        if (-not $target) { continue }
        $osaAvg = $osaPieceAvg[$tgtKey]
        if (-not $osaAvg -or $osaAvg -le 0) { continue }
        $factor = $target / $osaAvg
        # here factor can be <1 (downscale intentional for weapons - RBM cut weapon damage
        # by ~30%, so we need to actually downscale OSA to match).
        # Only skip if factor is within 5% of 1
        if ($factor -ge 0.95 -and $factor -le 1.05) { continue }
        $newVal = [Math]::Round($current * $factor, 2)
        $node.SetAttribute('damage_factor', [string]$newVal)
        $touched = $true
    }
    if (-not $touched) { continue }

    $sw = New-Object System.IO.StringWriter
    $writer = New-Object System.Xml.XmlTextWriter($sw)
    $writer.Formatting = [System.Xml.Formatting]::Indented
    $clone.WriteTo($writer)
    $writer.Flush()
    [void]$pieceOut.AppendLine('  ' + $sw.ToString())
    $pieceChanged++
}
[void]$pieceOut.AppendLine('</CraftingPieces>')
$piecePath = Join-Path $outDir 'OSABalance_pieces_override.xml'
$pieceOut.ToString() | Set-Content -LiteralPath $piecePath -Encoding UTF8
Write-Host "  pieces: $pieceChanged pieces overridden"

# ---------- Summary ----------
Write-Host "`n[6/6] Done." -ForegroundColor Green
Write-Host "Files written:"
Write-Host "  $armorPath"
Write-Host "  $piecePath"
Write-Host "`nApplied rules:"
Write-Host "  - armor buffs: max(1.0, factor); skip near-equal (factor <= 1.05); skip shoes/boots/moccasins in LegArmor"
Write-Host "  - Cape.arm slot: target = 12 flat"
Write-Host "  - helmet extension: mat in {Chainmail, Plate} & tier >= 4 -> add body=head*0.43, arm=head*0.37"
Write-Host "  - weapon Blade piece: factor can be <1 (RBM intentionally downscales weapon damage)"
