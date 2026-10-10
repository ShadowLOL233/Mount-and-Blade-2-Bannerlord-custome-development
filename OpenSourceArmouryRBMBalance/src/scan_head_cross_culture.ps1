# OSA v3 cross-culture scan: HeadArmor by (material, style_signature, tier).
# style_signature is derived from the display name by stripping:
#   - decorative tokens (Gilded / Plumed / Jeweled / etc per BALANCE_V2 L295)
#   - cultural prefixes (Imperial / Khuzait / Sturgian / Nordic / ...)
#   - standalone "Helmet" / "Helm" nouns
# and canonicalizing aventail phrases ("over mail" / "over leather" / "with mail" / ...).
# The remaining tokens form the structural fingerprint used to compare cultures.

$ErrorActionPreference = 'Stop'

$jsonPath = Join-Path $PSScriptRoot 'armor_data_head.json'
$outJson  = Join-Path $PSScriptRoot 'head_style_cross_culture_scan.json'

$cultures = @('empire','vlandia','sturgia','battania','khuzait','aserai','nord')
$VARIANCE_THRESHOLD = 5

# Decorative tokens have zero structural meaning (per BALANCE_V2 §Spangenhelm L295).
$decorative = @(
    'plumed','crested','crowned','feathered','gilded','jeweled',
    'southern','eastern','northern',
    'bronze','iron','steel','golden','silver',
    'noble','decorated','redcrest','lordly','ornate','fine','fancy','rough',
    'simple','plain','old','new','worn','rusty','polished',
    'exotic','regal','royal','elite','fancy'
)

# Cultural prefixes - strip when they appear as adjectival modifiers.
$culturalPrefix = @(
    'imperial','khuzait','khuzaiti','khergit','sturgian','sturgic','vlandian',
    'battanian','aserai','aseraic','nordic','rhodok','swadian',
    'empire','empires','khuzaits','vlandians','aserai','aserais'
)

# Common nouns stripped from style signature (shield/helmet are context, not shape).
$genericNouns = @('helmet','helm','helmets','helms')

# Aventail phrase canonicalization (match early so "over mail coif" doesn't collapse to "mail/coif").
$aventailPatterns = @(
    @{ pattern = '\bover\s+mail\s+coif\b';        canon = 'OVERMAILCOIF' }
    @{ pattern = '\bover\s+closed\s+mail\b';      canon = 'OVERCLOSEDMAIL' }
    @{ pattern = '\bclosed\s+mail\b';             canon = 'CLOSEDMAIL' }
    @{ pattern = '\bover\s+imperial\s+mail\b';    canon = 'OVERMAIL' }
    @{ pattern = '\bover\s+mail\b';               canon = 'OVERMAIL' }
    @{ pattern = '\bover\s+padded\s+coif\b';      canon = 'OVERPADDEDCOIF' }
    @{ pattern = '\bover\s+padded\s+cloth\b';     canon = 'OVERPADDEDCLOTH' }
    @{ pattern = '\bover\s+stripped\s+cloth\b';   canon = 'OVERSTRIPPEDCLOTH' }
    @{ pattern = '\bover\s+cloth\b';              canon = 'OVERCLOTH' }
    @{ pattern = '\bover\s+leather\b';            canon = 'OVERLEATHER' }
    @{ pattern = '\bwith\s+mail\b';               canon = 'WITHMAIL' }
    @{ pattern = '\bwith\s+leather\b';            canon = 'WITHLEATHER' }
    @{ pattern = '\bwith\s+metal\s+strips\b';     canon = 'WITHMETALSTRIPS' }
    @{ pattern = '\bwith\s+faceguard\b';          canon = 'WITHFACEGUARD' }
    @{ pattern = '\bwith\s+faceplate\b';          canon = 'WITHFACEPLATE' }
)

function Get-StyleSig([string]$displayName) {
    if ([string]::IsNullOrEmpty($displayName)) { return '(empty)' }
    $s = $displayName.ToLower()

    # Phase 1: canonicalize aventail phrases to single tokens.
    foreach ($p in $aventailPatterns) {
        $s = [regex]::Replace($s, $p.pattern, " $($p.canon) ")
    }

    # Phase 2: tokenize.
    $tokens = $s -split '[\s/,\-]+' | Where-Object { $_ -ne '' }

    # Phase 3: drop decorative / cultural / generic nouns.
    $kept = foreach ($t in $tokens) {
        if ($decorative -contains $t) { continue }
        if ($culturalPrefix -contains $t) { continue }
        if ($genericNouns -contains $t) { continue }
        if ($t -match "^'s$") { continue }
        $t
    }

    if ($kept.Count -eq 0) { return '(generic)' }
    return ($kept -join ' ')
}

$data = Get-Content -Raw -LiteralPath $jsonPath -Encoding UTF8 | ConvertFrom-Json
# Exclude multiplayer-only items ("mp_" prefix) - they ship intentionally lowered
# for MP balance and pollute cross-culture SP comparisons.
$data = @($data | Where-Object { $cultures -contains $_.culture -and $_.stringId -notlike 'mp_*' })

foreach ($r in $data) {
    $mat = if ([string]::IsNullOrEmpty($r.materialType)) { '-' } else { $r.materialType }
    Add-Member -InputObject $r -NotePropertyName 'mat'      -NotePropertyValue $mat -Force
    Add-Member -InputObject $r -NotePropertyName 'styleSig' -NotePropertyValue (Get-StyleSig $r.displayName) -Force
}

# Group by (mat, styleSig, tier).
$buckets = @{}
foreach ($r in $data) {
    $key = "{0}||{1}||T{2}" -f $r.mat, $r.styleSig, $r.tier
    if (-not $buckets.ContainsKey($key)) {
        $buckets[$key] = [pscustomobject]@{
            mat      = $r.mat
            styleSig = $r.styleSig
            tier     = [int]$r.tier
            total    = 0
            items    = @()
        }
    }
    $buckets[$key].total++
    $buckets[$key].items += $r
}

$results = foreach ($b in $buckets.Values) {
    $perCulture = @{}
    foreach ($c in $cultures) {
        $items = @($b.items | Where-Object { $_.culture -eq $c })
        if ($items.Count -eq 0) { $perCulture[$c] = $null; continue }
        $heads = @($items | ForEach-Object { [int]$_.headArmor })
        $perCulture[$c] = [pscustomobject]@{
            count = $items.Count
            minH  = ($heads | Measure-Object -Minimum).Minimum
            maxH  = ($heads | Measure-Object -Maximum).Maximum
            meanH = [math]::Round(($heads | Measure-Object -Average).Average, 1)
        }
    }

    $means = @()
    foreach ($c in $cultures) {
        if ($perCulture[$c] -ne $null) { $means += $perCulture[$c].meanH }
    }
    $culturesWithData = $means.Count
    $variance = if ($culturesWithData -ge 2) {
        ($means | Measure-Object -Maximum).Maximum - ($means | Measure-Object -Minimum).Minimum
    } else { 0 }

    [pscustomobject]@{
        mat              = $b.mat
        styleSig         = $b.styleSig
        tier             = $b.tier
        totalItems       = $b.total
        culturesWithData = $culturesWithData
        variance         = [math]::Round($variance, 1)
        perCulture       = $perCulture
        flagged          = ($culturesWithData -ge 2 -and $variance -gt $VARIANCE_THRESHOLD)
        items            = $b.items
    }
}

$crossCultBuckets = @($results | Where-Object { $_.culturesWithData -ge 2 })
$flagged = @($crossCultBuckets | Where-Object { $_.flagged } | Sort-Object @{Expression='variance';Descending=$true})

Write-Host ("Total buckets (mat x style x tier): {0}" -f $results.Count)
Write-Host ("Buckets with >=2 cultures:          {0}" -f $crossCultBuckets.Count)
Write-Host ("Flagged (variance > {0}):            {1}" -f $VARIANCE_THRESHOLD, $flagged.Count)
Write-Host ""
Write-Host "Top-20 flagged buckets by variance:"
$flagged | Select-Object -First 20 | ForEach-Object {
    Write-Host ("  {0,-10} {1,-45} T{2}  var={3,5}  cult={4}  items={5}" -f `
        $_.mat, $_.styleSig, $_.tier, $_.variance, $_.culturesWithData, $_.totalItems)
}

$results | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $outJson -Encoding UTF8
Write-Host ""
Write-Host ("Scan JSON -> {0}" -f $outJson)
