# Strip [assembly: ...] attributes from all split .cs files to prevent duplicates.
# GenerateAssemblyInfo=true in csproj will provide them from <PropertyGroup>.

param([string]$Dir)

Get-ChildItem -Path $Dir -Filter "*.cs" | ForEach-Object {
    $lines = Get-Content -LiteralPath $_.FullName
    $new = @()
    $stripped = 0
    foreach ($l in $lines) {
        if ($l -match '^\s*\[assembly:') { $stripped++; continue }
        $new += $l
    }
    if ($stripped -gt 0) {
        [IO.File]::WriteAllLines($_.FullName, $new, [System.Text.UTF8Encoding]::new($true))
        Write-Host "  Stripped $stripped assembly attrs from $($_.Name)"
    }
}
