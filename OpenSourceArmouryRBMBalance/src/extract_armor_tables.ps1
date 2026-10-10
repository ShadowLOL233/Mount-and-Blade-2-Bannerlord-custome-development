# Merge vanilla + RBM + OSA item CSVs into per-slot armor JSON files.
# Load precedence (whole-row replace by id, per Bannerlord XML semantics):
#   Vanilla < RBM < OSA.
# Tier via vanilla DefaultItemValueModel.CalculateArmorTier formula
# (documented in BALANCE_V2_LOG.md L257-262):
#   raw    = 1.2 * head + body + leg + arm
#   scaled = raw * typeMultiplier
#   tier   = clamp(round(scaled * 0.1 - 0.4), 0, 6) - 1

$ErrorActionPreference = 'Stop'

$repoRoot   = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$vanillaCsv = Join-Path $repoRoot 'Vanilla_Reference\data\vanilla_items.csv'
$rbmCsv     = Join-Path $repoRoot 'RBM_Reference\data\rbm_items.csv'
$osaCsv     = Join-Path $repoRoot 'OSA_Reference\data\osa_items.csv'

$outDir = $PSScriptRoot

# Only these 5 armor Types are in scope.
$armorTypes = @('HeadArmor','BodyArmor','Cape','HandArmor','LegArmor')

# Tier formula multipliers (BALANCE_V2_LOG L260).
$typeMult = @{
    'HeadArmor' = 1.2
    'BodyArmor' = 1.0
    'Cape'      = 1.8
    'LegArmor'  = 1.6
    'HandArmor' = 1.7
}

function Import-ItemCsv([string]$path, [string]$mergeTag) {
    if (-not (Test-Path -LiteralPath $path)) { throw "Missing CSV: $path" }
    # Import-Csv -Encoding UTF8 handles the BOM.
    $rows = Import-Csv -LiteralPath $path -Encoding UTF8
    foreach ($r in $rows) {
        # Stamp an unambiguous origin tag for the merge-dict.
        Add-Member -InputObject $r -NotePropertyName '_origin' -NotePropertyValue $mergeTag -Force
    }
    return $rows
}

function Get-Int([object]$v) {
    if ($null -eq $v) { return 0 }
    $s = "$v".Trim()
    if ([string]::IsNullOrEmpty($s)) { return 0 }
    $n = 0
    if ([int]::TryParse($s, [ref]$n)) { return $n }
    return 0
}

function Get-DoubleOrZero([object]$v) {
    if ($null -eq $v) { return 0.0 }
    $s = "$v".Trim()
    if ([string]::IsNullOrEmpty($s)) { return 0.0 }
    $n = 0.0
    if ([double]::TryParse($s, [ref]$n)) { return $n }
    return 0.0
}

function Strip-NameToken([string]$name) {
    if ([string]::IsNullOrEmpty($name)) { return '' }
    return ($name -replace '^\{=[^}]+\}', '').Trim()
}

function Strip-CulturePrefix([string]$c) {
    if ([string]::IsNullOrEmpty($c)) { return '(none)' }
    return ($c -replace '^Culture\.', '').Trim()
}

Write-Host "Loading CSVs..."
$vanillaRows = Import-ItemCsv $vanillaCsv 'Vanilla'
$rbmRows     = Import-ItemCsv $rbmCsv     'RBM'
$osaRows     = Import-ItemCsv $osaCsv     'OSA'
Write-Host ("  vanilla={0} rbm={1} osa={2}" -f $vanillaRows.Count, $rbmRows.Count, $osaRows.Count)

# Merge: later source wins; track effective source.
$merged = [ordered]@{}
foreach ($r in $vanillaRows) {
    if ([string]::IsNullOrEmpty($r.id)) { continue }
    $merged[$r.id] = $r
}
foreach ($r in $rbmRows) {
    if ([string]::IsNullOrEmpty($r.id)) { continue }
    $merged[$r.id] = $r
}
foreach ($r in $osaRows) {
    if ([string]::IsNullOrEmpty($r.id)) { continue }
    $merged[$r.id] = $r
}
Write-Host ("Merged unique ids: {0}" -f $merged.Count)

# Filter to armor and project out the fields we care about.
$armorRows = foreach ($r in $merged.Values) {
    $t = "$($r.Type)".Trim()
    if ($armorTypes -notcontains $t) { continue }

    $h = Get-Int $r.'Armor.head_armor'
    $b = Get-Int $r.'Armor.body_armor'
    $a = Get-Int $r.'Armor.arm_armor'
    $l = Get-Int $r.'Armor.leg_armor'

    $raw    = 1.2 * $h + $b + $l + $a
    $mult   = $typeMult[$t]
    $scaled = $raw * $mult
    $tier   = [math]::Max(0, [math]::Min(6, [math]::Round($scaled * 0.1 - 0.4, [MidpointRounding]::AwayFromZero))) - 1

    [pscustomobject]@{
        slot          = $t
        stringId      = $r.id
        displayName   = Strip-NameToken $r.name
        culture       = Strip-CulturePrefix $r.culture
        tier          = [int]$tier
        rawArmor      = [math]::Round($raw, 1)
        scaledArmor   = [math]::Round($scaled, 1)
        headArmor     = $h
        bodyArmor     = $b
        armArmor      = $a
        legArmor      = $l
        materialType  = "$($r.'Armor.material_type')".Trim()
        weight        = Get-DoubleOrZero $r.weight
        value         = "$($r.value)".Trim()
        sourceMod     = "$($r._origin)".Trim()
        sourceFile    = "$($r.source_file)".Trim()
    }
}

Write-Host ("Armor rows after filter: {0}" -f $armorRows.Count)

# Split per slot and write JSON.
$slotFileMap = @{
    'HeadArmor' = 'armor_data_head.json'
    'BodyArmor' = 'armor_data_body.json'
    'Cape'      = 'armor_data_shoulder.json'
    'HandArmor' = 'armor_data_arm.json'
    'LegArmor'  = 'armor_data_leg.json'
}

foreach ($slot in $armorTypes) {
    $rows = @($armorRows | Where-Object { $_.slot -eq $slot })
    $out  = Join-Path $outDir $slotFileMap[$slot]
    $rows | ConvertTo-Json -Depth 3 | Set-Content -LiteralPath $out -Encoding UTF8
    Write-Host ("  {0,-10} -> {1,4} rows -> {2}" -f $slot, $rows.Count, $slotFileMap[$slot])
}

# Also write a flat all-armor file for cross-slot queries.
$allOut = Join-Path $outDir 'armor_data_all.json'
$armorRows | ConvertTo-Json -Depth 3 | Set-Content -LiteralPath $allOut -Encoding UTF8
Write-Host ("  ALL        -> {0,4} rows -> armor_data_all.json" -f $armorRows.Count)
