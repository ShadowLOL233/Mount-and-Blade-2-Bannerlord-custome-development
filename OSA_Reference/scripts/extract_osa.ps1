# Extract every OSA / OSW / Saddlery item + crafting piece into flat CSVs.
#
# Strategy: iterate all *.xml under the three workshop ModuleData folders, parse
# with [xml], visit each <Item> / <CraftedItem> / <CraftingPiece>, and flatten
# the whole element tree into a single row. Column names are auto-collected as
# the union of every attribute seen — subnode attributes are prefixed with the
# subnode path (Armor.body_armor, Weapon.weapon_class, BladeData.Swing.damage_factor, ...).
#
# Boolean flag containers (WeaponFlags, Flags on Item, Flags on CraftingPiece)
# each become one column per flag with value True/False.
#
# Output: OSA_Reference/data/{osa_items,osa_crafted_items,osa_crafted_items_pieces,osa_crafting_pieces}.csv
#
# Rerun after workshop mod updates. Idempotent.

param(
    [string]$WorkshopRoot = 'E:\SteamLibrary\steamapps\workshop\content\261550',
    [string]$OutDir = (Join-Path $PSScriptRoot '..\data')
)

$ErrorActionPreference = 'Stop'

$mods = @(
    @{ Id = '3011479883'; Name = 'OSA' }        # armour
    @{ Id = '3010984416'; Name = 'OSW' }        # weapons + crafting pieces
    @{ Id = '3010990914'; Name = 'Saddlery' }   # horse harness
)

$OutDir = Resolve-Path -LiteralPath $OutDir -ErrorAction SilentlyContinue
if (-not $OutDir) {
    $OutDir = Join-Path $PSScriptRoot '..\data'
    New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
    $OutDir = Resolve-Path -LiteralPath $OutDir
}

Write-Host "Output dir: $OutDir" -ForegroundColor Cyan

# --- Field-collector state ------------------------------------------------
$itemRows = New-Object System.Collections.Generic.List[hashtable]
$craftedItemRows = New-Object System.Collections.Generic.List[hashtable]
$craftedItemPieceRows = New-Object System.Collections.Generic.List[hashtable]
$craftingPieceRows = New-Object System.Collections.Generic.List[hashtable]

# Recursively flatten an XmlElement into a hashtable keyed by prefixed attribute paths.
# ItemComponent is treated as a transparent wrapper (no prefix added) so an Item's
# <Armor>/<Weapon>/<Horse>/<HorseHarness>/<Shield>/<Banner> child of ItemComponent shows
# up as e.g. Armor.body_armor rather than ItemComponent.Armor.body_armor.
function Flatten-Element {
    param(
        [System.Xml.XmlElement]$Node,
        [string]$Prefix,
        [hashtable]$Bag
    )
    if (-not $Node) { return }
    foreach ($attr in $Node.Attributes) {
        $key = if ($Prefix) { "$Prefix.$($attr.Name)" } else { $attr.Name }
        $Bag[$key] = $attr.Value
    }
    foreach ($child in $Node.ChildNodes) {
        if ($child.NodeType -ne [System.Xml.XmlNodeType]::Element) { continue }
        if ($child.LocalName -eq 'ItemComponent') {
            # transparent wrapper: descend without adding to prefix
            Flatten-Element -Node $child -Prefix $Prefix -Bag $Bag
            continue
        }
        $childPrefix = if ($Prefix) { "$Prefix.$($child.LocalName)" } else { $child.LocalName }
        Flatten-Element -Node $child -Prefix $childPrefix -Bag $Bag
    }
}

# --- Main scan -----------------------------------------------------------
$totalFiles = 0
foreach ($mod in $mods) {
    $modDataPath = Join-Path $WorkshopRoot "$($mod.Id)\ModuleData"
    if (-not (Test-Path -LiteralPath $modDataPath)) {
        Write-Warning "Missing: $modDataPath (skipping $($mod.Name))"
        continue
    }

    $xmls = Get-ChildItem -LiteralPath $modDataPath -Filter '*.xml' -File
    foreach ($xmlFile in $xmls) {
        # Skip localization / cloth material files that don't hold items.
        if ($xmlFile.Name -match '^(cloth_bodies|cloth_materials)\.xml$') { continue }
        $totalFiles++

        try {
            [xml]$doc = Get-Content -LiteralPath $xmlFile.FullName -Raw -Encoding UTF8
        } catch {
            Write-Warning "Parse fail: $($xmlFile.FullName) - $_"
            continue
        }

        # <Item> (regular armor / weapon / horse / horse harness)
        foreach ($item in $doc.SelectNodes('//Item')) {
            $bag = @{ source_mod = $mod.Name; source_file = $xmlFile.Name }
            Flatten-Element -Node $item -Prefix '' -Bag $bag
            [void]$itemRows.Add($bag)
        }

        # <CraftedItem> — pre-built crafted weapon; its Pieces go to separate table.
        foreach ($crafted in $doc.SelectNodes('//CraftedItem')) {
            $bag = @{ source_mod = $mod.Name; source_file = $xmlFile.Name }
            # Flatten crafted item top-level attributes only (skip Pieces subtree).
            foreach ($attr in $crafted.Attributes) { $bag[$attr.Name] = $attr.Value }
            [void]$craftedItemRows.Add($bag)

            $craftedId = $crafted.GetAttribute('id')
            $pieces = $crafted.SelectNodes('.//Piece')
            $slotIndex = 0
            foreach ($piece in $pieces) {
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

        # <CraftingPiece> (blade / guard / handle / pommel with BladeData etc.)
        foreach ($cp in $doc.SelectNodes('//CraftingPiece')) {
            $bag = @{ source_mod = $mod.Name; source_file = $xmlFile.Name }
            Flatten-Element -Node $cp -Prefix '' -Bag $bag
            [void]$craftingPieceRows.Add($bag)
        }
    }
}

Write-Host "Scanned $totalFiles XML files across $($mods.Count) mods."
Write-Host "  Items:            $($itemRows.Count)"
Write-Host "  CraftedItems:     $($craftedItemRows.Count)"
Write-Host "  CraftedPieces:    $($craftedItemPieceRows.Count)"
Write-Host "  CraftingPieces:   $($craftingPieceRows.Count)"

# --- CSV writer: build union column set, emit rows -----------------------
function Write-BagsToCsv {
    param(
        [System.Collections.Generic.List[hashtable]]$Bags,
        [string]$Path,
        [string[]]$LeadingColumns  # columns forced to appear first (in order)
    )

    # Union of all keys.
    $keySet = New-Object System.Collections.Generic.HashSet[string]
    foreach ($b in $Bags) { foreach ($k in $b.Keys) { [void]$keySet.Add($k) } }

    # Ordered columns: leading first (if present), then rest sorted.
    $orderedCols = New-Object System.Collections.Generic.List[string]
    foreach ($c in $LeadingColumns) {
        if ($keySet.Contains($c)) {
            [void]$orderedCols.Add($c)
            [void]$keySet.Remove($c)
        }
    }
    foreach ($c in ($keySet | Sort-Object)) { [void]$orderedCols.Add($c) }

    # Materialize rows as pscustomobjects in that column order and pipe to Export-Csv.
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

$itemsCsv           = Join-Path $OutDir 'osa_items.csv'
$craftedItemsCsv    = Join-Path $OutDir 'osa_crafted_items.csv'
$craftedPiecesCsv   = Join-Path $OutDir 'osa_crafted_items_pieces.csv'
$craftingPiecesCsv  = Join-Path $OutDir 'osa_crafting_pieces.csv'

Write-BagsToCsv -Bags $itemRows           -Path $itemsCsv          -LeadingColumns @('source_mod','source_file','id','name','Type','culture','subtype','mesh','weight','value','difficulty','appearance','Armor.material_type','Armor.modifier_group','Armor.head_armor','Armor.body_armor','Armor.arm_armor','Armor.leg_armor','Armor.covers_body','Armor.has_gender_variations','Armor.hair_cover_type','Armor.beard_cover_type','Armor.mane_cover_type','Armor.maneuver_bonus','Armor.speed_bonus','Armor.charge_bonus','Armor.family_type','Armor.reins_mesh','Weapon.weapon_class','Weapon.ammo_class','Weapon.weapon_length','Weapon.thrust_damage','Weapon.thrust_damage_type','Weapon.thrust_speed','Weapon.swing_damage','Weapon.swing_damage_type','Weapon.swing_speed','Weapon.speed_rating','Weapon.accuracy','Weapon.missile_speed','Weapon.stack_amount','Weapon.handling','Weapon.modifier_group','Weapon.item_usage')
Write-BagsToCsv -Bags $craftedItemRows    -Path $craftedItemsCsv   -LeadingColumns @('source_mod','source_file','id','name','crafting_template','culture','modifier_group')
Write-BagsToCsv -Bags $craftedItemPieceRows -Path $craftedPiecesCsv -LeadingColumns @('source_mod','source_file','crafted_item_id','slot_index','id','Type','scale_factor')
Write-BagsToCsv -Bags $craftingPieceRows  -Path $craftingPiecesCsv -LeadingColumns @('source_mod','source_file','id','name','tier','piece_type','culture','mesh','weight','length','BladeData.blade_length','BladeData.blade_width','BladeData.stack_amount','BladeData.Swing.damage_type','BladeData.Swing.damage_factor','BladeData.Thrust.damage_type','BladeData.Thrust.damage_factor','StatContributions.armor_bonus','BuildData.piece_offset','BuildData.next_piece_offset','CraftingCost','is_default')

Write-Host "`nDone." -ForegroundColor Yellow
