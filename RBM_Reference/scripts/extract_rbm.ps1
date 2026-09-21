# Extract every RBM / RBM_WS item + crafting piece into flat CSVs.
#
# Same strategy as OSA_Reference/scripts/extract_osa.ps1:
#   Iterate all *.xml under the two workshop ModuleData folders. Filter by the
#   XML root element -- only files whose DocumentElement is <Items> or
#   <CraftingPieces> hold items/pieces we care about (skips ItemModifiers,
#   EquipmentRosters, NPCCharacters, strings, arrow visuals, siege data,
#   economy tables, custom battle presets, native param tweaks, etc.).
#
# ItemComponent is treated as a transparent wrapper so Armor / Weapon / Horse /
# HorseHarness / Shield subnodes surface directly (Armor.body_armor,
# Weapon.weapon_class, Horse.charge_damage, ...).
#
# Horse items carry <AdditionalMeshes> + <Materials> visual subtrees which we
# deliberately skip -- they are asset/appearance data (mesh names, colour
# multipliers) unrelated to combat stats, and their nested repeated <Mesh>
# / <Material> children would collide on flatten.
#
# Output: RBM_Reference/data/{rbm_items,rbm_crafted_items,rbm_crafted_items_pieces,rbm_crafting_pieces}.csv
#
# Rerun after RBM/RBM_WS workshop updates. Idempotent.

param(
    [string]$WorkshopRoot = 'E:\SteamLibrary\steamapps\workshop\content\261550',
    [string]$OutDir = (Join-Path $PSScriptRoot '..\data')
)

$ErrorActionPreference = 'Stop'

$mods = @(
    @{ Id = '2859251492'; Name = 'RBM' }        # main RBM (armour + horses + shields + ranged + crafting pieces + lances etc.)
    @{ Id = '3635788184'; Name = 'RBM_WS' }     # RBM Weapons+Shields extension
)

# Horse visual subtrees to ignore (asset-only, would collide on flatten).
$skipSubtrees = @('AdditionalMeshes','Materials','MeshMultipliers','face','skills','upgrade_targets')

$OutDir = Resolve-Path -LiteralPath $OutDir -ErrorAction SilentlyContinue
if (-not $OutDir) {
    $OutDir = Join-Path $PSScriptRoot '..\data'
    New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
    $OutDir = Resolve-Path -LiteralPath $OutDir
}

Write-Host "Output dir: $OutDir" -ForegroundColor Cyan

$itemRows = New-Object System.Collections.Generic.List[hashtable]
$craftedItemRows = New-Object System.Collections.Generic.List[hashtable]
$craftedItemPieceRows = New-Object System.Collections.Generic.List[hashtable]
$craftingPieceRows = New-Object System.Collections.Generic.List[hashtable]

function Flatten-Element {
    param(
        [System.Xml.XmlElement]$Node,
        [string]$Prefix,
        [hashtable]$Bag,
        [string[]]$SkipChildren
    )
    if (-not $Node) { return }
    foreach ($attr in $Node.Attributes) {
        $key = if ($Prefix) { "$Prefix.$($attr.Name)" } else { $attr.Name }
        $Bag[$key] = $attr.Value
    }
    foreach ($child in $Node.ChildNodes) {
        if ($child.NodeType -ne [System.Xml.XmlNodeType]::Element) { continue }
        if ($SkipChildren -contains $child.LocalName) { continue }
        if ($child.LocalName -eq 'ItemComponent') {
            Flatten-Element -Node $child -Prefix $Prefix -Bag $Bag -SkipChildren $SkipChildren
            continue
        }
        $childPrefix = if ($Prefix) { "$Prefix.$($child.LocalName)" } else { $child.LocalName }
        Flatten-Element -Node $child -Prefix $childPrefix -Bag $Bag -SkipChildren $SkipChildren
    }
}

$totalFiles = 0
$skippedFiles = @()
foreach ($mod in $mods) {
    $modDataPath = Join-Path $WorkshopRoot "$($mod.Id)\ModuleData"
    if (-not (Test-Path -LiteralPath $modDataPath)) {
        Write-Warning "Missing: $modDataPath (skipping $($mod.Name))"
        continue
    }

    $xmls = Get-ChildItem -LiteralPath $modDataPath -Filter '*.xml' -File
    foreach ($xmlFile in $xmls) {
        try {
            [xml]$doc = Get-Content -LiteralPath $xmlFile.FullName -Raw -Encoding UTF8
        } catch {
            Write-Warning "Parse fail: $($xmlFile.FullName) - $_"
            continue
        }

        $rootName = $doc.DocumentElement.LocalName
        if ($rootName -ne 'Items' -and $rootName -ne 'CraftingPieces') {
            $skippedFiles += "$($mod.Name)/$($xmlFile.Name) <$rootName>"
            continue
        }
        $totalFiles++

        foreach ($item in $doc.SelectNodes('//Item')) {
            $bag = @{ source_mod = $mod.Name; source_file = $xmlFile.Name }
            Flatten-Element -Node $item -Prefix '' -Bag $bag -SkipChildren $skipSubtrees
            [void]$itemRows.Add($bag)
        }

        foreach ($crafted in $doc.SelectNodes('//CraftedItem')) {
            $bag = @{ source_mod = $mod.Name; source_file = $xmlFile.Name }
            foreach ($attr in $crafted.Attributes) { $bag[$attr.Name] = $attr.Value }
            [void]$craftedItemRows.Add($bag)

            $craftedId = $crafted.GetAttribute('id')
            $slotIndex = 0
            foreach ($piece in $crafted.SelectNodes('.//Piece')) {
                $pieceBag = @{
                    source_mod = $mod.Name
                    source_file = $xmlFile.Name
                    crafted_item_id = $craftedId
                    slot_index = $slotIndex
                }
                foreach ($attr in $piece.Attributes) { $pieceBag[$attr.Name] = $attr.Value }
                [void]$craftedItemPieceRows.Add($pieceBag)
                $slotIndex++
            }
        }

        foreach ($cp in $doc.SelectNodes('//CraftingPiece')) {
            $bag = @{ source_mod = $mod.Name; source_file = $xmlFile.Name }
            Flatten-Element -Node $cp -Prefix '' -Bag $bag -SkipChildren $skipSubtrees
            [void]$craftingPieceRows.Add($bag)
        }
    }
}

Write-Host "Scanned $totalFiles item/piece XML files across $($mods.Count) mods."
if ($skippedFiles.Count -gt 0) {
    Write-Host "Skipped non-item XMLs ($($skippedFiles.Count)):" -ForegroundColor DarkGray
    $skippedFiles | ForEach-Object { Write-Host "  $_" -ForegroundColor DarkGray }
}
Write-Host "  Items:            $($itemRows.Count)"
Write-Host "  CraftedItems:     $($craftedItemRows.Count)"
Write-Host "  CraftedPieces:    $($craftedItemPieceRows.Count)"
Write-Host "  CraftingPieces:   $($craftingPieceRows.Count)"

function Write-BagsToCsv {
    param(
        [System.Collections.Generic.List[hashtable]]$Bags,
        [string]$Path,
        [string[]]$LeadingColumns
    )
    $keySet = New-Object System.Collections.Generic.HashSet[string]
    foreach ($b in $Bags) { foreach ($k in $b.Keys) { [void]$keySet.Add($k) } }

    $orderedCols = New-Object System.Collections.Generic.List[string]
    foreach ($c in $LeadingColumns) {
        if ($keySet.Contains($c)) {
            [void]$orderedCols.Add($c)
            [void]$keySet.Remove($c)
        }
    }
    foreach ($c in ($keySet | Sort-Object)) { [void]$orderedCols.Add($c) }

    $records = foreach ($b in $Bags) {
        $ordered = [ordered]@{}
        foreach ($c in $orderedCols) {
            if ($b.ContainsKey($c)) { $ordered[$c] = $b[$c] } else { $ordered[$c] = '' }
        }
        [pscustomobject]$ordered
    }

    $records | Export-Csv -LiteralPath $Path -NoTypeInformation -Encoding UTF8
    Write-Host "Wrote $Path  ($($Bags.Count) rows x $($orderedCols.Count) cols)" -ForegroundColor Green
}

$itemsCsv           = Join-Path $OutDir 'rbm_items.csv'
$craftedItemsCsv    = Join-Path $OutDir 'rbm_crafted_items.csv'
$craftedPiecesCsv   = Join-Path $OutDir 'rbm_crafted_items_pieces.csv'
$craftingPiecesCsv  = Join-Path $OutDir 'rbm_crafting_pieces.csv'

Write-BagsToCsv -Bags $itemRows           -Path $itemsCsv          -LeadingColumns @('source_mod','source_file','id','name','Type','culture','subtype','mesh','weight','value','difficulty','appearance','item_category','Armor.material_type','Armor.modifier_group','Armor.head_armor','Armor.body_armor','Armor.arm_armor','Armor.leg_armor','Armor.covers_body','Armor.has_gender_variations','Armor.hair_cover_type','Armor.beard_cover_type','Armor.mane_cover_type','Armor.maneuver_bonus','Armor.speed_bonus','Armor.charge_bonus','Armor.family_type','Armor.reins_mesh','Weapon.weapon_class','Weapon.ammo_class','Weapon.weapon_length','Weapon.thrust_damage','Weapon.thrust_damage_type','Weapon.thrust_speed','Weapon.swing_damage','Weapon.swing_damage_type','Weapon.swing_speed','Weapon.speed_rating','Weapon.accuracy','Weapon.missile_speed','Weapon.stack_amount','Weapon.handling','Weapon.modifier_group','Weapon.item_usage','Horse.monster','Horse.maneuver','Horse.speed','Horse.charge_damage','Horse.body_length','Horse.extra_health','Horse.is_mountable','Horse.is_pack_animal','Horse.modifier_group')
Write-BagsToCsv -Bags $craftedItemRows    -Path $craftedItemsCsv   -LeadingColumns @('source_mod','source_file','id','name','crafting_template','culture','modifier_group')
Write-BagsToCsv -Bags $craftedItemPieceRows -Path $craftedPiecesCsv -LeadingColumns @('source_mod','source_file','crafted_item_id','slot_index','id','Type','scale_factor')
Write-BagsToCsv -Bags $craftingPieceRows  -Path $craftingPiecesCsv -LeadingColumns @('source_mod','source_file','id','name','tier','piece_type','culture','mesh','weight','length','BladeData.blade_length','BladeData.blade_width','BladeData.stack_amount','BladeData.Swing.damage_type','BladeData.Swing.damage_factor','BladeData.Thrust.damage_type','BladeData.Thrust.damage_factor','StatContributions.armor_bonus','BuildData.piece_offset','BuildData.next_piece_offset','CraftingCost','is_default','is_hidden')

Write-Host "`nDone." -ForegroundColor Yellow
