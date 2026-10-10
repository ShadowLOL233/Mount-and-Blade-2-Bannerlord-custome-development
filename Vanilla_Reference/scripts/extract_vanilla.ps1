# Extract vanilla + NavalDLC items + crafting pieces into flat CSVs.
#
# Mirrors OSA_Reference/extract_osa.ps1 logic (Flatten-Element, ItemComponent
# transparent unwrap, four output tables). Points at vanilla ModuleData:
#   - SandBoxCore\ModuleData\items\*.xml  (weapons/shields/armor/tournament)
#   - Native\ModuleData\crafting_pieces.xml
#   - Native\ModuleData\mpitems.xml  (kept separate via source_mod tag)
#   - NavalDLC\ModuleData\{items,naval_weapons,naval_crafting_pieces}.xml
#
# Skips subtrees that are asset-only or would collide on flatten
# (AdditionalMeshes, Materials, upgrade_targets, etc.).
#
# Output: Vanilla_Reference/data/{vanilla_items,vanilla_crafted_items,
#         vanilla_crafted_items_pieces,vanilla_crafting_pieces}.csv
#
# Rerun after game updates. Idempotent.

param(
    [string]$GameRoot = 'E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord',
    [string]$OutDir = (Join-Path $PSScriptRoot '..\data')
)

$ErrorActionPreference = 'Stop'

# (source_tag, absolute file path) pairs
$sources = @(
    @{ Tag = 'Vanilla_SP'; File = 'Modules\SandBoxCore\ModuleData\items\weapons.xml' }
    @{ Tag = 'Vanilla_SP'; File = 'Modules\SandBoxCore\ModuleData\items\shields.xml' }
    @{ Tag = 'Vanilla_SP'; File = 'Modules\SandBoxCore\ModuleData\items\tournament_weapons.xml' }
    @{ Tag = 'Vanilla_SP'; File = 'Modules\SandBoxCore\ModuleData\items\horses_and_others.xml' }
    @{ Tag = 'Vanilla_Native'; File = 'Modules\Native\ModuleData\crafting_pieces.xml' }
    @{ Tag = 'Vanilla_Native_MP'; File = 'Modules\Native\ModuleData\mpitems.xml' }
    @{ Tag = 'Vanilla_Native_MP'; File = 'Modules\Native\ModuleData\mp_crafting_pieces.xml' }
    @{ Tag = 'NavalDLC'; File = 'Modules\NavalDLC\ModuleData\items.xml' }
    @{ Tag = 'NavalDLC'; File = 'Modules\NavalDLC\ModuleData\naval_weapons.xml' }
    @{ Tag = 'NavalDLC'; File = 'Modules\NavalDLC\ModuleData\naval_crafting_pieces.xml' }
)

$skipSubtrees = @('AdditionalMeshes','Materials','MeshMultipliers','face','skills','upgrade_targets')

$OutDir = Resolve-Path -LiteralPath $OutDir -ErrorAction SilentlyContinue
if (-not $OutDir) {
    $OutDir = Join-Path $PSScriptRoot '..\data'
    New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
    $OutDir = Resolve-Path -LiteralPath $OutDir
}

Write-Host "Game root: $GameRoot" -ForegroundColor Cyan
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
foreach ($src in $sources) {
    $full = Join-Path $GameRoot $src.File
    if (-not (Test-Path -LiteralPath $full)) {
        Write-Warning "Missing: $full (skipping)"
        continue
    }
    $totalFiles++
    try {
        [xml]$doc = Get-Content -LiteralPath $full -Raw -Encoding UTF8
    } catch {
        Write-Warning "Parse fail: $full - $_"
        continue
    }

    $fileName = Split-Path $src.File -Leaf

    foreach ($item in $doc.SelectNodes('//Item')) {
        $bag = @{ source_mod = $src.Tag; source_file = $fileName }
        Flatten-Element -Node $item -Prefix '' -Bag $bag -SkipChildren $skipSubtrees
        [void]$itemRows.Add($bag)
    }

    foreach ($crafted in $doc.SelectNodes('//CraftedItem')) {
        $bag = @{ source_mod = $src.Tag; source_file = $fileName }
        foreach ($attr in $crafted.Attributes) { $bag[$attr.Name] = $attr.Value }
        [void]$craftedItemRows.Add($bag)

        $craftedId = $crafted.GetAttribute('id')
        $slotIndex = 0
        foreach ($piece in $crafted.SelectNodes('.//Piece')) {
            $pieceBag = @{
                source_mod = $src.Tag
                source_file = $fileName
                crafted_item_id = $craftedId
                slot_index = $slotIndex
            }
            foreach ($attr in $piece.Attributes) { $pieceBag[$attr.Name] = $attr.Value }
            [void]$craftedItemPieceRows.Add($pieceBag)
            $slotIndex++
        }
    }

    foreach ($cp in $doc.SelectNodes('//CraftingPiece')) {
        $bag = @{ source_mod = $src.Tag; source_file = $fileName }
        Flatten-Element -Node $cp -Prefix '' -Bag $bag -SkipChildren $skipSubtrees
        [void]$craftingPieceRows.Add($bag)
    }
}

Write-Host "Scanned $totalFiles source files."
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

$leadItems = @('source_mod','source_file','id','name','Type','culture','subtype','mesh','weight','value','difficulty','appearance','item_category','Armor.material_type','Armor.modifier_group','Armor.head_armor','Armor.body_armor','Armor.arm_armor','Armor.leg_armor','Armor.covers_body','Armor.has_gender_variations','Armor.hair_cover_type','Armor.beard_cover_type','Armor.mane_cover_type','Armor.maneuver_bonus','Armor.speed_bonus','Armor.charge_bonus','Armor.family_type','Armor.reins_mesh','Weapon.weapon_class','Weapon.ammo_class','Weapon.weapon_length','Weapon.thrust_damage','Weapon.thrust_damage_type','Weapon.thrust_speed','Weapon.swing_damage','Weapon.swing_damage_type','Weapon.swing_speed','Weapon.speed_rating','Weapon.accuracy','Weapon.missile_speed','Weapon.stack_amount','Weapon.handling','Weapon.modifier_group','Weapon.item_usage','Weapon.body_armor','Weapon.hit_points','Horse.monster','Horse.maneuver','Horse.speed','Horse.charge_damage','Horse.body_length','Horse.extra_health','Horse.is_mountable','Horse.is_pack_animal','Horse.modifier_group')

Write-BagsToCsv -Bags $itemRows           -Path (Join-Path $OutDir 'vanilla_items.csv')                 -LeadingColumns $leadItems
Write-BagsToCsv -Bags $craftedItemRows    -Path (Join-Path $OutDir 'vanilla_crafted_items.csv')         -LeadingColumns @('source_mod','source_file','id','name','crafting_template','culture','modifier_group')
Write-BagsToCsv -Bags $craftedItemPieceRows -Path (Join-Path $OutDir 'vanilla_crafted_items_pieces.csv') -LeadingColumns @('source_mod','source_file','crafted_item_id','slot_index','id','Type','scale_factor')
Write-BagsToCsv -Bags $craftingPieceRows  -Path (Join-Path $OutDir 'vanilla_crafting_pieces.csv')        -LeadingColumns @('source_mod','source_file','id','name','tier','piece_type','culture','mesh','weight','length','BladeData.blade_length','BladeData.blade_width','BladeData.stack_amount','BladeData.Swing.damage_type','BladeData.Swing.damage_factor','BladeData.Thrust.damage_type','BladeData.Thrust.damage_factor','StatContributions.armor_bonus','BuildData.piece_offset','BuildData.next_piece_offset','CraftingCost','is_default','is_hidden')

Write-Host "`nDone." -ForegroundColor Yellow
