# Extract complete shield data from OSABalance_shield_override.xml to ASCII JSON.
# Material tag resolved via priority-ordered keyword search (longest match wins).
# Tier computed via vanilla DefaultItemValueModel.CalculateShieldTier formula.

$ErrorActionPreference = 'Stop'

$xmlPath  = Join-Path $PSScriptRoot '..\ModuleData\OSABalance_shield_override.xml'
$jsonPath = Join-Path $PSScriptRoot 'shield_table_data.json'

# Material index = leftmost material keyword in the display name, longest-match-wins
# at that position. Shape words (Wooden Square, Tower, Buckler, Oval, Round, ...)
# are intentionally NOT in this list - they describe form, not substance.
$materials = @(
    'Leopard Pelt Adarga','Lion Pelt Adarga','Zebra Skin Adarga',
    'Hide Wrapped','Cloth Wrapped',
    'Strapped Iron','Strapped Bronze','Strapped Norse','Strapped Northern',
    'Highland Iron','Highland Brass',
    'Nomadic Steppe','Eastern Rider','Woven Reed',
    'Leopard Pelt','Lion Pelt','Zebra Skin',
    'Wrapped Timber','Wrapped Wicker',
    'Highland','Steel','Brass','Bronze','Iron',
    'Timber','Wicker','Leather','Reed','Hide','Wrapped'
)

[xml]$doc = Get-Content -Raw -Encoding UTF8 -LiteralPath $xmlPath

$rows = foreach ($item in $doc.Items.Item) {
    $id       = $item.id
    $rawName  = $item.name
    $displayName = ($rawName -replace '^\{=[^}]+\}', '').Trim()

    $weight   = [double]$item.weight
    $weapon   = $item.ItemComponent.Weapon
    $hp       = [int]$weapon.hit_points
    $ba       = [int]$weapon.body_armor
    $thrust   = [int]$weapon.thrust_speed

    # Tier formula (TaleWorlds.Core v1.4.7).
    $num   = [math]::Pow($hp, 1.22) + 3 * $ba + $thrust
    $den   = 6.0 + [math]::Pow($weight, 1.11)
    $raw   = $num / $den * 0.04 - 2.0
    $tier  = [math]::Max(0, [math]::Min(6, [math]::Round($raw, [MidpointRounding]::AwayFromZero)))

    # Material = leftmost material keyword in display name (longest-match at each
    # position). Shields with no material word (e.g. "Wooden Square Shield") fall
    # back to "Wood" since OSW's bare-wood shields have no explicit material tag.
    $tokens = $displayName -split '\s+'
    $material = 'Wood'
    :outer for ($pos = 0; $pos -lt $tokens.Count; $pos++) {
        foreach ($mat in $materials) {
            $mt = $mat -split '\s+'
            if ($pos + $mt.Count -le $tokens.Count) {
                $slice = ($tokens[$pos..($pos + $mt.Count - 1)]) -join ' '
                if ($slice -ieq $mat) { $material = $mat; break outer }
            }
        }
    }

    [pscustomobject]@{
        material    = $material
        tier        = [int]$tier
        hp          = $hp
        ba          = $ba
        weight      = $weight
        thrust      = $thrust
        stringId    = $id
        displayName = $displayName
        rawTier     = [math]::Round($raw, 2)
    }
}

$rows | ConvertTo-Json -Depth 3 | Set-Content -LiteralPath $jsonPath -Encoding UTF8
Write-Host ("Wrote {0} rows to {1}" -f $rows.Count, $jsonPath)
