# Fix common ILSpy decomp artifacts that don't compile.
param([string]$Dir)

$ErrorActionPreference = 'Stop'

$files = Get-ChildItem -Path $Dir -Filter "*.cs" -Recurse
$totalRefFix = 0
$totalClosureFix = 0

foreach ($f in $files) {
    $raw = Get-Content -LiteralPath $f.FullName -Raw
    if ([string]::IsNullOrEmpty($raw)) { continue }
    $before = $raw

    # Fix 1: (TYPE)(ref IDENT) → IDENT  (apply repeatedly for nested casts)
    # Pattern allows TYPE to have dots, generics, question marks (nullable)
    $iterCount = 0
    while ($iterCount -lt 10) {
        $new = [regex]::Replace($raw, '\(([A-Za-z0-9_.<>?]+)\)\(\s*ref\s+(\w+(?:\.\w+)*)\s*\)', '$2')
        if ($new -eq $raw) { break }
        $raw = $new
        $iterCount++
    }

    $refCount = ([regex]::Matches($before, '\(([A-Za-z0-9_.<>?]+)\)\(\s*ref\s+(\w+(?:\.\w+)*)\s*\)')).Count
    $totalRefFix += $refCount

    # Fix 2: compiler-generated identifier `<>` prefix → `__` prefix
    # Matches `<>c`, `<>9`, `<>9__XX_Y`, `<>d__N`, `<>e__DisplayClassN` etc.
    # Does NOT touch generic type usage `List<T>` or `Func<,>`.
    $beforeClosure = $raw
    $raw = [regex]::Replace($raw, '<>(\w+)', '__$1')
    $closureCount = ([regex]::Matches($beforeClosure, '<>(\w+)')).Count
    $totalClosureFix += $closureCount

    # Fix 2b: compiler-generated method names `<MethodName>b__N_M` / `<Method>d__N` / `<Method>g__N`
    # Replace angle brackets around identifier when followed by compiler suffix.
    $raw = [regex]::Replace($raw, '<(\w+)>(b__|d__|g__|\w+__)', '_$1_$2')

    # Fix 3: `(ref IDENT)` in comparison expression contexts (not in method call args)
    # Targeted patterns only · NOT a broad replacement (would break foo(ref var))
    $beforeRef2 = $raw
    # `(ref IDENT) != / == / < / > / <= / >= / -> ` → `IDENT ...`
    $raw = [regex]::Replace($raw, '\(\s*ref\s+(\w+(?:\.\w+)*)\s*\)\s*(!=|==|<=|>=|<|>|-)', '$1 $2')
    # `OPERATOR (ref IDENT)` → `OPERATOR IDENT`
    $raw = [regex]::Replace($raw, '(!=|==|<=|>=|<|>|=|,|return)\s*\(\s*ref\s+(\w+(?:\.\w+)*)\s*\)', '$1 $2')
    $ref2Count = ([regex]::Matches($beforeRef2, '\(\s*ref\s+\w+(?:\.\w+)*\s*\)\s*(!=|==|<=|>=)')).Count
    $totalRefFix += $ref2Count

    if ($raw -ne $before) {
        [IO.File]::WriteAllText($f.FullName, $raw, [System.Text.UTF8Encoding]::new($true))
        Write-Host ("  {0,-45}  ref-casts={1,3}  closure-rename={2,4}" -f $f.Name, $refCount, $closureCount)
    }
}
Write-Host ""
Write-Host "Total ref-cast fixes:        $totalRefFix"
Write-Host "Total closure-ident renames: $totalClosureFix"
