# Concatenate Chinese preamble + ASCII armor table body into final per-slot
# markdown docs under the OSW mod root. All script content is ASCII; the
# Chinese parts live in preamble_*.md UTF-8 files loaded verbatim.

$ErrorActionPreference = 'Stop'

$modRoot = Split-Path -Parent $PSScriptRoot

$slots = @(
    @{ preamble='armor_preamble_head.md';     body='armor_tables_head.md';     out='ARMOR_DATA_HEAD.md' }
    @{ preamble='armor_preamble_body.md';     body='armor_tables_body.md';     out='ARMOR_DATA_BODY.md' }
    @{ preamble='armor_preamble_shoulder.md'; body='armor_tables_shoulder.md'; out='ARMOR_DATA_SHOULDER.md' }
    @{ preamble='armor_preamble_arm.md';      body='armor_tables_arm.md';      out='ARMOR_DATA_ARM.md' }
    @{ preamble='armor_preamble_leg.md';      body='armor_tables_leg.md';      out='ARMOR_DATA_LEG.md' }
)

foreach ($s in $slots) {
    $preamblePath = Join-Path $PSScriptRoot $s.preamble
    $bodyPath     = Join-Path $PSScriptRoot $s.body
    $outPath      = Join-Path $modRoot $s.out

    $pre  = Get-Content -Raw -LiteralPath $preamblePath -Encoding UTF8
    $body = Get-Content -Raw -LiteralPath $bodyPath     -Encoding UTF8

    $combined = $pre + "`r`n" + $body
    Set-Content -LiteralPath $outPath -Value $combined -Encoding UTF8

    $lineCount = ($combined -split "`r?`n").Count
    Write-Host ("  {0,-28} {1,5} lines" -f $s.out, $lineCount)
}

Write-Host "Done composing 5 slot docs under $modRoot"
