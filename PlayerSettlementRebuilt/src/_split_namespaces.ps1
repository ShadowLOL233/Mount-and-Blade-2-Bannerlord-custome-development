# Split monolithic decompiled .cs into per-namespace files.
# Handles both block-scoped (brace on same OR next line) and file-scoped namespaces.

param(
    [string]$InputFile,
    [string]$OutputDir,
    [string]$Mode = 'Standalone'   # 'Standalone' or 'InternalCopy' (changes naming rules)
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path $InputFile)) { throw "InputFile not found: $InputFile" }
New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null

$lines = Get-Content -LiteralPath $InputFile

# Phase 1: find first line that starts with `namespace` → everything above is preamble
$preambleEnd = -1
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match '^namespace\b') { $preambleEnd = $i - 1; break }
}
if ($preambleEnd -lt 0) { throw "No namespace found" }
$preamble = ($lines[0..$preambleEnd]) -join "`r`n"
Write-Host "Preamble: lines 1-$($preambleEnd + 1)"

# Phase 2: find all namespace block boundaries.
$namespaceBlocks = @()
$i = $preambleEnd + 1
while ($i -lt $lines.Count) {
    $line = $lines[$i]
    if ($line -match '^namespace\s+(\S+?)\s*(\{|;)?\s*$') {
        $nsName = $Matches[1].Trim()
        $isFileScoped = ($Matches[2] -eq ';')
        $openOnSameLine = ($Matches[2] -eq '{')
        $startLine = $i

        if ($isFileScoped) {
            $endLine = $lines.Count - 1
            $i = $lines.Count
        } else {
            # Find the opening brace (same line or next non-blank line)
            $braceStart = -1
            if ($openOnSameLine) {
                $braceStart = $i
            } else {
                for ($j = $i + 1; $j -lt $lines.Count; $j++) {
                    if ($lines[$j] -match '^\s*\{') { $braceStart = $j; break }
                    if ($lines[$j] -match '\S' -and $lines[$j] -notmatch '^\s*$') { break }
                }
            }
            if ($braceStart -lt 0) { throw "Namespace $nsName at L$($i+1) · no opening brace" }

            # Count braces from brace line onwards
            $depth = 0
            $endLine = -1
            for ($j = $braceStart; $j -lt $lines.Count; $j++) {
                $l = $lines[$j]
                $depth += ([regex]::Matches($l, '\{')).Count
                $depth -= ([regex]::Matches($l, '\}')).Count
                if ($depth -eq 0 -and $j -ge $braceStart) { $endLine = $j; break }
            }
            if ($endLine -lt 0) { throw "Unterminated namespace at L$($i+1): $nsName" }
            $i = $endLine + 1
        }

        $namespaceBlocks += [pscustomobject]@{
            Name = $nsName
            StartLine = $startLine
            EndLine = $endLine
            LineCount = $endLine - $startLine + 1
        }
    } else {
        $i++
    }
}

Write-Host "Found $($namespaceBlocks.Count) namespace blocks"

# Phase 3: emit one file per namespace with naming rules
foreach ($ns in $namespaceBlocks) {
    $nsSafe = $ns.Name -replace '[^A-Za-z0-9_.]', '_'
    $outName = switch -Regex ($ns.Name) {
        '^Microsoft\.CodeAnalysis$'              { '_Generated_CodeAnalysis.cs' }
        '^System\.Runtime\.CompilerServices$'    { '_Generated_CompilerServices.cs' }
        '^BannerlordPlayerSettlement$'           { 'Main.cs' }
        '^BannerlordPlayerSettlement\.(.+)$'     { "$($Matches[1] -replace '\.', '_').cs" }
        '^PlayerSettlementFixes$' {
            if ($Mode -eq 'InternalCopy') { '_Internal_PlayerSettlementFixesLoader.cs' }
            else                           { 'PlayerSettlementFixes.cs' }
        }
        default                                   { "$nsSafe.cs" }
    }

    $outPath = Join-Path $OutputDir $outName
    $suffix = 2
    while (Test-Path $outPath) {
        $base = [IO.Path]::GetFileNameWithoutExtension($outName)
        $outPath = Join-Path $OutputDir "${base}_${suffix}.cs"
        $suffix++
    }

    $content = @()
    $content += $preamble
    $content += ""
    $content += $lines[$ns.StartLine..$ns.EndLine]

    [IO.File]::WriteAllLines($outPath, $content, [System.Text.UTF8Encoding]::new($true))
    Write-Host ("  {0,-50} {1,5} lines  -> {2}" -f $ns.Name, $ns.LineCount, (Split-Path -Leaf $outPath))
}

Write-Host "Done."
