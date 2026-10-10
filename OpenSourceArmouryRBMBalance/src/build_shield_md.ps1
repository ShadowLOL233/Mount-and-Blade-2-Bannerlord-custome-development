# Build ASCII markdown tables (one per material group) from shield_table_data.json.
# Keeps all script content ASCII to avoid Windows PowerShell 5.1 encoding issues.
# Output is appended into SHIELD_DATA_COMPLETE.md by a subsequent step.

$ErrorActionPreference = 'Stop'

$jsonPath = Join-Path $PSScriptRoot 'shield_table_data.json'
$outPath  = Join-Path $PSScriptRoot 'shield_tables_ascii.md'

$data = Get-Content -Raw -LiteralPath $jsonPath -Encoding UTF8 | ConvertFrom-Json

# Material order used in the final doc: coarse-to-fine by family.
$order = @(
    'Steel','Brass','Bronze','Iron',
    'Strapped Iron','Strapped Bronze','Strapped Norse','Strapped Northern',
    'Highland Iron','Highland Brass','Highland',
    'Wood','Timber','Wicker',
    'Leather','Hide Wrapped','Cloth Wrapped','Wrapped','Wrapped Timber','Wrapped Wicker',
    'Lion Pelt','Leopard Pelt','Zebra Skin',
    'Lion Pelt Adarga','Leopard Pelt Adarga','Zebra Skin Adarga',
    'Woven Reed',
    'Nomadic Steppe','Eastern Rider'
)

$sb = [System.Text.StringBuilder]::new()

foreach ($mat in $order) {
    $rows = @($data | Where-Object { $_.material -eq $mat })
    if ($rows.Count -eq 0) { continue }
    $sorted = $rows | Sort-Object @{Expression='tier';Descending=$true}, @{Expression='hp';Descending=$true}, @{Expression='stringId';Descending=$false}

    [void]$sb.AppendLine("<!-- MAT:$mat COUNT:$($rows.Count) -->")
    [void]$sb.AppendLine("| StringId | Display Name | Tier | HP | BA | Wt | ThrSpd | rawTier |")
    [void]$sb.AppendLine("|---|---|---:|---:|---:|---:|---:|---:|")
    foreach ($r in $sorted) {
        [void]$sb.AppendLine(("| ``{0}`` | {1} | T{2} | {3} | {4} | {5} | {6} | {7} |" -f `
            $r.stringId, $r.displayName, $r.tier, $r.hp, $r.ba, $r.weight, $r.thrust, $r.rawTier))
    }
    [void]$sb.AppendLine()
}

# Tier distribution summary
$tierSummary = $data | Group-Object tier | Sort-Object @{Expression={[int]$_.Name}; Descending=$true}
[void]$sb.AppendLine("<!-- TIER_DIST -->")
[void]$sb.AppendLine("| Tier | Count |")
[void]$sb.AppendLine("|---|---:|")
foreach ($tg in $tierSummary) {
    [void]$sb.AppendLine("| T$($tg.Name) | $($tg.Count) |")
}

Set-Content -LiteralPath $outPath -Value $sb.ToString() -Encoding UTF8
Write-Host ("Wrote {0} tables + summary to {1}" -f $order.Count, $outPath)
