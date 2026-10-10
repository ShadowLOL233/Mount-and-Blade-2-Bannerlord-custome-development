# Build ASCII markdown table blobs (one per slot) from armor_data_*.json.
# Output grouping: per-culture subsections, sorted by tier desc then raw armor desc.
# Keeps script content ASCII to avoid Windows PowerShell 5.1 encoding surprises.

$ErrorActionPreference = 'Stop'

$slots = @(
    @{ key='HeadArmor'; json='armor_data_head.json';     out='armor_tables_head.md' }
    @{ key='BodyArmor'; json='armor_data_body.json';     out='armor_tables_body.md' }
    @{ key='Cape';      json='armor_data_shoulder.json'; out='armor_tables_shoulder.md' }
    @{ key='HandArmor'; json='armor_data_arm.json';      out='armor_tables_arm.md' }
    @{ key='LegArmor';  json='armor_data_leg.json';      out='armor_tables_leg.md' }
)

$cultureOrder = @('empire','vlandia','sturgia','battania','khuzait','aserai','nord','looters','neutral_culture','(none)')

function Format-Price([object]$v) {
    if ($null -eq $v) { return 'auto' }
    $s = "$v".Trim()
    if ([string]::IsNullOrEmpty($s)) { return 'auto' }
    return $s
}

function Format-Weight([object]$w) {
    if ($null -eq $w -or $w -eq 0) { return '0' }
    return ("{0:0.##}" -f [double]$w)
}

function Format-Mat([object]$m) {
    $s = "$m".Trim()
    if ([string]::IsNullOrEmpty($s)) { return '-' }
    return $s
}

foreach ($slot in $slots) {
    $jsonPath = Join-Path $PSScriptRoot $slot.json
    $outPath  = Join-Path $PSScriptRoot $slot.out

    $data = Get-Content -Raw -LiteralPath $jsonPath -Encoding UTF8 | ConvertFrom-Json
    $data = @($data)

    $sb = [System.Text.StringBuilder]::new()
    [void]$sb.AppendLine(("<!-- SLOT:{0} COUNT:{1} -->" -f $slot.key, $data.Count))
    [void]$sb.AppendLine()

    # Per-slot source/tier summaries first for quick scanning.
    [void]$sb.AppendLine("<!-- SOURCE_DIST -->")
    [void]$sb.AppendLine("| Source | Count |")
    [void]$sb.AppendLine("|---|---:|")
    $data | Group-Object sourceMod | Sort-Object Count -Descending | ForEach-Object {
        [void]$sb.AppendLine(("| {0} | {1} |" -f $_.Name, $_.Count))
    }
    [void]$sb.AppendLine()

    [void]$sb.AppendLine("<!-- TIER_DIST -->")
    [void]$sb.AppendLine("| Tier | Count |")
    [void]$sb.AppendLine("|---|---:|")
    $data | Group-Object tier | Sort-Object @{Expression={[int]$_.Name};Descending=$true} | ForEach-Object {
        [void]$sb.AppendLine(("| T{0} | {1} |" -f $_.Name, $_.Count))
    }
    [void]$sb.AppendLine()

    # Collect cultures present, in preferred order first then others.
    $presentCultures = ($data | Select-Object -ExpandProperty culture -Unique)
    $ordered = @()
    foreach ($c in $cultureOrder) { if ($presentCultures -contains $c) { $ordered += $c } }
    foreach ($c in $presentCultures) { if ($ordered -notcontains $c) { $ordered += $c } }

    foreach ($culture in $ordered) {
        $rows = @($data | Where-Object { $_.culture -eq $culture })
        if ($rows.Count -eq 0) { continue }

        $sorted = $rows | Sort-Object @{Expression='tier';Descending=$true}, @{Expression='rawArmor';Descending=$true}, @{Expression='stringId';Descending=$false}

        [void]$sb.AppendLine(("<!-- CULTURE:{0} COUNT:{1} -->" -f $culture, $rows.Count))
        [void]$sb.AppendLine("| StringId | Display Name | Mat | Tier | Price | H | B | A | L | Wt | Src |")
        [void]$sb.AppendLine("|---|---|---|---:|---:|---:|---:|---:|---:|---:|---|")
        foreach ($r in $sorted) {
            [void]$sb.AppendLine(("| ``{0}`` | {1} | {2} | T{3} | {4} | {5} | {6} | {7} | {8} | {9} | {10} |" -f `
                $r.stringId, $r.displayName, (Format-Mat $r.materialType), $r.tier,
                (Format-Price $r.value), $r.headArmor, $r.bodyArmor, $r.armArmor, $r.legArmor,
                (Format-Weight $r.weight), $r.sourceMod))
        }
        [void]$sb.AppendLine()
    }

    Set-Content -LiteralPath $outPath -Value $sb.ToString() -Encoding UTF8
    Write-Host ("  {0,-10} -> {1,4} rows -> {2}" -f $slot.key, $data.Count, $slot.out)
}
